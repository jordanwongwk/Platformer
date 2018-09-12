using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeManager : MonoBehaviour {

    [SerializeField] Text multiplierText;

    [Header("Difficulty Description")]
    [SerializeField] GameObject warningNoGameMode;
    [SerializeField] Text introductionText;
    [SerializeField] Text easyText;
    [SerializeField] Text normalText;
    [SerializeField] Text hardText;

    [Header("Handicap Mode")]
    [SerializeField] Text introductionHandicapText;
    [SerializeField] GameObject noIndicatorText;
    [SerializeField] GameObject limitedLifeText;
    [SerializeField] GameObject oneLifeText;
    [SerializeField] GameObject zeroDivineText;

    bool gameModeIsChosen = false;
    int divineOrbCharges;
    float waterRisingSpeed;
    float waterInitialSpeed;
    float scoreMultiplier;
    string difficultyString;

    bool handicapNoIndicator = false;
    bool handicapLimitedLife = false;
    bool handicapOneLife = false;
    bool handicapZeroDivine = false;
    float handicapMultiplier;

    #region Game Modes
    public void EasyDifficultySelected (bool isSelected)
    {
        if (isSelected)
        {
            gameModeIsChosen = true;
            DisableIntroductionText(introductionText);
            easyText.gameObject.SetActive(true);
            divineOrbCharges = 1;
            waterRisingSpeed = 0.5f;
            waterInitialSpeed = 1.0f;
            scoreMultiplier = 1.0f;
            difficultyString = "Easy";
        }
        else
        {
            easyText.gameObject.SetActive(false);
        }
    }

    public void NormalDifficultySelected(bool isSelected)
    {
        if (isSelected)
        {
            gameModeIsChosen = true;
            DisableIntroductionText(introductionText);
            normalText.gameObject.SetActive(true);
            divineOrbCharges = 1;
            waterRisingSpeed = 0.75f;
            waterInitialSpeed = 1.5f;
            scoreMultiplier = 2.0f;
            difficultyString = "Normal";
        }
        else
        {
            normalText.gameObject.SetActive(false);
        }
    }

    public void HardDifficultySelected(bool isSelected)
    {
        if (isSelected)
        {
            gameModeIsChosen = true;
            DisableIntroductionText(introductionText);
            hardText.gameObject.SetActive(true);
            divineOrbCharges = 2;
            waterRisingSpeed = 1.0f;
            waterInitialSpeed = 2.0f;
            scoreMultiplier = 3.0f;
            difficultyString = "Hard";
        }
        else
        {
            hardText.gameObject.SetActive(false); 
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

    public void NoIndicatorSelected(bool isSelected)
    {
        DisableIntroductionText(introductionHandicapText);
        DisableAllTextInHandicap();
        noIndicatorText.SetActive(true);
        handicapNoIndicator = isSelected;

        if (isSelected) { handicapMultiplier += 1.50f; }
        else { handicapMultiplier -= 1.50f; }
    }

    public void LimitedLifeSelected(bool isSelected)
    {
        DisableIntroductionText(introductionHandicapText);
        DisableAllTextInHandicap();
        limitedLifeText.SetActive(true);
        handicapLimitedLife = isSelected;

        if (isSelected) { handicapMultiplier += 1.75f; }
        else { handicapMultiplier -= 1.75f; }
    }

    public void OneLifeSelected(bool isSelected)
    {
        DisableIntroductionText(introductionHandicapText);
        DisableAllTextInHandicap();
        oneLifeText.SetActive(true);
        handicapOneLife = isSelected;

        if (isSelected) { handicapMultiplier += 2.50f; }
        else { handicapMultiplier -= 2.50f; }
    }

    public void ZeroDivineSelected(bool isSelected)
    {
        DisableIntroductionText(introductionHandicapText);
        DisableAllTextInHandicap();
        zeroDivineText.SetActive(true);
        handicapZeroDivine = isSelected;

        if (isSelected) { handicapMultiplier += 1.50f; }
        else { handicapMultiplier -= 1.50f; }
    }

    void DisableAllTextInHandicap()
    {
        //TODO Refactor
        noIndicatorText.SetActive(false);
        limitedLifeText.SetActive(false);
        oneLifeText.SetActive(false);
        zeroDivineText.SetActive(false);
    }


    private void DisableIntroductionText(Text intro)
    {
        if (intro.IsActive())
        {
            intro.gameObject.SetActive(false);
        }
    }

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


    private void Update()
    {
        multiplierText.text = "x " + (scoreMultiplier + handicapMultiplier).ToString("F2");
    }
}
