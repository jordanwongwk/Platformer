using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour {

    [Header("Setup")]
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject pausePanel;

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

    [Header("Handicaps")]
    [SerializeField] GameObject waterLevelIndicatorObject;
    [SerializeField] GameObject lifePanel;
    [SerializeField] Text lifeText;

    [Header("Pause")]
    [SerializeField] GameObject retryConfirmationWindow;
    [SerializeField] GameObject quitConfirmationWindow;

    [Header("Game Over")]
    [SerializeField] Text timeText;
    [SerializeField] Text hazardHitsText;
    [SerializeField] Text totalScoreText;
    [SerializeField] AudioClip NormalGameOver;
    [SerializeField] AudioClip HighScoreGameOver;
    [SerializeField] GameObject HighScorePanel;
    [SerializeField] GameObject DrownedDeath;
    [SerializeField] GameObject HazardDeath;

    float waterSpeedAddition = 0f;
    float lastRaisedTime = 0;
    float distanceDiff;
    bool isTheGamePausing = false;
    GameObject currentActiveWindow;
    AudioSource masterMusicPlayer;
    AudioSource managerAudioSource;
    Player myPlayer;
    RisingTide myRisingTide;

    const float WAIT_TIME = 3.0f;

    static float soundVolume;

    private void Awake()
    {
        myPlayer = FindObjectOfType<Player>();
        myRisingTide = FindObjectOfType<RisingTide>();

        masterMusicPlayer = FindObjectOfType<MasterMusicPlayer>().GetComponent<AudioSource>();
        soundVolume = PlayerPrefsManager.GetSoundVolume();
        managerAudioSource = GetComponent<AudioSource>();
        managerAudioSource.volume = soundVolume;
    }

    private void Start()
    {
        if (FindObjectOfType<GameSettingsManager>() != null)
        {
            waterSpeedAddition = GameSettingsManager.GetWaterRisingSpeed();

            if (GameSettingsManager.GetHandicapNoIndicator())
            {
                waterLevelIndicatorObject.SetActive(false);
            }

            if (GameSettingsManager.GetHandicapLimitedLives() || GameSettingsManager.GetHandicapOneLife())
            {
                lifePanel.SetActive(true);
                UpdateLifeCount();
            }
            else
            {
                lifePanel.SetActive(false);
            }
        }
        else
        {
            // Call when play from level straight from editor!
            Debug.Log("No GameSettingsManager script exists. Setting to default diffculty.");
            waterSpeedAddition = 1.0f;
        }
    }

    public void UpdateLifeCount()
    {
        int limitedLifeValue = myPlayer.GetLifeCount();
        lifeText.text = limitedLifeValue.ToString();
    }

    public static float GetSoundVolume()
    {
        return soundVolume;
    }

    public float GetDistanceDifference()
    {
        return distanceDiff;
    }

    private void Update()
    {
        // Pause Game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isTheGamePausing)
            {
                OpenPausePanel();
            }
            else if (isTheGamePausing && currentActiveWindow != pausePanel)
            {
                CloseConfirmation();
            }
            else
            {
                ClosePausePanel();
            }
        }
        CheckForTimeToRaiseWaterSpeed();
        CalculateDistanceDifference();
    }

    private void CalculateDistanceDifference()
    {
        distanceDiff = myPlayer.transform.position.y - myRisingTide.transform.position.y;
        WaterLevelUpdate(distanceDiff);
    }

    void CheckForTimeToRaiseWaterSpeed()
    {
        bool isWaterFrozen = myRisingTide.GetWaterFrozenStatus();
        if (!isWaterFrozen && Time.timeSinceLevelLoad >= timeForRaisingWaterSpeed + lastRaisedTime)
        {
            lastRaisedTime = Time.timeSinceLevelLoad;
            myRisingTide.RisingWaterSpeed(waterSpeedAddition);

            StartCoroutine(TextActiveTime());
        }
    }

    IEnumerator TextActiveTime()
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


    #region Pause Part
    public void OpenPausePanel()
    {
        if (myPlayer.GetIsPlayerAlive())
        {
            Time.timeScale = 0f;
            currentActiveWindow = pausePanel;
            isTheGamePausing = true;
            pausePanel.SetActive(true);
        }
    }

    public void ClosePausePanel()
    {
        Time.timeScale = 1f;
        isTheGamePausing = false;
        pausePanel.SetActive(false);
    }

    public void OpenUpRetryConfirmation()
    {
        currentActiveWindow = retryConfirmationWindow;
        retryConfirmationWindow.SetActive(true);
    }

    public void OpenUpQuitConfirmation()
    {
        currentActiveWindow = quitConfirmationWindow;
        quitConfirmationWindow.SetActive(true);
    }

    public void CloseConfirmation()
    {
        currentActiveWindow.SetActive(false);
        currentActiveWindow = pausePanel;
    }
    #endregion

    #region Game Over Part
    public void OpenGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        PlayGameOverSound();
    }

    public void GameOverPanelUpdate(bool isDrowningDeath)
    {
        // Choosing death image
        if (isDrowningDeath) { DrownedDeath.SetActive(true); }
        else { HazardDeath.SetActive(true); }

        // Play time
        int minutes = Mathf.FloorToInt(Time.timeSinceLevelLoad / 60f);
        int seconds = Mathf.FloorToInt(Time.timeSinceLevelLoad % 60f);
        timeText.text = minutes.ToString("00") + " : " + seconds.ToString("00");

        // Hazard hit counts
        hazardHitsText.text = myPlayer.GetHazardHits().ToString();

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
    #endregion
}
