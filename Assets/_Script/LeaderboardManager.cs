using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager: MonoBehaviour {

    [SerializeField] List<GameObject> placingScore;
    [SerializeField] List<GameObject> placingDifficulty;
    [SerializeField] List<Image> placingHandicapNoIndicator;
    [SerializeField] List<Image> placingHandicapLimitedLives;
    [SerializeField] List<Image> placingHandicapOneLife;
    [SerializeField] List<Image> placingHandicapZeroDivine;

    Color bgColor;
    Color darkGrey = new Color(0.15f, 0.15f, 0.15f, 1.0f);

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

            // Handicap
            bool handicapNoIndicator = PlayerPrefsManager.GetHandicapNoIndicator(i + 1);
            bool handicapLimitedLives = PlayerPrefsManager.GetHandicapLimitedLives(i + 1);
            bool handicapOneLife = PlayerPrefsManager.GetHandicapOneLife(i + 1);
            bool handicapZeroDivine = PlayerPrefsManager.GetHandicapZeroDivine(i + 1);

            if (handicapNoIndicator) { placingHandicapNoIndicator[i].color = Color.white; }
            else if (!handicapNoIndicator) { placingHandicapNoIndicator[i].color = darkGrey; }

            if (handicapLimitedLives) { placingHandicapLimitedLives[i].color = Color.white; }
            else if (!handicapLimitedLives) { placingHandicapLimitedLives[i].color = darkGrey; }

            if (handicapOneLife) { placingHandicapOneLife[i].color = Color.white; }
            else if (!handicapOneLife) { placingHandicapOneLife[i].color = darkGrey; }

            if (handicapZeroDivine) { placingHandicapZeroDivine[i].color = Color.white; }
            else if (!handicapZeroDivine) { placingHandicapZeroDivine[i].color = darkGrey; }
        }
    }
}
