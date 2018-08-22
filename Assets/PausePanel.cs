using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanel : MonoBehaviour {

    [SerializeField] GameObject quitConfirmationWindow;

    public void OpenUpConfirmation()
    {
        quitConfirmationWindow.SetActive(true);
    }

    public void CloseConfirmation()
    {
        quitConfirmationWindow.SetActive(false);
    }
}
