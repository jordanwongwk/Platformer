using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeManager : MonoBehaviour {

    [SerializeField] Text multiplierText;

    [Header("Difficulty Description")]
    [SerializeField] GameObject warningNoGameMode;
    [SerializeField] GameObject gameModeTextParent;

    [Header("Handicap Mode")]
    [SerializeField] GameObject handicapTextParent;

    bool gameModeIsChosen = false;
    int divineOrbCharges;
    float waterRisingSpeed;
    float waterInitialSpeed;
    float scoreMultiplier;
    string difficultyString;
    Text introductionGameModeText;
    List<GameObject> gameModeTextObject = new List<GameObject>();

    bool handicapNoIndicator = false;
    bool handicapLimitedLife = false;
    bool handicapOneLife = false;
    bool handicapZeroDivine = false;
    float handicapMultiplier;
    Text introductionHandicapText;
    List<GameObject> handicapTextObject = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < gameModeTextParent.transform.childCount; i++)
        {
            gameModeTextObject.Add(gameModeTextParent.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < handicapTextParent.transform.childCount; i++)
        {
            handicapTextObject.Add(handicapTextParent.transform.GetChild(i).gameObject);
        }

        introductionGameModeText = gameModeTextObject[0].GetComponent<Text>();
        introductionHandicapText = handicapTextObject[0].GetComponent<Text>();
    }

    private void Update()
    {
        multiplierText.text = "x " + (scoreMultiplier + handicapMultiplier).ToString("F2");
    }

    #region Game Modes
    public void EasyDifficultySelected (bool isSelected)
    {
        if (isSelected)
        {
            InitializeGameModeText();
            gameModeTextObject[1].SetActive(true);

            divineOrbCharges = 1;
            waterRisingSpeed = 0.5f;
            waterInitialSpeed = 1.0f;
            scoreMultiplier = 1.0f;
            difficultyString = "Easy";
        }
    }

    public void NormalDifficultySelected(bool isSelected)
    {
        if (isSelected)
        {
            InitializeGameModeText();
            gameModeTextObject[2].SetActive(true);

            divineOrbCharges = 1;
            waterRisingSpeed = 0.75f;
            waterInitialSpeed = 1.5f;
            scoreMultiplier = 2.0f;
            difficultyString = "Normal";
        }
    }

    public void HardDifficultySelected(bool isSelected)
    {
        if (isSelected)
        {
            InitializeGameModeText();
            gameModeTextObject[3].SetActive(true);

            divineOrbCharges = 2;
            waterRisingSpeed = 1.0f;
            waterInitialSpeed = 2.0f;
            scoreMultiplier = 3.0f;
            difficultyString = "Hard";
        }
    }

    private void InitializeGameModeText()
    {
        gameModeIsChosen = true;
        DisableIntroductionText(introductionGameModeText);
        DisableAllTextInGameMode();
    }

    private void DisableAllTextInGameMode()
    {
        foreach (GameObject gameModeText in gameModeTextObject)
        {
            gameModeText.SetActive(false);
        }
    }

    public void ConfirmGameMode()
    {
        if (gameModeIsChosen)
        {
            GetComponent<UIManager>().EnableHandicapPanel();
        }
        else
        {
            warningNoGameMode.SetActive(true);
        }
    }

    public void CloseWarningNoGameMode()
    {
        warningNoGameMode.SetActive(false);
    }
    #endregion

    #region Handicap Mode
    public void NoIndicatorSelected(bool isSelected)
    {
        DisableIntroductionText(introductionHandicapText);
        DisableAllTextInHandicap();
        handicapTextObject[1].SetActive(true);
        handicapNoIndicator = isSelected;

        if (isSelected) { handicapMultiplier += 1.50f; }
        else { handicapMultiplier -= 1.50f; }
    }

    public void LimitedLifeSelected(bool isSelected)
    {
        DisableIntroductionText(introductionHandicapText);
        DisableAllTextInHandicap();
        handicapTextObject[2].SetActive(true);
        handicapLimitedLife = isSelected;

        if (isSelected) { handicapMultiplier += 1.75f; }
        else { handicapMultiplier -= 1.75f; }
    }

    public void OneLifeSelected(bool isSelected)
    {
        DisableIntroductionText(introductionHandicapText);
        DisableAllTextInHandicap();
        handicapTextObject[3].SetActive(true);
        handicapOneLife = isSelected;

        if (isSelected) { handicapMultiplier += 2.50f; }
        else { handicapMultiplier -= 2.50f; }
    }

    public void ZeroDivineSelected(bool isSelected)
    {
        DisableIntroductionText(introductionHandicapText);
        DisableAllTextInHandicap();
        handicapTextObject[4].SetActive(true);
        handicapZeroDivine = isSelected;

        if (isSelected) { handicapMultiplier += 1.50f; }
        else { handicapMultiplier -= 1.50f; }
    }

    void DisableAllTextInHandicap()
    {
        foreach (GameObject handicapText in handicapTextObject)
        {
            handicapText.SetActive(false);
        }
    }

    private void DisableIntroductionText(Text intro)
    {
        if (intro.IsActive())
        {
            intro.gameObject.SetActive(false);
        }
    }
    #endregion

    public void ConfirmGameSettingsSelection()
    {
        float finalMultiplier = scoreMultiplier + handicapMultiplier;

        if (handicapZeroDivine) { divineOrbCharges = 0; }

        GameSettingsManager.SetDivineOrbCharges(divineOrbCharges);
        GameSettingsManager.SetWaterRisingSpeed(waterRisingSpeed);
        GameSettingsManager.SetWaterInitialSpeed(waterInitialSpeed);
        GameSettingsManager.SetScoreMultiplier(finalMultiplier);
        GameSettingsManager.SetDifficultyString(difficultyString);

        GameSettingsManager.SetHandicapNoIndicator(handicapNoIndicator);
        GameSettingsManager.SetHandicapLife(handicapLimitedLife, handicapOneLife);
        GameSettingsManager.SetHandicapZeroDivine(handicapZeroDivine);

        Time.timeScale = 1.0f;

        FindObjectOfType<LevelHandler>().StartGameHandler();
    }
}
