using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour {

    const string MUSIC_VOLUME = "music_volume";
    const string SOUND_VOLUME = "sound_volume";
    const string RISING_WATER_ADDITIONAL_SPEED = "rising_water_additional_speed";
    const string SCORE_MULTIPLIER = "score_multiplier";
    const string HIGH_SCORE = "high_score";

    #region Option Settings
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
    #endregion

    #region Difficulty Settings
    public static void SetRisingWaterAdditionalSpeed(float speed)
    {
        PlayerPrefs.SetFloat(RISING_WATER_ADDITIONAL_SPEED, speed);
    }

    public static float GetRisingWaterAdditionalSpeed()
    {
        return PlayerPrefs.GetFloat(RISING_WATER_ADDITIONAL_SPEED);
    }

    public static void SetScoreMultiplier(float multiplier)
    {
        PlayerPrefs.SetFloat(SCORE_MULTIPLIER, multiplier);
    }

    public static float GetScoreMultiplier()
    {
        return PlayerPrefs.GetFloat(SCORE_MULTIPLIER);
    }
    #endregion

    #region Scores and Leaderboards
    public static void SetHighScore(int placing, int score)
    {
        string highScoreKey = HIGH_SCORE + placing.ToString();
        PlayerPrefs.SetInt(highScoreKey, score);
    }

    public static int GetHighScore(int placing)
    {
        string highScoreKey = HIGH_SCORE + placing.ToString();
        return PlayerPrefs.GetInt(highScoreKey);
    }
    #endregion
}
