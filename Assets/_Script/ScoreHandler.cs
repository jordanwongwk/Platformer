using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct HighScoreStat
{
    public int score;
    public string difficulty;
    public bool handicapNoIndicator;
    public bool handicapLimitedLives;
    public bool handicapOneLife;
    public bool handicapZeroDivine;
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

    void Start ()
    {
        myPlayer = FindObjectOfType<Player>();

        if (FindObjectOfType<GameSettingsManager>() != null)
        {
            scoreMultiplier = GameSettingsManager.GetScoreMultiplier();
            playerDifficulty = GameSettingsManager.GetDifficultyString();
        }
        else
        {
            // Call when play from level straight from editor!
            Debug.Log("No GameSettingsManager script exists. Setting to default diffculty.");
            scoreMultiplier = 1.0f;
            playerDifficulty = "Default";
        }

        scoreText.text = playerScore.ToString();
    }
	
	void Update ()
    {
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
            statTemp.handicapNoIndicator = PlayerPrefsManager.GetHandicapNoIndicator(i + 1);
            statTemp.handicapLimitedLives = PlayerPrefsManager.GetHandicapLimitedLives(i + 1);
            statTemp.handicapOneLife = PlayerPrefsManager.GetHandicapOneLife(i + 1);
            statTemp.handicapZeroDivine = PlayerPrefsManager.GetHandicapZeroDivine(i + 1);
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
            PlayerPrefsManager.SetHandicapNoIndicator(i + 1, highScores[i].handicapNoIndicator);
            PlayerPrefsManager.SetHandicapLimitedLives(i + 1, highScores[i].handicapLimitedLives);
            PlayerPrefsManager.SetHandicapOneLife(i + 1, highScores[i].handicapOneLife);
            PlayerPrefsManager.SetHandicapZeroDivine(i + 1, highScores[i].handicapZeroDivine);
        }
    }

    private void AddInCurrentScoreStats()
    {
        HighScoreStat currentStat = new HighScoreStat();
        currentStat.score = playerScore;
        currentStat.difficulty = playerDifficulty;
        currentStat.handicapNoIndicator = GameSettingsManager.GetHandicapNoIndicator();
        currentStat.handicapLimitedLives = GameSettingsManager.GetHandicapLimitedLives();
        currentStat.handicapOneLife = GameSettingsManager.GetHandicapOneLife();
        currentStat.handicapZeroDivine = GameSettingsManager.GetHandicapZeroDivine();
        highScores.Add(currentStat);                    
    }

    public bool GetHighScoreStatus()
    {
        return isHighScore;
    }
}
