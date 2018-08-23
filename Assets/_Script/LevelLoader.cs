using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

    [SerializeField] int gameSceneBuildIndex = 2;
    [SerializeField] Text loadingText;

    // Use this for initialization
    void Start()
    {
        loadingText.text = "Hang on. Your game is loading...";
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
                loadingText.text = "Get ready! Tap the screen to begin climbing!";

                if (Input.GetMouseButtonDown(0))
                {
                    async.allowSceneActivation = true;
                }
            }

            yield return null;
        }

    }
}
