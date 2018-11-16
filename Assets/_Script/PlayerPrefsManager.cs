using System.Collections;
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
    const string HANDICAP_NO_INDICATOR = "handicap_no_indicator";
    const string HANDICAP_LIMITED_LIVES = "Handicap_limited_life";
    const string HANDICAP_ONE_LIFE = "handicap_one_life";
    const string HANDICAP_ZERO_DIVINE = "handicap_zero_divine";

    const string MULTIPLAYER_TIME_LIMIT = "multiplayer_time_limit";

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

    // Handicaps
    public static void SetHandicapNoIndicator(int placing, bool status)
    {
        string handicapNoIndicatorKey = HANDICAP_NO_INDICATOR + placing.ToString();

        int statusInInt = 0;
        if (status) { statusInInt = 1; }

        PlayerPrefs.SetInt(handicapNoIndicatorKey, statusInInt);
    }

    public static bool GetHandicapNoIndicator(int placing)
    {
        string handicapNoIndicatorKey = HANDICAP_NO_INDICATOR + placing.ToString();
        int statusInInt = PlayerPrefs.GetInt(handicapNoIndicatorKey, 0);
        bool status = (statusInInt == 1);
        return status;
    }

    public static void SetHandicapLimitedLives(int placing, bool status)
    {
        string handicapLimitedLivesKey = HANDICAP_LIMITED_LIVES + placing.ToString();

        int statusInInt = 0;
        if (status) { statusInInt = 1; }

        PlayerPrefs.SetInt(handicapLimitedLivesKey, statusInInt);
    }

    public static bool GetHandicapLimitedLives(int placing)
    {
        string handicapLimitedLivesKey = HANDICAP_LIMITED_LIVES + placing.ToString();
        int statusInInt = PlayerPrefs.GetInt(handicapLimitedLivesKey, 0);
        bool status = (statusInInt == 1);
        return status;
    }

    public static void SetHandicapOneLife(int placing, bool status)
    {
        string handicapOneLife = HANDICAP_ONE_LIFE + placing.ToString();

        int statusInInt = 0;
        if (status) { statusInInt = 1; }

        PlayerPrefs.SetInt(handicapOneLife, statusInInt);
    }

    public static bool GetHandicapOneLife(int placing)
    {
        string handicapOneLife = HANDICAP_ONE_LIFE + placing.ToString();
        int statusInInt = PlayerPrefs.GetInt(handicapOneLife, 0);
        bool status = (statusInInt == 1);
        return status;
    }

    public static void SetHandicapZeroDivine(int placing, bool status)
    {
        string handicapZeroDivine = HANDICAP_ZERO_DIVINE + placing.ToString();

        int statusInInt = 0;
        if (status) { statusInInt = 1; }

        PlayerPrefs.SetInt(handicapZeroDivine, statusInInt);
    }

    public static bool GetHandicapZeroDivine(int placing)
    {
        string handicapZeroDivine = HANDICAP_ZERO_DIVINE + placing.ToString();
        int statusInInt = PlayerPrefs.GetInt(handicapZeroDivine, 0);
        bool status = (statusInInt == 1);
        return status;
    }
    #endregion

    #region Multiplayer
    public static void SetTimeLimitForMultiplayer(float timeLimit)
    {
        PlayerPrefs.SetFloat(MULTIPLAYER_TIME_LIMIT, timeLimit);
    }

    public static float GetTimeLimitForMultiplayer()
    {
        return PlayerPrefs.GetFloat(MULTIPLAYER_TIME_LIMIT);
    }
    #endregion
}
