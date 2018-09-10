using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsManager : MonoBehaviour {

    static int divineOrbCharges;
    static float waterRisingSpeed;
    static float waterInitialSpeed;
    static float scoreMultiplier;
    static string difficultyString;

    public static void SetDivineOrbCharges(int value)
    {
        divineOrbCharges = value;
    }

    public static int GetDivineOrbCharges()
    {
        return divineOrbCharges;
    }

    public static void SetWaterRisingSpeed(float setSpeed)
    {
        waterRisingSpeed = setSpeed;
    }

    public static float GetWaterRisingSpeed()
    {
        return waterRisingSpeed;
    }

    public static void SetWaterInitialSpeed(float setInitialSpeed)
    {
        waterInitialSpeed = setInitialSpeed;
    }

    public static float GetWaterInitialSpeed()
    {
        return waterInitialSpeed;
    }

    public static void SetScoreMultiplier(float multiplier)
    {
        scoreMultiplier = multiplier;
    }

    public static float GetScoreMultiplier()
    {
        return scoreMultiplier;
    }

    public static void SetDifficultyString(string difficulty)
    {
        difficultyString = difficulty;
    }

    public static string GetDifficultyString()
    {
        return difficultyString;
    }
}
