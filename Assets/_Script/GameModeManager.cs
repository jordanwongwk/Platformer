using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeManager : MonoBehaviour {

    [Header("Difficulty Description")]
    [SerializeField] Text introductionText;
    [SerializeField] Text easyText;
    [SerializeField] Text normalText;
    [SerializeField] Text hardText;

    float waterRisingSpeed;
    float waterInitialSpeed;
    float scoreMultiplier;
    string difficultyString;

    public void EasyDifficultySelected (bool isSelected)
    {
        if (isSelected)
        {
            DisableIntroductionText();
            easyText.gameObject.SetActive(true);
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
            DisableIntroductionText();
            normalText.gameObject.SetActive(true);
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
            DisableIntroductionText();
            hardText.gameObject.SetActive(true);
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

    private void DisableIntroductionText()
    {
        if (introductionText.IsActive())
        {
            introductionText.gameObject.SetActive(false);
        }
    }

    public void ConfirmGameModeSelection()
    {
        PlayerPrefsManager.SetRisingWaterAdditionalSpeed(waterRisingSpeed);
        PlayerPrefsManager.SetRisingWaterInitialSpeed(waterInitialSpeed);
        PlayerPrefsManager.SetScoreMultiplier(scoreMultiplier);
        PlayerPrefsManager.SetDifficultyString(difficultyString);
        Time.timeScale = 1.0f;

        FindObjectOfType<LevelHandler>().StartGameHandler();
    }
}
