using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMultiplayerManager : MonoBehaviour {

    [SerializeField] GameObject multiplayerRoomPanel;
    [SerializeField] GameObject tutorialPanel;
    [SerializeField] Button joinRoomButton;

    bool isPlayerBusyInRoom = false;

    public void OpenMultiplayerRoomPanel(bool status)
    {
        multiplayerRoomPanel.SetActive(status);
    }

    // Temporary disable join button to prevent a bug from joining back too fast
    public void DisableJoinRoomButtonUponLeaving()
    {
        StartCoroutine(TemporaryDisableJoinButton());
    }

    IEnumerator TemporaryDisableJoinButton()
    {
        joinRoomButton.interactable = false;
        yield return new WaitForSecondsRealtime(2.0f);
        joinRoomButton.interactable = true;
    }

    public void OnClickShowTutorial()
    {
        tutorialPanel.SetActive(true);
    }

    public void OnClickHideTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    public void OnClickBackToMainMenu()
    {
        LevelManager.Instance.ExitToMainMenu();
    }


    public void SetIsPlayerBusy(bool isBusy)
    {
        isPlayerBusyInRoom = isBusy;
    }


    private void Update()
    {
        if (!isPlayerBusyInRoom && Input.GetKeyDown(KeyCode.Escape))
        {
            LevelManager.Instance.ExitToMainMenu();
        }
    }
}
