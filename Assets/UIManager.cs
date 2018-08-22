using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    [Header("Panels Configurations")]
    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameObject creditsPanel;
    [SerializeField] GameObject quitConfirmationPanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EnableQuitPanel();
        }
    }

    #region Options Panel
    public void EnableOptionsPanel()
    {
        Time.timeScale = 0f;
        optionsPanel.SetActive(true);
    }

    public void ConfirmOptionChanges()
    {
        // TODO Recognize changes and implement to player prefs
        Debug.Log("Player Prefs Updated!");
        Time.timeScale = 1f;
        optionsPanel.SetActive(false);
    }

    public void CancelOptionChanges()
    {
        // TODO nullified changes?
        Time.timeScale = 1f;
        optionsPanel.SetActive(false);
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
