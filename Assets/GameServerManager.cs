using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameServerManager : NetworkBehaviour
{
    [Header("Gameplay Configuration")]
    [SerializeField] float timeLimit;

    [Header("Setting Up")]
    [SerializeField] GameObject generalCanvasObject;
    [SerializeField] GameObject timesUpTextImage;
    [SerializeField] GameObject distanceSlider;
    [SerializeField] List<GameObject> resultPanels;

    [Header("Setting up text field")]
    [SerializeField] Text timeText;

    [SyncVar] bool isThisTimeKeeper = false;
    [SyncVar] bool playerForfeits = false;
    [SyncVar] int winnerPlayerID = -1;

    bool isTheGameOver = false;
    float currentTime;
    MultiplayerDistanceSlider distanceSliderScript; 

    const int SERVER_PLAYER_ID = 1;
    const int CLIENT_PLAYER_ID = 2;

    // Initialization
    void Start()
    {
        // If the object is a server AND a local player = it is the one and only main server that overlooks the game
        if (isServer && isLocalPlayer)
        {
            isThisTimeKeeper = true;
            distanceSliderScript = distanceSlider.GetComponent<MultiplayerDistanceSlider>();
        }

        // The "isThisTimeKeeper" boolean is a SyncVar, the server has made it TRUE. This means on the client side,
        // the similar object will have this boolean TRUE as well. Thus, the following method is run by the Player 1
        // Connection Object for both on server and on P2 client!
        if (isThisTimeKeeper)
        {
            currentTime = timeLimit;
            generalCanvasObject.SetActive(true);
        }
    }

    void Update()
    {
        // If "isThisTimeKeeper" boolean is FALSE, it MUST not run the following codes after this line.
        if (!isThisTimeKeeper) { return; }

        // If the game is NOT over yet
        if (!isTheGameOver)
        {
            // The main server will be the one calculating the on-going time.
            if (isServer)
            {
                CalculatingServerTime();
            }

            // The time will be output as Text format here
            int currentTimeInt = Mathf.RoundToInt(currentTime);
            timeText.text = (currentTimeInt / 60).ToString() + ":" + (currentTimeInt % 60).ToString("00");

            // This method is called to check if the current time surpasses 0 and below to end game
            CheckToSeeIfTimeIsUp();
        }
    }

    // Counting down time limit
    private void CalculatingServerTime()
    {
        // If none of the player forfeits during the game. The time will count down normally. If any player forfeits,
        // the current time will instantly become zero, thus ending the game immediately.
        if (!playerForfeits)
        {
            currentTime -= Time.deltaTime;
        }
        else
        {
            currentTime = 0f;
        }

        // Send an RPC to the other client's "server" to update the time on their side.
        RpcSendCurrentTimeToAllClient(currentTime);
    }

    // Check if the count down has touch 0
    private void CheckToSeeIfTimeIsUp()
    {
        if (currentTime <= 0f)
        {
            isTheGameOver = true;
            DisplayingEndGameUI();

            if (!playerForfeits)
            {
                TallyMatchResult();         // 1st End Game : Calculating who is the furtherest when time ends
            }
            else
            {
                DisplayingResults();        // 2nd End Game : Player who forfeits automatically lose the game
            }
        }
    }

    // Displaying "Time's Up!" Text and coroutine for result panel
    private void DisplayingEndGameUI()
    {
        timesUpTextImage.SetActive(true);
        // TODO Whistle sound?
        StartCoroutine(EndingGame());
    }

    // Displaying Result Panel after a certain delay
    IEnumerator EndingGame()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(3.0f);
        DisplayingResults();
    }

    // 1st End Game: Standard victory condition calculation (By distance)
    void TallyMatchResult()
    {
        // Getting the winner ID based on the distance slider
        if (isServer && isLocalPlayer)
        {
            winnerPlayerID = distanceSliderScript.GetMatchWinner();
        }

        // If the number is "-1" by default, try to get the result again. 
        if (winnerPlayerID < 0)
        {
            StartCoroutine(TallyResultAgain());
        }
    }

    IEnumerator TallyResultAgain()
    {
        yield return new WaitForEndOfFrame();
        TallyMatchResult();
    }

    // Displaying the final result on result panel.
    private void DisplayingResults()
    {
        if (isServer) { ResultDisplayingPanel(SERVER_PLAYER_ID, CLIENT_PLAYER_ID); }
        else { ResultDisplayingPanel(CLIENT_PLAYER_ID, SERVER_PLAYER_ID); }
    }

    // Generalized Panel Display method
    private void ResultDisplayingPanel(int playerID, int opponentID)
    {
        if (winnerPlayerID == playerID) { resultPanels[0].SetActive(true); }
        else if (winnerPlayerID == opponentID) { resultPanels[1].SetActive(true); }
        else { resultPanels[2].SetActive(true); }
    }

    // 2nd Victory Condition by opponent surrendering
    public void ForfeitingGame(int forfeitPlayerID)
    {
        playerForfeits = true;

        // If Player 1 forfeits, Player 2 wins, vice versa
        if (forfeitPlayerID == SERVER_PLAYER_ID) { winnerPlayerID = CLIENT_PLAYER_ID; }
        else if (forfeitPlayerID == CLIENT_PLAYER_ID) { winnerPlayerID = SERVER_PLAYER_ID; }
    }

    [ClientRpc]
    void RpcSendCurrentTimeToAllClient(float time)
    {
        currentTime = time;
    }
}

