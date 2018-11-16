using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MultiplayerGameSelection : NetworkBehaviour
{
    [SerializeField] GameObject hostTimeLimitPanel;
    [SerializeField] GameObject timeLimitInteractablePanel;
    [SerializeField] List<GameObject> timeLimitOptions;

    [SyncVar] bool isThisPlayerHost = false;
    [SyncVar] int selectedTimeLimitOptInt;

    const int OPTION_SHORT_TIME_LIMIT = 0;
    const int OPTION_LONG_TIME_LIMIT = 1;

    void Start ()
    {
        if (isServer && isLocalPlayer)
        {
            isThisPlayerHost = true;
            timeLimitInteractablePanel.SetActive(!isThisPlayerHost);
            SettingTimeLimitToPlayerPrefs(180f);            // Place initial timelimit to player prefs in case player dont change
        }
        else if (!isServer && !isLocalPlayer)
        {
            foreach (GameObject option in timeLimitOptions)
            {
               option.GetComponent<Toggle>().isOn = false;
            }

            timeLimitOptions[selectedTimeLimitOptInt].GetComponent<Toggle>().isOn = true;
        }

        // Time Limit Panel canvas (for host ONLY)
        hostTimeLimitPanel.SetActive(isThisPlayerHost);
    }

    public void OnClickShortTimeLimitSelected(bool isSelected)
    {
        if (isSelected)
        {
            RpcShortTimeLimitSelected(OPTION_SHORT_TIME_LIMIT);
            selectedTimeLimitOptInt = 0;
            Debug.Log("Short Game Selected");
            SettingTimeLimitToPlayerPrefs(180f);
        }
    }

    public void OnClickLongTimeLimitSelected(bool isSelected)
    {
        if (isSelected)
        {
            RpcShortTimeLimitSelected(OPTION_LONG_TIME_LIMIT);
            selectedTimeLimitOptInt = 1;
            Debug.Log("Long Game Selected");
            SettingTimeLimitToPlayerPrefs(300f);
        }
    }

    private void SettingTimeLimitToPlayerPrefs(float timeLimit)
    {
        PlayerPrefsManager.SetTimeLimitForMultiplayer(timeLimit);
    }

    [ClientRpc]
    void RpcShortTimeLimitSelected(int selectedOption)
    {
        timeLimitOptions[selectedOption].gameObject.GetComponent<Toggle>().isOn = true;
    }
}
