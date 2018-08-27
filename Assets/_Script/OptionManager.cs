using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    [SerializeField] Slider musicPlayerVolume;
    [SerializeField] Slider soundPlayerVolume;

    AudioSource musicPlayer;
    AudioSource soundPlayer;

    void Start() {
        musicPlayer = GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<AudioSource>();
        soundPlayer = GetComponent<AudioSource>();

        DefaultOptions();
    }

    // Update is called once per frame
    void Update() {
        if (GetComponent<UIManager>().OptionPanelStatus())
        {
            musicPlayer.volume = musicPlayerVolume.value;
            soundPlayer.volume = soundPlayerVolume.value;
        }
    }

    // Sound Volume being dragged
    public void OnSoundBeingDragged()
    {
        soundPlayer.Play();
    }

    public void OnSoundStopDragged()
    {
        soundPlayer.Stop();
    }
    // End Sound Volume being dragged

    public float GetMusicVolumeChange()
    {
        return musicPlayer.volume;
    }

    public float GetSoundVolumeChange()
    {
        return soundPlayer.volume;
    }

    public void DefaultOptions()
    {
        musicPlayer.volume = PlayerPrefsManager.GetMusicVolume();
        soundPlayer.volume = PlayerPrefsManager.GetSoundVolume();

        musicPlayerVolume.value = PlayerPrefsManager.GetMusicVolume();
        soundPlayerVolume.value = PlayerPrefsManager.GetSoundVolume();
    }
}
