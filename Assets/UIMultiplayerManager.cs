using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMultiplayerManager : MonoBehaviour {

    [SerializeField] GameObject multiplayerRoomPanel;
    [SerializeField] Button joinRoomButton;

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
}
