using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

    [SerializeField] int gameSceneBuildIndex = 2;
    [SerializeField] GameObject loadingPanel;

    Image loadingPanelImage;
    Text loadingPanelText;

    // Use this for initialization
    void Start()
    {
        loadingPanelImage = loadingPanel.GetComponent<Image>();
        loadingPanelText = loadingPanel.GetComponentInChildren<Text>();

        loadingPanelText.text = "Hang on. Your game is loading...";
        loadingPanelImage.color = Color.grey;
        StartCoroutine(LoadingGame());
    }

    IEnumerator LoadingGame()
    {
        yield return null;
        AsyncOperation async = SceneManager.LoadSceneAsync(gameSceneBuildIndex);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            if (async.progress >= 0.9f)
            {
                loadingPanelText.text = "Get ready! Tap the screen to begin climbing!";
                loadingPanelImage.color = Color.white;

                if (Input.GetMouseButtonDown(0))
                {
                    async.allowSceneActivation = true;
                }
            }

            yield return null;
        }

    }
}
