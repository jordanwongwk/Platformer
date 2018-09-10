using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpDivineOrb : MonoBehaviour {

    [Header("Presentation Setup")]
    [SerializeField] Image cooldownFill;
    [SerializeField] Text chargesLeftText;
    [SerializeField] Text warningText;
    [SerializeField] GameObject playerBuffingObject;
    [SerializeField] AudioClip divineOrbBuffSound;

    [Header("Divine Orb Settings")]
    [SerializeField] float cooldownTime = 5.0f;

    int charges;
    bool isOnCooldown = false;
    float cooldownTimeLeft;
    Coroutine TextActiveCoroutine;
    Animator buffingAnimator;
    AudioSource buffingAudioSource;
    RisingTide myRisingTide;

    const float FULL_FILL_AMOUNT = 1.0f;
    const float WAIT_TIME = 3.0f;

    void Start()
    {
        if (FindObjectOfType<GameSettingsManager>() != null)
        {
            charges = GameSettingsManager.GetDivineOrbCharges();
        }
        else
        {
            charges = 1;        
        }
        chargesLeftText.text = charges.ToString();

        buffingAnimator = playerBuffingObject.GetComponent<Animator>();
        buffingAudioSource = playerBuffingObject.GetComponent<AudioSource>();
        myRisingTide = FindObjectOfType<RisingTide>();
    }

    void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimeLeft -= Time.deltaTime;
            cooldownFill.fillAmount = cooldownTimeLeft / cooldownTime;

            if (cooldownTimeLeft <= 0)
            {
                isOnCooldown = false;
            }
        }
    }

    public void OnPressTrigger()
    {
        bool isPlayerAlive = FindObjectOfType<Player>().GetIsPlayerAlive();
        if (isPlayerAlive && !isOnCooldown)
        {
            if (charges > 0)
            {
                UseDivineOrb();
                StartCooldownMechanism();
            }
            else
            {
                ShowWarningText("You do not have any divine orb left!");
            }
        }
        else
        {
            ShowWarningText("Skill is cooling down!");
        }
    }

    private void UseDivineOrb()
    {
        buffingAudioSource.PlayOneShot(divineOrbBuffSound);
        buffingAnimator.SetTrigger("DivineOrbTrigger");

        // Water Speed Reduction : only occurs when its above the initial speed
        float currentWaterSpeed = myRisingTide.GetCurrentWaterSpeed();
        float initialWaterSpeed = myRisingTide.GetInitialWaterSpeed();
        if (currentWaterSpeed > initialWaterSpeed)
        {
            ShowWarningText("The orb glows, it seems that the water speed has reduced!");
            myRisingTide.RisingWaterSpeed(-1.0f);
        }
        else
        {
            ShowWarningText("The orb glows dimly but the water speed seems to be unaffected...");
        }

        // Set Water Drop
        float distanceDifference = myRisingTide.GetDistanceDifference();
        if (distanceDifference <= 25.0f)
        {
            myRisingTide.ReduceWaterPosition(100f);
        }
        else if (distanceDifference > 25.0f && distanceDifference <= 50.0f)
        {
            myRisingTide.ReduceWaterPosition(50f);
        }
        else
        {
            myRisingTide.ReduceWaterPosition(25f);
        }        
    }

    private void StartCooldownMechanism()
    {
        cooldownTimeLeft = cooldownTime;
        isOnCooldown = true;

        cooldownFill.fillAmount = FULL_FILL_AMOUNT;
        charges--;
        chargesLeftText.text = charges.ToString();
    }

    void ShowWarningText(string text)
    {
        warningText.text = text;

        if (TextActiveCoroutine != null)
        {
            StopCoroutine(TextActiveCoroutine);
        }

        TextActiveCoroutine = StartCoroutine(TextActiveTime());
    }

    IEnumerator TextActiveTime()
    {
        warningText.gameObject.SetActive(true);
        yield return new WaitForSeconds(WAIT_TIME);
        warningText.gameObject.SetActive(false);
    }
}
