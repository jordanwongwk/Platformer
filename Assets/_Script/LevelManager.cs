using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    [Header("Build Index Configurations")]
    [SerializeField] int mainMenuIndex;
    [SerializeField] int gameLoadingBuildIndex;

    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameLoadingBuildIndex);
        Time.timeScale = 1.0f;
    }

    public void QuitGame()
    {
        Debug.Log("Game is quitting.");
        Application.Quit();
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(mainMenuIndex);
        Time.timeScale = 1.0f;
    }

    #region For Stage Progression Mode
    public void RestartLevel()
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentLevelIndex);
    }

    public void ProceedNextLevel()
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentLevelIndex + 1);
    }
    #endregion
}
