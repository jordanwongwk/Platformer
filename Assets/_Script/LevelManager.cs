using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    [Header("Build Index Configurations")]
    [SerializeField] int mainMenuIndex;
    [SerializeField] int gameLoadingBuildIndex;

    [Header("Panels Configurations")]
    [SerializeField] GameObject leaderboardPanel;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameObject creditsPanel;

    public void StartGame()
    {
        SceneManager.LoadScene(gameLoadingBuildIndex);
    }
}
