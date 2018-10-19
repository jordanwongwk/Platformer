using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LobbyUIManager : NetworkBehaviour
{
    [SerializeField] List<GameObject> readyPanelsPosition;

    public Vector2 GetPanelPosition(int i)
    {
        var panelImage = readyPanelsPosition[i - 1].GetComponent<Image>();
        return panelImage.rectTransform.anchoredPosition;
    }

    public void UpdatePanelActive(int numberOfPlayers)
    {
        if (numberOfPlayers == 1)
        {
            readyPanelsPosition[0].SetActive(false);
            readyPanelsPosition[1].SetActive(true);
        }
        else
        {
            readyPanelsPosition[0].SetActive(false);
            readyPanelsPosition[1].SetActive(false);
        }
    }
}
