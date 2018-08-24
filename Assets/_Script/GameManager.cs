using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [Header("Setup")]
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] AudioSource masterMusicPlayer;

    [Header("Water Slider Options")]
    [SerializeField] Slider waterLevelIndicatorSlider;
    [SerializeField] Text waterDistanceToPlayer;
    [SerializeField] Image[] waterLevelIndicatorImages;

    [Header("UI Handler")]
    [SerializeField] Text lifeText;         // TODO do I need life?
    [SerializeField] Text scoreText;
    [SerializeField] int playerLife = 0;

    int playerScore = 0;

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

        masterMusicPlayer.volume = PlayerPrefsManager.GetMusicVolume();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }
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

        waterLevelIndicatorSlider.value = distance / 200.0f;

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

    public void OpenGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    public void ClosePausePanel()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }
}
