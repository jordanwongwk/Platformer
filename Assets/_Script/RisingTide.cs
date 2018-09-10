using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingTide : MonoBehaviour {

    [Header("Water Settings")]
    [SerializeField] float risingSpeed = 1.0f;

    [Header("Audio Settings")]
    [SerializeField] AudioClip risingWaterSFX;
    [SerializeField] AudioClip frozenWaterSFX;

    float lastWaterSpeed;
    float initialWaterSpeed;
    bool isWaterFrozen = false;
    bool isWaterGoingDown = false;
    float movingDownFinalPos;
    float distanceDiff;
    Player player;
    GameManager gameManager;
    Coroutine stopWaterCoroutine;
    AudioSource waterAudioSource;

	void Start ()
    {
        player = FindObjectOfType<Player>();
        gameManager = FindObjectOfType<GameManager>();
        waterAudioSource = GetComponent<AudioSource>();
        waterAudioSource.volume = GameManager.GetSoundVolume();

        if (FindObjectOfType<GameSettingsManager>() != null)
        {
            initialWaterSpeed = GameSettingsManager.GetWaterInitialSpeed();
        }
        else
        {
            // Call when play from level straight from editor!
            Debug.Log("No GameSettingsManager script exists. Setting to default diffculty.");
            initialWaterSpeed = 1.0f;         // Set Default Speed
        }

        risingSpeed = initialWaterSpeed;
    }

    void Update ()
    {
        if (isWaterGoingDown)
        {
            // Water going down
            transform.position -= new Vector3(0f, 25f * Time.deltaTime, 0f);

            if (transform.position.y <= movingDownFinalPos)
            {
                isWaterGoingDown = false;
            }
        }
        else
        {
            // Water going Up
            transform.position += new Vector3(0f, risingSpeed * Time.deltaTime, 0f);
        }
        distanceDiff = player.transform.position.y - transform.position.y;
        gameManager.WaterLevelUpdate(distanceDiff);
    }

    public void RisingWaterSpeed(float addedSpeed)
    {
        waterAudioSource.PlayOneShot(risingWaterSFX);
        risingSpeed += addedSpeed;
    }

    public float GetCurrentWaterSpeed()
    {
        return risingSpeed;
    }

    public float GetInitialWaterSpeed()
    {
        return initialWaterSpeed;
    }

    public float GetDistanceDifference()
    {
        return distanceDiff;
    }

    #region Power Up : Freeze Water
    public void FreezeWater(float freezeTime)
    {
        waterAudioSource.PlayOneShot(frozenWaterSFX);
        if (!isWaterFrozen)
        {
            isWaterFrozen = true;
            lastWaterSpeed = risingSpeed;
            gameManager.SetWaterFrozenUI(true);
            stopWaterCoroutine = StartCoroutine(StopWater(freezeTime));
        }
        else 
        {
            StopCoroutine(stopWaterCoroutine);
            stopWaterCoroutine = StartCoroutine(StopWater(freezeTime));
        }
    }

    IEnumerator StopWater(float stopTime)
    {
        risingSpeed = 0f;
        yield return new WaitForSeconds(stopTime);
        risingSpeed = lastWaterSpeed;
        gameManager.SetWaterFrozenUI(false);
        isWaterFrozen = false;
    }

    public bool GetWaterFrozenStatus()
    {
        return isWaterFrozen;
    }
    #endregion

    public void ReduceWaterPosition(float amountReduced)
    {
        movingDownFinalPos = transform.position.y - amountReduced;
        isWaterGoingDown = true;
    }
}
