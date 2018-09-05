using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreHandler : MonoBehaviour {
    [SerializeField] Text scoreText;

    List<int> highScores = new List<int>();

    bool isHighScore = false;
    int playerScore = 0;
    float currentScore;
    float scoreMultiplier = 0f;
    Player myPlayer;

    const int NUMBER_OF_PLACINGS = 3;

    void Start () {
        myPlayer = FindObjectOfType<Player>();

        scoreMultiplier = PlayerPrefsManager.GetScoreMultiplier();
        scoreText.text = playerScore.ToString();
    }
	
	void Update () {
        ScoreUpdate();
    }

    void ScoreUpdate()
    {
        currentScore = myPlayer.transform.position.y * 100f * scoreMultiplier;

        if (playerScore >= currentScore) { return; }

        playerScore = Mathf.RoundToInt(currentScore);
        scoreText.text = playerScore.ToString();
    }

    public int GetFinalScore()
    {

        CheckForHighScore();
        return playerScore; 
    }

    public void CheckForHighScore()
    {
        for (int i = 0; i < NUMBER_OF_PLACINGS; i++)                     
        {
            highScores.Add(PlayerPrefsManager.GetHighScore(i + 1));
        }

        highScores.Add(playerScore);                    // Add current score into List
        highScores.Sort((a,b) => b.CompareTo(a));

        // If score is in List (not the last), play high score music
        if (highScores[highScores.Count - 1] != playerScore)     
        {
            Debug.Log("High Score!");
            isHighScore = true;
        }

        for (int i = 0; i < NUMBER_OF_PLACINGS; i++)                     
        {
            PlayerPrefsManager.SetHighScore(i + 1, highScores[i]);
        }
    }

    public bool GetHighScoreStatus()
    {
        return isHighScore;
    }
}
