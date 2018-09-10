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
    bool isWaterFrozen = false;
    Player player;
    GameManager gameManager;
    Coroutine stopWaterCoroutine;
    AudioSource waterAudioSource;

	// Use this for initialization
	void Start () {
        player = FindObjectOfType<Player>();
        gameManager = FindObjectOfType<GameManager>();
        waterAudioSource = GetComponent<AudioSource>();
        waterAudioSource.volume = GameManager.GetSoundVolume();

        if (FindObjectOfType<GameSettingsManager>() != null)
        {
            risingSpeed = GameSettingsManager.GetWaterInitialSpeed();
        }
        else
        {
            // Call when play from level straight from editor!
            Debug.Log("No GameSettingsManager script exists. Setting to default diffculty.");
            risingSpeed = 1.0f;         // Set Default Speed
        }
    }

    // Update is called once per frame
    void Update () {
        transform.position += new Vector3(0f, risingSpeed * Time.deltaTime, 0f);
        float distanceDiff = player.transform.position.y - transform.position.y;
        gameManager.WaterLevelUpdate(distanceDiff);
	}

    public void RisingWaterSpeed(float addedSpeed)
    {
        waterAudioSource.PlayOneShot(risingWaterSFX);
        risingSpeed += addedSpeed;
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
}
