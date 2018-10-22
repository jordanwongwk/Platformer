using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerAudioManager : MonoBehaviour {

    [Header("Audio Clips")]
    [SerializeField] AudioClip playerEnterLeaveClip;
    [SerializeField] AudioClip readyToStartClip;

    AudioSource lobbySFXPlayer;

    // Use this for initialization
    void Start () {
        lobbySFXPlayer = GetComponent<AudioSource>();
        lobbySFXPlayer.volume = PlayerPrefsManager.GetSoundVolume();
	}

    public void PlayerEnterLeaveRoom()
    {
        lobbySFXPlayer.PlayOneShot(playerEnterLeaveClip);
    }

    public void ReadyToStartClip()
    {
        lobbySFXPlayer.PlayOneShot(readyToStartClip);
    }
}
