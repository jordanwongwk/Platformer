using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [Header("General")]
    [SerializeField] Text waterDistanceToPlayer;
    [SerializeField] Text scoreText;

    [Header("Normal Mode")]
    [SerializeField] Text lifeText;         // TODO do I need life?
    [SerializeField] int playerLife = 0;

    int playerScore = 0;
    public static GameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if (lifeText != null)
        {
            lifeText.text = playerLife.ToString();
        }
        else if (scoreText != null)
        {
            scoreText.text = playerScore.ToString();
        }
    }

    public void RestartLevel()
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentLevelIndex);
    }

    public void ProceedNextLevel()
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentLevelIndex + 1);
    }

    public void ScoreUpdate(int score)
    {
        playerScore += score;
        scoreText.text = playerScore.ToString();
    }

    public void LifeUpdate(int life)
    {
        playerLife += life;
        lifeText.text = playerLife.ToString();
    }

    public void WaterLevelUpdate(float distance)
    {
        waterDistanceToPlayer.text = distance.ToString("F2");       // To 2 Decimal Points
    }
}
