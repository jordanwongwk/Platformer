using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    [Header("Panels Configurations")]
    [SerializeField] GameObject gameModeSelectionPanel;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameObject creditsPanel;
    [SerializeField] GameObject quitConfirmationPanel;

    bool isOptionPanelOn = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EnableQuitPanel();
        }
    }

    #region Game Mode Selection Panel
    public void EnableGameModeSelectionPanel()
    {
        Time.timeScale = 0f;
        gameModeSelectionPanel.SetActive(true);
    }

    public void DisableGameModeSelectionPanel()
    {
        Time.timeScale = 1f;
        gameModeSelectionPanel.SetActive(false);
    }
    #endregion

    #region Options Panel
    public void EnableOptionsPanel()
    {
        Time.timeScale = 0f;
        isOptionPanelOn = true;
        optionsPanel.SetActive(true);
    }

    public void ConfirmOptionChanges()
    {
        Time.timeScale = 1f;
        float musicVolumeChange = GetComponent<OptionManager>().GetMusicVolumeChange();
        float soundVolumeChange = GetComponent<OptionManager>().GetSoundVolumeChange();

        PlayerPrefsManager.SetMusicVolume(musicVolumeChange);
        PlayerPrefsManager.SetSoundVolume(soundVolumeChange);
        isOptionPanelOn = false;
        optionsPanel.SetActive(false);
    }

    public void CancelOptionChanges()
    {
        Time.timeScale = 1f;
        GetComponent<OptionManager>().DefaultOptions();
        isOptionPanelOn = false;
        optionsPanel.SetActive(false);
    }

    public bool OptionPanelStatus()
    {
        return isOptionPanelOn;
    }
    #endregion

    #region Credits Panel
    public void EnableCreditsPanel()
    {
        Time.timeScale = 0f;
        creditsPanel.SetActive(true);
    }

    public void DisableCreditsPanel()
    {
        Time.timeScale = 1f;
        creditsPanel.SetActive(false);
    }
    #endregion

    #region Quit Confirmation Panel
    public void EnableQuitPanel()
    {
        Time.timeScale = 0f;
        quitConfirmationPanel.SetActive(true);
    }

    public void DisableQuitPanel()
    {
        Time.timeScale = 1f;
        quitConfirmationPanel.SetActive(false);
    }
    #endregion
}
