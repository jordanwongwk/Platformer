using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMultiplayerManager : MonoBehaviour {

    [SerializeField] GameObject multiplayerRoomPanel;
    [SerializeField] GameObject tutorialListPanel;
    [SerializeField] GameObject networkTutorialPanel;
    [SerializeField] GameObject gameTutorialPanel;
    [SerializeField] Button joinRoomButton;

    List<GameObject> ActiveTutorialPanels = new List<GameObject>();
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

    public void OnClickShowTutorialList()
    {
        tutorialListPanel.SetActive(true);
        ActiveTutorialPanels.Add(tutorialListPanel);
    }

    public void OnClickShowNetworkTutorial()
    {
        networkTutorialPanel.SetActive(true);
        ActiveTutorialPanels.Add(networkTutorialPanel);
    }

    public void OnClickShowGameTutorial()
    {
        gameTutorialPanel.SetActive(true);
        ActiveTutorialPanels.Add(gameTutorialPanel);
    }

    public void OnClickCloseTutorial()
    {
        int currentlyActiveTutorialInt = ActiveTutorialPanels.Count - 1;
        GameObject currentlyActiveTutorial = ActiveTutorialPanels[currentlyActiveTutorialInt];

        currentlyActiveTutorial.SetActive(false);
        ActiveTutorialPanels.Remove(currentlyActiveTutorial);
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
