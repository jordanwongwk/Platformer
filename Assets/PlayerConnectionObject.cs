using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnectionObject : NetworkBehaviour
{
    public int playerID;
    public GameObject playerUnitPrefab;

    [Header("Player / Client UI")]
    [SerializeField] GameObject forfeitConfirmationWindow;
    [SerializeField] GameObject disconnectionWindow;

    GameServerManager mainGameServerManager;
    GameObject thisPlayerGameObject;
    GameObject opponentPlayerGameObject;
    List<GameObject> activeWindows = new List<GameObject>();

    #region Initialization
    // Use this for initialization
    void Start()
    {
        if (isServer)
        {
            SettingUpGameServerManager();
        }

        if (!isLocalPlayer)
        {
            return;
        }

        // Setting up ID for players
        if (isServer) { playerID = 1; }
        else { playerID = 2; }

        transform.GetChild(0).gameObject.SetActive(true);
        CmdRequestForMyUnit();
        SettingUpPlayerInSpawn();
        SettingUpOpponentPlayer();

        NetworkClient thisClient = NetworkManager.singleton.client;
        thisClient.RegisterHandler(MsgType.Disconnect, OnDisconnect);
    }

    // Set up the Game Server Manager script
    // The setting up of the script is such because of authority issue. P2 (Client) could not issue command for server's
    // GameServerManager because of . That is why they must inherit the controller's script.
    void SettingUpGameServerManager()
    {
        // Local Players will set their GameServerManager as their script 
        if (isLocalPlayer)
        {
            mainGameServerManager = GetComponent<GameServerManager>();
        }
        else
        {
            // Other non-local player will set the local player's script as their gameServerManagerScript
            var gameServerManagerScript = FindObjectsOfType<GameServerManager>();
            foreach (GameServerManager script in gameServerManagerScript)
            {
                if (script.gameObject != this.gameObject)
                {
                    mainGameServerManager = script;

                    if (mainGameServerManager == null)
                    {
                        StartCoroutine(SearchForGameServerManagerAgain());
                    }
                }
            }
        }
    }


    IEnumerator SearchForGameServerManagerAgain()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        SettingUpGameServerManager();
    }

    // ONLY-SERVER: Set the spawner main controller as server's player
    private void SettingUpPlayerInSpawn()
    {
        // Set the player in SpawnScript
        if (isServer)
        {
            if (FindObjectOfType<NetworkGeneratorScript>() != null)
            {
                FindObjectOfType<NetworkGeneratorScript>().SetServerAsPlayer(gameObject);
            }
            else
            {
                StartCoroutine(DelaySpawn());
            }
        }
    }

    IEnumerator DelaySpawn()
    {
        yield return new WaitForSeconds(0.25f);
        SettingUpPlayerInSpawn();
    }

    // Setting up all other players that are NOT controlled by this player (aka their opponent)
    void SettingUpOpponentPlayer()
    {
        // REQUIREMENT: This script must know who is their owner first before searching for opponent
        if (thisPlayerGameObject == null)
        {
            StartCoroutine(WaitUntilCurrentPlayerIsRegistered());
            return;
        }

        var players = FindObjectsOfType<NetworkPlayer>();
        foreach (NetworkPlayer player in players)
        {
            if (player.gameObject != thisPlayerGameObject)
            {
                opponentPlayerGameObject = player.gameObject;
                break;
            }
        }
    }

    IEnumerator WaitUntilCurrentPlayerIsRegistered()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        SettingUpOpponentPlayer();
    }

    // Register disconnection messages / windows
    void OnDisconnect(NetworkMessage msg)
    {
        Debug.Log("Disconnected cause server is lost.");
        Time.timeScale = 0f;
        RegisteringActiveWindow(disconnectionWindow);
    }
    #endregion

    #region Getters
    public int GetThisPlayerID()
    {
        return playerID;
    }

    public GameObject GetThisPlayerGameObject()
    {
        return thisPlayerGameObject;
    }

    public GameObject GetThisPlayerOpponentGameObject()
    {
        return opponentPlayerGameObject;
    }
    #endregion

    #region Button Click Methods
    public void OnClickReturnToMainMenu()
    {
        Time.timeScale = 1.0f;
        if (isServer) { NetworkManager.singleton.StopHost(); }
        else { NetworkManager.singleton.StopClient(); }
    }

    public void OnClickForfeitConfirmationWindow()
    {
        RegisteringActiveWindow(forfeitConfirmationWindow);
    }

    public void OnClickCancelForfeit()
    {
        RemoveActiveWindow(forfeitConfirmationWindow);
    }

    public void OnClickConfirmForfeit()
    {
        RemoveActiveWindow(forfeitConfirmationWindow);
        CmdPlayerIsForfeiting(playerID);
    }
    #endregion

    // Managing active windows
    void RegisteringActiveWindow(GameObject window)
    {
        window.SetActive(true);
        activeWindows.Add(window);
    }

    void RemoveActiveWindow(GameObject window)
    {
        window.SetActive(false);
        activeWindows.Remove(window);
    }

    #region Commands
    [Command]
    void CmdPlayerIsForfeiting(int id)
    {
        mainGameServerManager.ForfeitingGame(id);
    }

    [Command]
    void CmdRequestForMyUnit()
    {
        if (connectionToClient.isReady)
        {
            SpawnUnit();
        }
        else
        {
            StartCoroutine(SpawnWhenReady());
        }
    }

    IEnumerator SpawnWhenReady()
    {
        while (!connectionToClient.isReady)
        {
            yield return new WaitForSeconds(0.25f);
        }
        SpawnUnit();
    }

    private void SpawnUnit()
    {
        thisPlayerGameObject = Instantiate(playerUnitPrefab);
        thisPlayerGameObject.transform.position = new Vector3(6, 3, 0);            // Position to Spawn Players
        NetworkServer.SpawnWithClientAuthority(thisPlayerGameObject, connectionToClient);
        TargetSettingUpPlayerAfterSpawn(connectionToClient, thisPlayerGameObject);
    }
    #endregion

    #region RPC
    [TargetRpc]
    void TargetSettingUpPlayerAfterSpawn(NetworkConnection conn, GameObject player)
    {
        thisPlayerGameObject = player;
        thisPlayerGameObject.GetComponent<NetworkPlayer>().SetPlayerID(playerID);        // Giving Player an ID
        FindObjectOfType<CinemachineSetupScript>().SetFollowCameraTarget(player);
    }
    #endregion
}

