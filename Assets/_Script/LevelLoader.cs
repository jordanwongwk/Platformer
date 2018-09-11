using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

    [SerializeField] int gameSceneBuildIndex = 2;
    [SerializeField] GameObject loadingPanel;

    bool isReady = false;
    AsyncOperation async;
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
        async = SceneManager.LoadSceneAsync(gameSceneBuildIndex);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            if (async.progress >= 0.9f)
            {
                loadingPanelText.text = "Your game is ready. Tap HERE to START.";
                loadingPanelImage.color = Color.white;
                isReady = true;
            }

            yield return null;
        }
    }

    public void OnClickStartGame()
    {
        if (isReady)
        {
            async.allowSceneActivation = true;
        }
    }
}
