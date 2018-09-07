using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : MonoBehaviour {

    [SerializeField] LevelManager myLevelManager;       // Serialize just to check

    [SerializeField] bool viewedAds = false;            // TODO Set False on Publish

	// Use this for initialization
	void Start () {
        myLevelManager = LevelManager.Instance;
	}

    public void StartGameHandler()
    {
        myLevelManager.StartGame();
    }

    public void QuitGameHandler()
    {
        myLevelManager.QuitGame();
    }

    public void RetryGame()
    {
        myLevelManager.StartGame();
    }

    public void BackToMainMenu()
    {
        if (!viewedAds)
        {
            viewedAds = true;
            FindObjectOfType<AdScript>().showInterstitialAd();
        }
        else
        {
            myLevelManager.ExitToMainMenu();
        }
    }

    public void BackToMainMenuWithPause()
    {
        FindObjectOfType<AdScript>().showInterstitialAd();
        myLevelManager.ExitToMainMenu();
    }
}
