using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using System.Net;
using System.Net.Sockets;

public class LobbyPlayerScript : NetworkBehaviour
{

    public static LobbyPlayerScript currentLocalInstance;

    [SerializeField] GameObject playerIdentity;
    [SerializeField] GameObject playerPublicPanel;
    [SerializeField] Image playerStatusPanel;
    [SerializeField] Image loadingPanel;
    [SerializeField] Text playerIPAddress;

    [SyncVar] bool isThisPlayerReady;

    public int playerID;
    Vector2 playerPanelPosition;
    UIMultiplayerManager offlineUIManager;
    NetworkButtonFunctions networkButtonUIManager;

    const int PRIVATE_CANVAS_INT = 0;
    const int PUBLIC_CANVAS_INT = 1;

    #region Initialization
    // Delay is needed for the Start() function to get the playerPanelPosition first before anchoring the position of actual panel
    public override void OnStartClient()
    {
        base.OnStartClient();
        StartCoroutine(DelayInitialize());
    }

    IEnumerator DelayInitialize()
    {
        yield return new WaitForEndOfFrame();
        playerPublicPanel.GetComponent<Image>().rectTransform.anchoredPosition = playerPanelPosition;
    }

    void Start()
    {
        // Hardcoded designated playerID, only applicable for 2 players. Might need to research better solution.
        if (isServer)
        {
            if (isLocalPlayer) { playerID = 1; }
            else { playerID = 2; }
        }
        else
        {
            if (isLocalPlayer) { playerID = 2; }
            else { playerID = 1; }
        }

        // Initialize Variable
        offlineUIManager = FindObjectOfType<UIMultiplayerManager>();
        networkButtonUIManager = FindObjectOfType<NetworkButtonFunctions>();

        // Get panel position based on playerID
        playerPanelPosition = FindObjectOfType<LobbyUIManager>().GetPanelPosition(playerID);

        // Enable public canvas (for local AND non-local players)
        transform.GetChild(PUBLIC_CANVAS_INT).gameObject.SetActive(true);

        // Non-local player to run the next method within and exit immediately
        if (!isLocalPlayer)
        {
            NonLocalPlayerInitialStatus(isThisPlayerReady);
            return;
        }

        // Only accessible by local player
        currentLocalInstance = this;

        offlineUIManager.OpenMultiplayerRoomPanel(true);
        networkButtonUIManager.OnSuccessfulConnectedToRoom();
        transform.GetChild(PRIVATE_CANVAS_INT).gameObject.SetActive(true);
        CmdIsPlayerReady(false);

        UpdatePlayerIdentity();

        // Show only the room's host IP Address
        if (isServer)
        {
            playerIPAddress.text = "IP Address is: " + LocalIPAddress();
        }
        else
        {
            playerIPAddress.text = "IP Address is: " + NetworkManager.singleton.networkAddress;
        }
    }

    // (For Non-Local Player) To get the initial state of all other players the moment client enters the lobby
    void NonLocalPlayerInitialStatus(bool status)
    {
        if (status) { PlayerIsReady(true); }
        else { PlayerIsReady(false); }
    }

    private void PlayerIsReady(bool isReady)
    {
        if (isReady)
        {
            playerStatusPanel.color = Color.green;
            playerStatusPanel.GetComponentInChildren<Text>().text = "READY!";
        }
        else
        {
            playerStatusPanel.color = Color.red;
            playerStatusPanel.GetComponentInChildren<Text>().text = "Not Ready";
        }
    }

    void UpdatePlayerIdentity()
    {
        if (playerID == 1)
        {
            Color blueColor = new Color32(0, 124, 255, 255);
            playerIdentity.GetComponent<Image>().color = blueColor;
            playerIdentity.GetComponentInChildren<Text>().text = "Player: Blue";
        }
        else if (playerID == 2)
        {
            Color redColor = new Color32(255, 62, 52, 255);
            playerIdentity.GetComponent<Image>().color = redColor;
            playerIdentity.GetComponentInChildren<Text>().text = "Player: Red";
        }
        else
        {
            Debug.LogError("Error: PlayerID Invalid (UpdatePlayerIdentity Method)");
        }
    }

    // To get the IP Address of the local player via native C# coding
    public string LocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }
    #endregion

    #region Button Pressed Functions
    // Button clicked to make this player ready
    public void OnPressPlayerReadyToStart()
    {
        if (isLocalPlayer && !isThisPlayerReady)
        {
            isThisPlayerReady = true;
            CmdIsPlayerReady(isThisPlayerReady);
            CmdPlayerReadyCheck(isThisPlayerReady);
        }
    }

    // Button clicked to make this player not ready
    public void OnPressPlayerNotReadyToStart()
    {
        if (isLocalPlayer && isThisPlayerReady)
        {
            isThisPlayerReady = false;
            CmdIsPlayerReady(isThisPlayerReady);
            CmdPlayerReadyCheck(isThisPlayerReady);
        }
    }

    // Button clicked to make this player leave the current room
    public void OnPressPlayerLeavingRoom()
    {
        if (isLocalPlayer)
        {
            if (isThisPlayerReady) { CmdPlayerReadyCheck(false); }      // Forced the player to set to NOT READY first before leaving
            networkButtonUIManager.SetPlayerIsInRoomBool(false);
            offlineUIManager.DisableJoinRoomButtonUponLeaving();
            StartCoroutine(LeaveRoomDelay());
        }
    }

    // Provide a small delay for the previous call to execute (Forced NOT READY part) before allowing player to leave
    IEnumerator LeaveRoomDelay()
    {
        yield return new WaitForSecondsRealtime(0.05f);
        offlineUIManager.OpenMultiplayerRoomPanel(false);

        if (isServer) { NetworkManager.singleton.StopHost(); }
        else { NetworkManager.singleton.StopClient(); }
    }
    #endregion

    // When "START GAME", prompt the following action to server
    public void PlayerIsReadyToStart()
    {
        CmdPlayerIsReadyToStart(true);
    }

    // [Call from NetworkManager] When scene changed, disable all private and public canvas
    public void SceneChangeUIButtonsUpdate()
    {
        transform.GetChild(PRIVATE_CANVAS_INT).gameObject.SetActive(false);
        transform.GetChild(PUBLIC_CANVAS_INT).gameObject.SetActive(false);
    }

    #region Commands
    // When player is ready, set the appropriate color and RPC to show other clients its current state
    [Command]
    void CmdIsPlayerReady(bool status)
    {
        isThisPlayerReady = status;

        if (status)
        {
            PlayerIsReady(status);
            RpcIsPlayerReady(status);
        }
        else
        {
            PlayerIsReady(status);
            RpcIsPlayerReady(status);
        }
    }

    // Send CMD to server and call it to check if the minimum number to enable start button is achieved or not
    [Command]
    void CmdPlayerReadyCheck(bool status)
    {
        networkButtonUIManager.ReadyCheckToEnableStartButton(status);
    }

    // When game start, the CMD is called to tell the server to load up the loading panel
    [Command]
    void CmdPlayerIsReadyToStart(bool status)
    {
        RpcPlayerIsReadyToStart(status);
    }
    #endregion

    #region RPC
    // Set the appropriate color when player is set to READY or NOT READY
    [ClientRpc]
    void RpcIsPlayerReady(bool status)
    {
        PlayerIsReady(status);
    }

    // Called to pop-up the loading panel on other clients and at the same time ask it to send Ready Message!
    [ClientRpc]
    void RpcPlayerIsReadyToStart(bool status)
    {
        loadingPanel.gameObject.SetActive(status);
        if (isLocalPlayer)
        {
            GetComponent<NetworkLobbyPlayer>().SendReadyToBeginMessage();
        }
    }
    #endregion
}

