using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerDistanceSlider : MonoBehaviour
{
    [SerializeField] Text distanceText;
    [SerializeField] List<GameObject> colorChangeObjects;

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
            return;
        }

        // If value is -ve means this player is leading (as currentPlayer's position will be higher)
        float distanceBetweenPlayer = opponentPlayer.transform.position.y - currentPlayer.transform.position.y;
        UpdateDistanceSlider(distanceBetweenPlayer);
    }

    void UpdateDistanceSlider(float distance)
    {
        // Slider position
        playerDistanceSlider.value = distance;

        // Slider text indicator
        float absoluteDistance = Mathf.Abs(distance);
        distanceText.text = Mathf.RoundToInt(absoluteDistance).ToString() + " m";

        // Slider indicator color
        if (distance < 0) { ChangeImageColor(Color.green); }
        else if (distance > 0) { ChangeImageColor(Color.red); }
        else { ChangeImageColor(Color.yellow); }
    }

    void ChangeImageColor(Color designatedColor)
    {
        foreach (GameObject imageObject in colorChangeObjects)
        {
            imageObject.GetComponent<Image>().color = designatedColor;
        }
    }

    // BELOW ARE ONLY CALLED BY SERVER!
    // DO NOTE THAT Only the Server will get the Match Winner, that is why the hard coded return of int is such. 
    public int GetMatchWinner()
    {
        if (currentPlayer.transform.position.y > opponentPlayer.transform.position.y) { return 1; }         // Server Wins
        else if (opponentPlayer.transform.position.y > currentPlayer.transform.position.y) { return 2; }    // Client Wins
        else { return 0; }      // Draw
    }

    public int GetServerPlayerDistanceTravelled()
    {
        int playerPosition = Mathf.RoundToInt(currentPlayer.transform.position.y);
        return playerPosition;
    }

    public int GetClientPlayerDistanceTravelled()
    {
        int playerPosition = Mathf.RoundToInt(opponentPlayer.transform.position.y);
        return playerPosition;
    }
}
