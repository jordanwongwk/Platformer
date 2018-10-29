using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterMusicPlayer : MonoBehaviour {

    [SerializeField] AudioClip mainMenuClip;
    [SerializeField] AudioClip singlePlayerGameClip;
    [SerializeField] AudioClip multiplayerLobbyClip;
    [SerializeField] AudioClip multiplayerGameClip;

    int currentSceneIndex;
    AudioSource masterAudioSource;

    private void Start()
    {
        masterAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentSceneIndex != SceneManager.GetActiveScene().buildIndex)
        {
            int newScene = SceneManager.GetActiveScene().buildIndex;
            currentSceneIndex = newScene;
            masterAudioSource.Stop();

            if (newScene == 0) { masterAudioSource.clip = mainMenuClip; }
            else if (newScene == 2) { masterAudioSource.clip = singlePlayerGameClip; }
            else if (newScene == 3) { masterAudioSource.clip = multiplayerLobbyClip; }
            else if (newScene == 4) { masterAudioSource.clip = multiplayerGameClip; }
            else { masterAudioSource.clip = null; }

            masterAudioSource.Play();
        }
    }
}
