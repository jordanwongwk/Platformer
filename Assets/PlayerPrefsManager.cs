using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour {

    const string MUSIC_VOLUME = "music_volume";
    const string SOUND_VOLUME = "sound_volume";

    public static void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME, volume);
    }

    public static float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(MUSIC_VOLUME);
    }

    public static void SetSoundVolume(float volume)
    {
        PlayerPrefs.SetFloat(SOUND_VOLUME, volume);
    }

    public static float GetSoundVolume()
    {
        return PlayerPrefs.GetFloat(SOUND_VOLUME);
    }
}
