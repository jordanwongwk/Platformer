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

    [Header("UI Handler")]
    [SerializeField] Text lifeText;         // TODO do I need life?
    [SerializeField] Text scoreText;
    [SerializeField] float scoreMultiplier = 1.5f;
    [SerializeField] int playerLife = 0;


    int playerScore = 0;
    float currentScore = 0;
    Player myPlayer;

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

        myPlayer = FindObjectOfType<Player>();
        masterMusicPlayer.volume = PlayerPrefsManager.GetMusicVolume();
    }

    private void Update()
    {
        // Pause Game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            ScoreUpdate();
        }
    }

    void ScoreUpdate()
    {
        if (playerScore >= currentScore) { return; }

        currentScore = myPlayer.transform.position.y * scoreMultiplier * 100f;
        playerScore = Mathf.RoundToInt(currentScore);
        scoreText.text = playerScore.ToString();
    }

    //public void LifeUpdate(int life)
    //{
    //    playerLife += life;
    //    lifeText.text = playerLife.ToString();
    //}

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
}
