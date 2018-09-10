﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour {

    const string MUSIC_VOLUME = "music_volume";
    const string SOUND_VOLUME = "sound_volume";
    const string RISING_WATER_ADDITIONAL_SPEED = "rising_water_additional_speed";
    const string RISING_WATER_INITIAL_SPEED = "rising_water_initial_speed";
    const string SCORE_MULTIPLIER = "score_multiplier";
    const string DIFFICULTY_STRING = "difficulty_string";
    const string HIGH_SCORE = "high_score";
    const string HIGH_SCORE_DIFF = "high_score_difficulty";

    #region Option Settings
    public static void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME, volume);
    }

    public static float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(MUSIC_VOLUME, 5.0f);
    }

    public static void SetSoundVolume(float volume)
    {
        PlayerPrefs.SetFloat(SOUND_VOLUME, volume);
    }

    public static float GetSoundVolume()
    {
        return PlayerPrefs.GetFloat(SOUND_VOLUME, 5.0f);
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

    public static void SetRisingWaterInitialSpeed(float initialSpeed)
    {
        PlayerPrefs.SetFloat(RISING_WATER_INITIAL_SPEED, initialSpeed);
    }

    public static float GetRisingWaterInitialSpeed()
    {
        return PlayerPrefs.GetFloat(RISING_WATER_INITIAL_SPEED);
    }

    public static void SetScoreMultiplier(float multiplier)
    {
        PlayerPrefs.SetFloat(SCORE_MULTIPLIER, multiplier);
    }

    public static float GetScoreMultiplier()
    {
        return PlayerPrefs.GetFloat(SCORE_MULTIPLIER);
    }

    public static void SetDifficultyString(string difficulty)
    {
        PlayerPrefs.SetString(DIFFICULTY_STRING, difficulty);
    }

    public static string GetDifficultyString()
    {
        return PlayerPrefs.GetString(DIFFICULTY_STRING);
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
        return PlayerPrefs.GetInt(highScoreKey, 0);
    }

    public static void SetHighScoreDifficulty(int placing, string difficulty)
    {
        string highScoreDiffKey = HIGH_SCORE_DIFF + placing.ToString();
        PlayerPrefs.SetString(highScoreDiffKey, difficulty);
    }

    public static string GetHighScoreDifficulty(int placing)
    {
        string highScoreDiffKey = HIGH_SCORE_DIFF + placing.ToString();
        return PlayerPrefs.GetString(highScoreDiffKey, "N/A");
    }
    #endregion
}
