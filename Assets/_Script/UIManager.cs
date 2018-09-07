using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    [Header("Panels Configurations")]
    [SerializeField] GameObject gameModeSelectionPanel;
    [SerializeField] GameObject leaderboardPanel;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameObject creditsPanel;
    [SerializeField] GameObject quitConfirmationPanel;

    bool isOptionPanelOn = false;
    bool isAnyPanelOn = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isAnyPanelOn)
            {
                EnableQuitPanel();
            }
            else
            {
                if (gameModeSelectionPanel.activeInHierarchy)   { DisableGameModeSelectionPanel(); }
                if (leaderboardPanel.activeInHierarchy) { DisableLeaderboardPanel(); }
                if (optionsPanel.activeInHierarchy) { CancelOptionChanges(); }
                if (creditsPanel.activeInHierarchy) { DisableCreditsPanel(); }
                if (quitConfirmationPanel.activeInHierarchy) { DisableQuitPanel(); }
            }
        }
    }

    private void PanelIsTurningOn(GameObject targettedPanel)
    {
        Time.timeScale = 0f;
        isAnyPanelOn = true;
        targettedPanel.SetActive(true);
    }

    private void PanelIsTurningOff(GameObject targettedPanel)
    {
        Time.timeScale = 1f;
        isAnyPanelOn = false;
        targettedPanel.SetActive(false);
    }

    #region Game Mode Selection Panel
    public void EnableGameModeSelectionPanel()
    {
        PanelIsTurningOn(gameModeSelectionPanel);
    }

    public void DisableGameModeSelectionPanel()
    {
        PanelIsTurningOff(gameModeSelectionPanel);
    }
    #endregion

    #region Leaderboard Panel
    public void EnableLeaderboardPanel()
    {
        GetComponent<LeaderboardManager>().GetLeaderboardScore();
        PanelIsTurningOn(leaderboardPanel);
    }

    public void DisableLeaderboardPanel()
    {
        PanelIsTurningOff(leaderboardPanel);
    }
    #endregion

    #region Options Panel
    public void EnableOptionsPanel()
    {
        isOptionPanelOn = true;
        PanelIsTurningOn(optionsPanel);
    }

    public void ConfirmOptionChanges()
    {
        float musicVolumeChange = GetComponent<OptionManager>().GetMusicVolumeChange();
        float soundVolumeChange = GetComponent<OptionManager>().GetSoundVolumeChange();

        PlayerPrefsManager.SetMusicVolume(musicVolumeChange);
        PlayerPrefsManager.SetSoundVolume(soundVolumeChange);
        isOptionPanelOn = false;
        PanelIsTurningOff(optionsPanel);
    }

    public void CancelOptionChanges()
    {
        GetComponent<OptionManager>().DefaultOptions();
        isOptionPanelOn = false;
        PanelIsTurningOff(optionsPanel);
    }

    public bool OptionPanelStatus()
    {
        return isOptionPanelOn;
    }
    #endregion

    #region Credits Panel
    public void EnableCreditsPanel()
    {
        PanelIsTurningOn(creditsPanel);
    }

    public void DisableCreditsPanel()
    {
        PanelIsTurningOff(creditsPanel);
    }
    #endregion

    #region Quit Confirmation Panel
    public void EnableQuitPanel()
    {
        PanelIsTurningOn(quitConfirmationPanel);
    }

    public void DisableQuitPanel()
    {
        PanelIsTurningOff(quitConfirmationPanel);
    }
    #endregion
}
