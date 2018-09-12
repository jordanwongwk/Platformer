using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsManager : MonoBehaviour {

    static int divineOrbCharges;
    static float waterRisingSpeed;
    static float waterInitialSpeed;
    static float scoreMultiplier;
    static string difficultyString;

    static bool handicapNoIndicator;
    static bool handicapLimitedLives;
    static bool handicapOneLife;
    static bool handicapZeroDivine;


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



    public static void SetHandicapNoIndicator(bool status)
    {
        handicapNoIndicator = status;
    }

    public static bool GetHandicapNoIndicator()
    {
        return handicapNoIndicator;
    }

    public static void SetHandicapLife(bool limitedLife, bool oneLife)
    {
        handicapLimitedLives = limitedLife;
        handicapOneLife = oneLife;
    }

    public static bool GetHandicapLimitedLives()
    {
        return handicapLimitedLives;
    }

    public static bool GetHandicapOneLife()
    {
        return handicapOneLife;
    }

    public static void SetHandicapZeroDivine(bool status)
    {
        handicapZeroDivine = status;
    }

    public static bool GetHandicapZeroDivine()
    {
        return handicapZeroDivine;
    }
}
