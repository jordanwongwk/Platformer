using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [SerializeField] Text lifeText;
    [SerializeField] Text scoreText;
    [SerializeField] int playerLife;

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
        lifeText.text = playerLife.ToString();
        scoreText.text = playerScore.ToString();
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
}
