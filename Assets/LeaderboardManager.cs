using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager: MonoBehaviour {

    [SerializeField] List<GameObject> placingText;

    public void GetLeaderboardScore()
    {
        for (int i = 0; i < placingText.Count; i++)
        {
            placingText[i].GetComponent<Text>().text = PlayerPrefsManager.GetHighScore(i + 1).ToString();
        }
    }
}
