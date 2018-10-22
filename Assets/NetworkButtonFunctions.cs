﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkButtonFunctions : NetworkBehaviour
{
    [Header("User Interactable UI")]
    [SerializeField] Button startGameButton;
    [SerializeField] InputField ipAddressInput;

    [Header("Pop-Up Messages")]
    [SerializeField] GameObject connectingMessage;
    [SerializeField] GameObject disconnectMessage;

    public int readyPlayerCount = 0;
    int minimumPlayersToStart;
    bool playerIsInRoom = false;

    List<GameObject> activeWindows = new List<GameObject>();

    private void Start()
    {
        minimumPlayersToStart = FindObjectOfType<MyNetworkLobbyManager>().minPlayers;
    }

    #region Multiplayer Main Menu
    // Button Click to Start Host Room
    public void OnPressStartHostRoom()
    {
        NetworkManager.singleton.StartHost();
    }

    // Button Click to Join Hosted Room based on IP on InputField
    public void OnPressJoinRoom()
    {
        NetworkManager.singleton.networkAddress = ipAddressInput.text;
        NetworkClient client = NetworkManager.singleton.StartClient();
        ConnectingMessage();
        client.RegisterHandler(MsgType.Disconnect, OnDisconnected);
    }

    // Display Connecting window pop-up when joining
    void ConnectingMessage()
    {
        RegisteringActiveWindow(connectingMessage);
    }

    // Upon disconnection or connection timed-out when joining
    void OnDisconnected(NetworkMessage netMsg)
    {
        Debug.Log("Player has disconnected.");
        if (playerIsInRoom)
        {
            LobbyPlayerScript.currentLocalInstance.OnPressPlayerLeavingRoom();
            playerIsInRoom = false;
        }

        RemoveActiveWindow(connectingMessage);
        RegisteringActiveWindow(disconnectMessage);
    }

    // When successfully connected to room
    public void OnSuccessfulConnectedToRoom()
    {
        playerIsInRoom = true;
        RemoveActiveWindow(connectingMessage);
        RemoveActiveWindow(disconnectMessage);
    }

    // Button pressed (On Connecting Window) to cancel joining attempt and close window
    public void OnClickCancelConnectionAttempt()
    {
        NetworkManager.singleton.StopClient();
        RemoveActiveWindow(connectingMessage);
    }

    // Button pressed (On Disconnection Window) to close the window
    public void CloseDisconnectWindow()
    {
        RemoveActiveWindow(disconnectMessage);
    }

    // Following 2 functions are registering / removing active windows (For clean managing UI)
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
    #endregion

    #region Multiplayer Room
    // Check to see if the minimum number of players is achieved and make Start button interactable if achieved
    public void ReadyCheckToEnableStartButton(bool status)
    {
        if (!isServer) { return; }                  // Only server / host can start game

        if (status) { readyPlayerCount++; }
        else if (!status) { readyPlayerCount--; }

        if (readyPlayerCount >= minimumPlayersToStart)
        {
            FindObjectOfType<MultiplayerAudioManager>().ReadyToStartClip();
            startGameButton.interactable = true;
        }
        else
        {
            startGameButton.interactable = false;
        }
    }

    // When game has started on button pressed, prompt loading screen.
    public void OnPressStartGame()
    {
        var players = FindObjectsOfType<LobbyPlayerScript>();
        foreach (var joinedPlayer in players)
        {
            joinedPlayer.PromptLoadingScreen();
        }
    }
    #endregion
}
