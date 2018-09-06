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

    [Header("Water Mechanism")]
    [SerializeField] float timeForRaisingWaterSpeed = 5.0f;     // Fixed for all difficulty
    [SerializeField] float dangerZone = 50.0f;
    [SerializeField] float cautionZone = 100.0f;
    [SerializeField] Text waterRisingTextAlert;
    [SerializeField] GameObject waterFrozenBorder;
    [SerializeField] GameObject waterFrozenAlert;

    [Header("Game Over")]
    [SerializeField] Text timeText;
    [SerializeField] Text hazardHitsText;
    [SerializeField] Text totalScoreText;
    [SerializeField] AudioClip NormalGameOver;
    [SerializeField] AudioClip HighScoreGameOver;
    [SerializeField] GameObject HighScorePanel;

    float waterSpeedAddition = 0f;
    float lastRaisedTime = 0;
    AudioSource managerAudioSource;
    RisingTide myRisingTide;

    const float WAIT_TIME = 3.0f;

    static float soundVolume;

    private void Awake()
    {
        myRisingTide = FindObjectOfType<RisingTide>();

        waterSpeedAddition = PlayerPrefsManager.GetRisingWaterAdditionalSpeed();
        masterMusicPlayer.volume = PlayerPrefsManager.GetMusicVolume();
        soundVolume = PlayerPrefsManager.GetSoundVolume();
        managerAudioSource = GetComponent<AudioSource>();
        managerAudioSource.volume = soundVolume;
    }

    public static float GetSoundVolume()
    {
        return soundVolume;
    }

    private void Update()
    {
        // Pause Game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenPausePanel();
        }
        CheckForTimeToRaiseWaterSpeed();
    }


    void CheckForTimeToRaiseWaterSpeed()
    {
        bool isWaterFrozen = myRisingTide.GetWaterFrozenStatus();
        if (!isWaterFrozen && Time.timeSinceLevelLoad >= timeForRaisingWaterSpeed + lastRaisedTime)
        {
            lastRaisedTime = Time.timeSinceLevelLoad;
            myRisingTide.RisingWaterSpeed(waterSpeedAddition);

            StartCoroutine(WaterRiseWarning());
        }
    }

    IEnumerator WaterRiseWarning()
    {
        waterRisingTextAlert.gameObject.SetActive(true);
        yield return new WaitForSeconds(WAIT_TIME);        
        waterRisingTextAlert.gameObject.SetActive(false);
    }

    public void SetWaterFrozenUI(bool status)
    {
        waterFrozenBorder.SetActive(status);
        waterFrozenAlert.SetActive(status);
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
        PlayGameOverSound();
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
        hazardHitsText.text = FindObjectOfType<Player>().GetHazardHits().ToString();

        // Final score
        totalScoreText.text = FindObjectOfType<ScoreHandler>().GetFinalScore().ToString();
    }

    void PlayGameOverSound()
    {
        bool HighScoreStatus = FindObjectOfType<ScoreHandler>().GetHighScoreStatus();
        if (HighScoreStatus)
        {
            HighScorePanel.SetActive(true);
            managerAudioSource.PlayOneShot(HighScoreGameOver);
        }
        else
        {
            HighScorePanel.SetActive(false);
            managerAudioSource.PlayOneShot(NormalGameOver);
        }
    }

    public void PauseMusic(bool isPausing)
    {
        if (isPausing)
        {
            masterMusicPlayer.Pause();
        }
        else
        {
            masterMusicPlayer.UnPause();
        }
    }
}
