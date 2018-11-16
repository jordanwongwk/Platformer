using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkLobbyManager : NetworkLobbyManager {

    // This only runs on server!
    public override void OnLobbyServerConnect(NetworkConnection conn)
    {
        FindObjectOfType<MultiplayerAudioManager>().PlayerEnterLeaveRoom();
        // TODO While extra player is being kicked out and start game, the game cannot start (freeze)
    }

    public override void OnLobbyServerDisconnect(NetworkConnection conn)
    {
        FindObjectOfType<MultiplayerAudioManager>().PlayerEnterLeaveRoom();
    }

    public override void OnLobbyServerPlayersReady()
    {
        Debug.Log("Counting down 3 seconds");
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        yield return new WaitForSecondsRealtime(3.0f);
        base.OnLobbyServerPlayersReady();
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);

        var players = FindObjectsOfType<LobbyPlayerScript>();
        foreach (var joinedPlayer in players)
        {
            joinedPlayer.SceneChangeUIButtonsUpdate();
        }
    }

    public override void OnLobbyStopClient()
    {
        base.OnLobbyStopClient();
        Debug.Log("Stop Client");
    }

    public override void OnLobbyStartHost()
    {
        base.OnLobbyStartHost();
        Debug.Log("Start Host");
    }

    public override void OnLobbyStopHost()
    {
        base.OnLobbyStopHost();
        Debug.Log("Stop Host");
    }

    // If player tries to join a room that is full
    public override void OnLobbyClientAddPlayerFailed()
    {
        base.OnLobbyClientAddPlayerFailed();
        Debug.Log("Full");
        FindObjectOfType<NetworkButtonFunctions>().JoinAttemptFailDueToRoomIsFull();
    }
}
