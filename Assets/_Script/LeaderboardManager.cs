using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager: MonoBehaviour {

    [SerializeField] List<GameObject> placingScore;
    [SerializeField] List<GameObject> placingDifficulty;

    Color bgColor;

    public void GetLeaderboardScore()
    {
        for (int i = 0; i < placingScore.Count; i++)
        {
            // Score
            placingScore[i].GetComponent<Text>().text = PlayerPrefsManager.GetHighScore(i + 1).ToString();

            // Difficulty
            string difficultyString = PlayerPrefsManager.GetHighScoreDifficulty(i + 1);
            placingDifficulty[i].GetComponentInChildren<Text>().text = difficultyString;

            if (difficultyString == "Easy") { bgColor = Color.green; }
            else if (difficultyString == "Normal") { bgColor = Color.yellow; }
            else if (difficultyString == "Hard") { bgColor = Color.red; }
            else { bgColor = Color.grey; }

            placingDifficulty[i].GetComponent<Image>().color = bgColor;
        }
    }


}
