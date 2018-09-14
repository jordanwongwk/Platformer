using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    [Header("Panels Configurations")]
    [SerializeField] GameObject gameModeSelectionPanel;
    [SerializeField] GameObject handicapPanel;
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
                if (handicapPanel.activeInHierarchy) { DisableHandicapPanel(); }
                else if (gameModeSelectionPanel.activeInHierarchy) { DisableGameModeSelectionPanel(); }
                else if (leaderboardPanel.activeInHierarchy) { DisableLeaderboardPanel(); }
                else if (optionsPanel.activeInHierarchy) { CancelOptionChanges(); }
                else if (creditsPanel.activeInHierarchy) { DisableCreditsPanel(); }
                else if (quitConfirmationPanel.activeInHierarchy) { DisableQuitPanel(); }
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

    public void EnableHandicapPanel()
    {
        // A Panel is already ON
        handicapPanel.SetActive(true);
    }

    public void DisableHandicapPanel()
    {
        // Go back to Game Mode
        handicapPanel.SetActive(false);
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

    //TODO Delete this on launch
    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
