using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : MonoBehaviour {

    [SerializeField] LevelManager myLevelManager;       // Serialize just to check

	// Use this for initialization
	void Start () {
        myLevelManager = LevelManager.Instance;
	}

    public void MultiplayerLobbyHandler()
    {
        myLevelManager.StartMultiplayerLobby();
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

    public void BackToMainMenuDirectly()
    {
        FindObjectOfType<AdScript>().showInterstitialAd();
        StartCoroutine(ReturnToMainMenu());
    }

    IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        myLevelManager.ExitToMainMenu();
    }
}
