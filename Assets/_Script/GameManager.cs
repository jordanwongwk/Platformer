using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [Header("General")]
    [SerializeField] Text waterDistanceToPlayer;
    [SerializeField] Image[] waterLevelIndicatorImages;
    [SerializeField] Slider waterLevelIndicatorSlider;
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
        // TODO make distance check a separate function / delegate (because might need to change BGM)
        foreach (Image image in waterLevelIndicatorImages)
        {
            if (distance <= 50.0f) { image.color = Color.red; }
            else if (distance <= 100.0f) { image.color = Color.yellow; }
            else { image.color = Color.green; }
        }

        waterLevelIndicatorSlider.value = (1 - distance / 200.0f);

        // TODO Int or Float?
        if (distance > 0f)
        {
            int distanceRoundUp = Mathf.RoundToInt(distance);
            waterDistanceToPlayer.text = distanceRoundUp.ToString() + " m";
        }
        else
        {
            waterDistanceToPlayer.text = "0 m";
        }
    }
}
