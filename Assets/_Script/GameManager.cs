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
    [SerializeField] float maxSliderLength = 200.0f;

    [Header("Water Caution Level")]
    [SerializeField] float dangerZone = 50.0f;
    [SerializeField] float cautionZone = 100.0f;

    [Header("Score Handler")]
    [SerializeField] Text scoreText;
    [SerializeField] float scoreMultiplier = 1.5f;

    [Header("Game Over Panel")]
    [SerializeField] Text timeText;
    [SerializeField] Text hazardHitsText;
    [SerializeField] Text totalScoreText;


    int playerScore = 0;
    float currentScore;
    Player myPlayer;

    private void Start()
    {
        myPlayer = FindObjectOfType<Player>();
        masterMusicPlayer.volume = PlayerPrefsManager.GetMusicVolume();
        scoreText.text = playerScore.ToString();
    }

    private void Update()
    {
        // Pause Game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }

        ScoreUpdate();
    }

    public void ScoreUpdate()
    {
        currentScore = myPlayer.transform.position.y * 100f * scoreMultiplier;

        if (playerScore >= currentScore) {  return;  }

        playerScore = Mathf.RoundToInt(currentScore);
        scoreText.text = playerScore.ToString();
    }

    public void WaterLevelUpdate(float distance)
    {
        foreach (Image image in waterLevelIndicatorImages)
        {
            if (distance <= dangerZone) { image.color = Color.red; }
            else if (distance <= cautionZone) { image.color = Color.yellow; }
            else { image.color = Color.green; }
        }

        waterLevelIndicatorSlider.value = distance / maxSliderLength;

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

    public void GameOverPanelUpdate()
    {
        // Play time
        float minutes = Mathf.Floor(Time.time / 60f);
        float seconds = Time.time % 60f;
        timeText.text = minutes.ToString("00") + " : " + seconds.ToString("00");

        // Hazard hit counts
        hazardHitsText.text = myPlayer.GetHazardHits().ToString();

        // Final score
        totalScoreText.text = playerScore.ToString();
    }
}
