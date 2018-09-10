using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct HighScoreStat
{
    public int score;
    public string difficulty;
}

public class ScoreHandler : MonoBehaviour {
    [SerializeField] Text scoreText;

    List<HighScoreStat> highScores = new List<HighScoreStat>();

    bool isHighScore = false;
    string playerDifficulty;
    int playerScore = 0;
    float currentScore;
    float scoreMultiplier = 0f;
    Player myPlayer;

    const int NUMBER_OF_PLACINGS = 3;

    void Start () {
        myPlayer = FindObjectOfType<Player>();

        scoreMultiplier = PlayerPrefsManager.GetScoreMultiplier();
        playerDifficulty = PlayerPrefsManager.GetDifficultyString();
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
            HighScoreStat statTemp = new HighScoreStat();
            statTemp.score = PlayerPrefsManager.GetHighScore(i + 1);
            statTemp.difficulty = PlayerPrefsManager.GetHighScoreDifficulty(i + 1);
            highScores.Add(statTemp);
        }

        AddInCurrentScoreStats();
        highScores.Sort((a, b) => b.score.CompareTo(a.score));

        // If score is in List (not the last), play high score music
        if (highScores[highScores.Count - 1].score != playerScore)
        {
            Debug.Log("High Score!");
            isHighScore = true;
        }

        for (int i = 0; i < NUMBER_OF_PLACINGS; i++)
        {
            PlayerPrefsManager.SetHighScore(i + 1, highScores[i].score);
            PlayerPrefsManager.SetHighScoreDifficulty(i + 1, highScores[i].difficulty);
        }
    }

    private void AddInCurrentScoreStats()
    {
        HighScoreStat currentStat = new HighScoreStat();
        currentStat.score = playerScore;
        currentStat.difficulty = playerDifficulty;
        highScores.Add(currentStat);                    // Add current score into List
    }

    public bool GetHighScoreStatus()
    {
        return isHighScore;
    }
}
