using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanel : MonoBehaviour {

    [SerializeField] GameObject retryConfirmationWindow;
    [SerializeField] GameObject quitConfirmationWindow;

    public void OpenUpRetryConfirmation()
    {
        retryConfirmationWindow.SetActive(true);
    }

    public void CloseRetryConfirmation()
    {
        retryConfirmationWindow.SetActive(false);
    }

    public void OpenUpConfirmation()
    {
        quitConfirmationWindow.SetActive(true);
    }

    public void CloseConfirmation()
    {
        quitConfirmationWindow.SetActive(false);
    }
}
