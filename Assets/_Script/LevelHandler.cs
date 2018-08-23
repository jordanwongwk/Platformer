using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : MonoBehaviour {

    [SerializeField] LevelManager myLevelManager;       // Serialize just to check

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

    public void BackToMainMenu()
    {
        myLevelManager.ExitToMainMenu();
    }

}
