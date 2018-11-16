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
    [SerializeField] GameObject contactUsPanel;

    [Header("Error and Confirmation")]
    [SerializeField] GameObject warningNoGameMode;
    [SerializeField] GameObject quitConfirmationPanel;

    List<GameObject> currentlyActivePanels = new List<GameObject>();
    
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
                if (isOptionPanelOn)
                {
                    CancelOptionChanges();
                }
                else
                {
                    PanelIsTurningOff(currentlyActivePanels[currentlyActivePanels.Count - 1]);
                }
            }
        }
    }

    private void PanelIsTurningOn(GameObject targettedPanel)
    {
        currentlyActivePanels.Add(targettedPanel);
        targettedPanel.SetActive(true);
        Time.timeScale = 0;
        isAnyPanelOn = true;
    }

    private void PanelIsTurningOff(GameObject targettedPanel)
    {
        currentlyActivePanels.Remove(targettedPanel);
        targettedPanel.SetActive(false);

        if (currentlyActivePanels.Count == 0)
        {
            Time.timeScale = 1;
            isAnyPanelOn = false;
        }
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
        PanelIsTurningOn(handicapPanel);
    }

    public void DisableHandicapPanel()
    {
        PanelIsTurningOff(handicapPanel);
    }

    public void EnableNoGameModeError()
    {
        PanelIsTurningOn(warningNoGameMode);
    }

    public void DisableNoGameModeError()
    {
        PanelIsTurningOff(warningNoGameMode);
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

    #region Contact Us Panel
    public void EnableContactUsPanel()
    {
        PanelIsTurningOn(contactUsPanel);
    }

    public void DisableContactUsPanel()
    {
        PanelIsTurningOff(contactUsPanel);
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
