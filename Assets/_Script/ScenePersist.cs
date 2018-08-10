using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePersist : MonoBehaviour {

    public static int startLevelIndex;
    public static ScenePersist instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

            if (currentLevelIndex == startLevelIndex)
            {
                Destroy(gameObject);
                return;
            }
            else if (currentLevelIndex != startLevelIndex)
            {
                Destroy(instance.gameObject);
                instance = this;
            }
        }

        startLevelIndex = SceneManager.GetActiveScene().buildIndex;
        DontDestroyOnLoad(gameObject);
    }
}
