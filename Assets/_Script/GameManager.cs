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

    [Header("Water Caution Mechanism")]
    [SerializeField] float dangerZone = 50.0f;
    [SerializeField] float cautionZone = 100.0f;
    [SerializeField] AudioClip waterRisingAlert;
    [SerializeField] Text waterRisingTextAlert;

    [Header("Score Handler")]
    [SerializeField] Text scoreText;

    [Header("Game Over Panel")]
    [SerializeField] Text timeText;
    [SerializeField] Text hazardHitsText;
    [SerializeField] Text totalScoreText;


    [SerializeField] float timeForRaisingWaterSpeed = 5.0f;     // Fixed for all difficulty

    [SerializeField] float waterSpeedAddition = 0f;     // TODO Change for different difficulty
    [SerializeField] float scoreMultiplier = 0f;

    int playerScore = 0;
    float currentScore;
    float lastRaisedTime = 0;
    Player myPlayer;
    AudioSource managerAudioSource;

    private void Start()
    {
        myPlayer = FindObjectOfType<Player>();
        managerAudioSource = GetComponent<AudioSource>();

        masterMusicPlayer.volume = PlayerPrefsManager.GetMusicVolume();
        managerAudioSource.volume = PlayerPrefsManager.GetSoundVolume();
        waterSpeedAddition = PlayerPrefsManager.GetRisingWaterAdditionalSpeed();
        scoreMultiplier = PlayerPrefsManager.GetScoreMultiplier();

        scoreText.text = playerScore.ToString();
    }

    private void Update()
    {
        // Pause Game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenPausePanel();
        }

        ScoreUpdate();
        CheckForTimeToRaiseWaterSpeed();
    }

    void ScoreUpdate()
    {
        currentScore = myPlayer.transform.position.y * 100f * scoreMultiplier;

        if (playerScore >= currentScore) {  return;  }

        playerScore = Mathf.RoundToInt(currentScore);
        scoreText.text = playerScore.ToString();
    }

    void CheckForTimeToRaiseWaterSpeed()
    {
        if (Time.timeSinceLevelLoad >= timeForRaisingWaterSpeed + lastRaisedTime)
        {
            lastRaisedTime = Time.timeSinceLevelLoad;
            FindObjectOfType<RisingTide>().RisingWaterSpeed(waterSpeedAddition);

            StartCoroutine(WaterRiseWarning());
        }
    }

    IEnumerator WaterRiseWarning()
    {
        managerAudioSource.PlayOneShot(waterRisingAlert);
        waterRisingTextAlert.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);          // TODO set a variable or const?
        waterRisingTextAlert.gameObject.SetActive(false);
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

    public void OpenPausePanel()
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    public void ClosePausePanel()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    public void GameOverPanelUpdate()
    {
        // Play time
        float minutes = Mathf.Floor(Time.timeSinceLevelLoad / 60f);
        float seconds = Time.timeSinceLevelLoad % 60f;
        timeText.text = minutes.ToString("00") + " : " + seconds.ToString("00");

        // Hazard hit counts
        hazardHitsText.text = myPlayer.GetHazardHits().ToString();

        // Final score
        totalScoreText.text = playerScore.ToString();
    }
}
