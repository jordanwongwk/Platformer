using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerDistanceSlider : MonoBehaviour
{
    GameObject currentPlayer;
    GameObject opponentPlayer;
    Slider playerDistanceSlider;
    PlayerConnectionObject thisPlayerConnectionObject;

    // Use this for initialization
    void Start()
    {
        thisPlayerConnectionObject = GetComponentInParent<PlayerConnectionObject>();
        playerDistanceSlider = GetComponent<Slider>();

        RegisterCurrentPlayerGameObject();
        RegisterOpponentPlayerGameObject();
    }

    // Registering current player object with permission to re-loop if null
    void RegisterCurrentPlayerGameObject()
    {
        if (thisPlayerConnectionObject.GetThisPlayerGameObject() != null)
        {
            currentPlayer = thisPlayerConnectionObject.GetThisPlayerGameObject();
        }
        else
        {
            StartCoroutine(SearchForPlayerGOAgain());
        }
    }

    IEnumerator SearchForPlayerGOAgain()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        RegisterCurrentPlayerGameObject();
    }

    // Looking and registering opponent player (Requires currentPlayer first)
    void RegisterOpponentPlayerGameObject()
    {
        if (thisPlayerConnectionObject.GetThisPlayerOpponentGameObject() != null)
        {
            opponentPlayer = thisPlayerConnectionObject.GetThisPlayerOpponentGameObject();
        }
        else
        {
            StartCoroutine(SearchForOpoonentGOAgain());
        }
    }

    IEnumerator SearchForOpoonentGOAgain()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        RegisterOpponentPlayerGameObject();
    }

    // Update is called once per frame
    void Update()
    {
        // If there are no opponent, there will be error when continue running the script
        if (opponentPlayer == null)
        {
            Debug.LogError("Opponent Player not found! Please make sure the implementation is done correctly or don't play alone!");
            return;
        }

        // If value is -ve means this player is leading (as currentPlayer's position will be higher)
        // TODO Please adjust the position based on need! (X or Y)
        float distanceBetweenPlayer = opponentPlayer.transform.position.x - currentPlayer.transform.position.x;
        if (distanceBetweenPlayer <= 0)
        {
            UpdateDistanceSlider(0);
        }
        else
        {
            UpdateDistanceSlider(distanceBetweenPlayer);
        }
    }

    void UpdateDistanceSlider(float distance)
    {
        playerDistanceSlider.value = distance;
    }

    // DO NOTE THAT Only the Server will get the Match Winner, that is why the hard coded return of int is such. 
    public int GetMatchWinner()
    {
        if (currentPlayer.transform.position.x > opponentPlayer.transform.position.x) { return 1; }         // Server Wins
        else if (opponentPlayer.transform.position.x > currentPlayer.transform.position.x) { return 2; }    // Client Wins
        else { return 0; }      // Draw
    }
}
