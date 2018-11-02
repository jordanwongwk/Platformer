using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkPowerUpUI : MonoBehaviour {

    // Always make sure [0] for powerUpImages, Indication, DurationImage is for NULL / White Image

    [Header("Power Up UI Setup")]
    [SerializeField] float popUpDisplayDuration = 5.0f;
    [SerializeField] List<Sprite> powerUpImages;
    [SerializeField] Image powerUpImageUI;
    [SerializeField] GameObject powerUpButtonUI;
    [SerializeField] GameObject powerUpReadyIndicator;

    [Header("Power Up Indication UI Setup")]
    [SerializeField] GameObject powerUpTextBox;
    [SerializeField] List<GameObject> powerUpIndication;
    [SerializeField] List<GameObject> powerUpDurationImage;

    [Header("Power Up Audio Clip")]
    [SerializeField] AudioClip powerUpPickedUp;
    [SerializeField] AudioClip powerUpUsedSuccessfully;
    [SerializeField] AudioClip powerUpNegated;
    [SerializeField] AudioClip powerUpFrozen;
    [SerializeField] AudioClip powerUpConfused;
    [SerializeField] AudioClip powerUpWeaken;
    [SerializeField] AudioClip powerUpBlind;
    [SerializeField] AudioClip powerUpSlippery;
    [SerializeField] AudioClip powerUpOrbitalBeamDanger;

    List<int> numberOfSimilarPowerActive = new List<int>();
    int thisPlayerID;
    bool isPowerUpTextBoxBeingDisplayed = false;
    PowerUps currentPowerUp;
    GameObject thisPlayerGameObject;
    PowerUpScript thisPlayerPowerUpScript;
    AudioSource powerUpAudioSource;
    Button powerUpButton;
    Text powerUpText;

    Coroutine powerUpTextBoxCoroutine;

    // List of Power-Ups
    const int NO_POWER_UP_NUMBER = 0;
    const int FREEZE_NUMBER = 1;
    const int CONFUSE_NUMBER = 2;
    const int SHIELD_NUMBER = 3;
    const int WEAKEN_NUMBER = 4;
    const int BLIND_NUMBER = 5;
    const int SLIPPERY_NUMBER = 6;
    const int ORBITAL_BEAM_NUMBER = 7;

    #region initialization
    // Use this for initialization
    void Start()
    {
        thisPlayerID = GetComponentInParent<PlayerConnectionObject>().GetThisPlayerID();
        powerUpImageUI = powerUpImageUI.GetComponent<Image>();
        powerUpButton = powerUpButtonUI.GetComponent<Button>();
        powerUpText = powerUpTextBox.GetComponentInChildren<Text>();
        powerUpAudioSource = GetComponent<AudioSource>();

        powerUpButton.interactable = false;
        powerUpAudioSource.volume = PlayerPrefsManager.GetSoundVolume();

        SettingUpListSizeForSimilarPowerActive();
        SearchForThisPlayerGameObject();
    }

    // Set numberOfSimilarPowerActive size count to same amount as number of power ups
    void SettingUpListSizeForSimilarPowerActive()
    {
        foreach (Sprite powerUp in powerUpImages)
        {
            numberOfSimilarPowerActive.Add(0);
        }
    }

    void SearchForThisPlayerGameObject()
    {
        var players = FindObjectsOfType<NetworkPlayer>();
        foreach (NetworkPlayer selectedPlayer in players)
        {
            if (selectedPlayer.GetPlayerID() == thisPlayerID)
            {
                thisPlayerGameObject = selectedPlayer.gameObject;
                LinkingPowerUpUIAndScript();
            }
        }

        if (thisPlayerGameObject == null)
        {
            StartCoroutine(SearchForPlayerGOAgain());
        }
    }

    IEnumerator SearchForPlayerGOAgain()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        SearchForThisPlayerGameObject();
    }

    void LinkingPowerUpUIAndScript()
    {
        thisPlayerPowerUpScript = thisPlayerGameObject.GetComponent<PowerUpScript>();
        thisPlayerGameObject.GetComponent<NetworkPlayer>().SetThisPlayerPowerUpUI(this);
    }
    #endregion

    public void SetUpPowerUpImageAndReadiness(PowerUps powerUpGranted, bool ready)
    {
        powerUpAudioSource.PlayOneShot(powerUpPickedUp);
        powerUpReadyIndicator.SetActive(true);
        powerUpButton.interactable = ready;
        currentPowerUp = powerUpGranted;

        switch (powerUpGranted)
        {
            case PowerUps.freeze:
                powerUpImageUI.sprite = powerUpImages[FREEZE_NUMBER];
                break;
            case PowerUps.confuse:
                powerUpImageUI.sprite = powerUpImages[CONFUSE_NUMBER];
                break;
            case PowerUps.shield:
                powerUpImageUI.sprite = powerUpImages[SHIELD_NUMBER];
                break;
            case PowerUps.weaken:
                powerUpImageUI.sprite = powerUpImages[WEAKEN_NUMBER];
                break;
            case PowerUps.blind:
                powerUpImageUI.sprite = powerUpImages[BLIND_NUMBER];
                break;
            case PowerUps.slippery:
                powerUpImageUI.sprite = powerUpImages[SLIPPERY_NUMBER];
                break;
            case PowerUps.orbitalBeam:
                powerUpImageUI.sprite = powerUpImages[ORBITAL_BEAM_NUMBER];
                break;
            default:
                powerUpImageUI.sprite = powerUpImages[NO_POWER_UP_NUMBER];
                Debug.LogError("No Image for this Power Up!");
                break;
        }
    }

    public void OnClickUsePowerUp()
    {
        powerUpButton.interactable = false;
        powerUpReadyIndicator.SetActive(false);
        powerUpImageUI.sprite = powerUpImages[NO_POWER_UP_NUMBER];
        thisPlayerPowerUpScript.ExecutePower(currentPowerUp);
    }

    // Indication Script
    public void TurnOnIndicationAndDurationImage(int powerUpNumber, float duration)
    {
        if (powerUpIndication[powerUpNumber].activeSelf == false)
        {
            powerUpIndication[powerUpNumber].SetActive(true);
        }
        else
        {
            numberOfSimilarPowerActive[powerUpNumber]++;
        }
        StartCoroutine(DurationImageMechanism(powerUpNumber, duration));
    }

    IEnumerator DurationImageMechanism(int powerUpNumber, float duration)
    {
        float timePassed = 0f;
        float localDuration = 0f;                       // To track only the local buff/debuff duration
        Image durationImage = powerUpDurationImage[powerUpNumber].GetComponent<Image>();
        durationImage.fillAmount = 0f;            // Initializing fill amount back to 0f

        while (localDuration < 1.0f)
        {
            timePassed += Time.deltaTime;
            durationImage.fillAmount = timePassed / duration;
            localDuration = durationImage.fillAmount;
            yield return null;
        }

        PowerUpDurationEnd(powerUpNumber);
    }

    public void PowerUpDurationEnd(int powerUpNumber)
    {
        if (numberOfSimilarPowerActive[powerUpNumber] <= 0)
        {
            powerUpIndication[powerUpNumber].SetActive(false);
        }
        else
        {
            numberOfSimilarPowerActive[powerUpNumber]--;
        }
    }

    // Power-Up Text Display
    // Target
    public void TargetPowerUpSuccesfullyBeenInflictedText(PowerUps usedPowerUp)
    {
        ManagingPowerUpTextBox();
        switch (usedPowerUp)
        {
            case PowerUps.freeze:
                powerUpText.text = "You felt a cold wind and ice starts to engulf and trapped you! \nYou are frozen solid momentarily!";
                powerUpAudioSource.PlayOneShot(powerUpFrozen);
                break;
            case PowerUps.confuse:
                powerUpText.text = "You felt dizzy. \nYou can move on but you lost your sense of direction!";
                powerUpAudioSource.PlayOneShot(powerUpConfused);
                break;
            case PowerUps.weaken:
                powerUpText.text = "You suddenly grow tired. \nYour movement capability has reduced.";
                powerUpAudioSource.PlayOneShot(powerUpWeaken);
                break;
            case PowerUps.blind:
                powerUpText.text = "You can't see well all the sudden. \nYour vision's range is reduced!";
                powerUpAudioSource.PlayOneShot(powerUpBlind);
                break;
            case PowerUps.slippery:
                powerUpText.text = "The floor seems slippery, it seems your feet is imbue with ice. \nBe careful as you walk!";
                powerUpAudioSource.PlayOneShot(powerUpSlippery);
                break;
            case PowerUps.orbitalBeam:
                powerUpText.text = "You sense the presence of high energy charging. \nAvoid the red laser NOW!";
                powerUpAudioSource.PlayOneShot(powerUpOrbitalBeamDanger);
                break;
            default:
                powerUpText.text = "Error: This power up does not have any text.";
                break;
        }
    }

    public void TargetPowerUpNegatedText()
    {
        ManagingPowerUpTextBox();
        powerUpAudioSource.PlayOneShot(powerUpNegated);
        powerUpText.text = "The shield absorbed the power your opponent casted on you and disappeared along with the power.";
    }

    // User
    public void UserPowerUpSuccessfulInflictionText(PowerUps usedPowerUp)
    {
        ManagingPowerUpTextBox();
        powerUpAudioSource.PlayOneShot(powerUpUsedSuccessfully);
        switch (usedPowerUp)
        {
            case PowerUps.freeze:
                powerUpText.text = "You used the power of ice! \nYour opponent is now frozen!";
                break;
            case PowerUps.confuse:
                powerUpText.text = "You used the power of deception! \nYour opponent has lost sense of direction!";
                break;
            case PowerUps.weaken:
                powerUpText.text = "You used the power of energy drain! \nYour opponent has suddenly lose their strength!";
                break;
            case PowerUps.blind:
                powerUpText.text = "You used the power of darkness! \nYour opponent is now blinded!";
                break;
            case PowerUps.slippery:
                powerUpText.text = "You used the power of ice! \nYour opponent's feet are imbue with ice, making them slippery!";
                break;
            case PowerUps.shield:
                powerUpText.text = "You've obtained the Guardian's blessing, granting you a shield that blocks any tricks your opponent try on you momentarily.";
                break;
            case PowerUps.orbitalBeam:
                powerUpText.text = "Orbital Beam is charging up. \nBeam is firing in 3 seconds.";
                break;
            default:
                powerUpText.text = "This Power Up does not require shield check!";
                break;
        }
    }

    public void UserPowerUpNegatedText()
    {
        ManagingPowerUpTextBox();
        powerUpAudioSource.PlayOneShot(powerUpNegated);
        powerUpText.text = "Your power has been absorbed by your opponent's shield. \nThe effect is negated!";
    }

    public void UserPowerUpThatCannotBeCastWhileActive(PowerUps blockedPowerUp)
    {
        ManagingPowerUpTextBox();
        powerUpAudioSource.PlayOneShot(powerUpNegated); // TODO cancel sound?
        powerUpText.text = "This power-up cannot be casted while it is active! Try again when it expires.";
        SetUpPowerUpImageAndReadiness(blockedPowerUp, true);
    }


    // General 
    void ManagingPowerUpTextBox()
    {
        if (isPowerUpTextBoxBeingDisplayed)
        {
            StopCoroutine(powerUpTextBoxCoroutine);
        }

        powerUpTextBoxCoroutine = StartCoroutine(PowerUpTextBoxDisplay());
    }

    IEnumerator PowerUpTextBoxDisplay()
    {
        powerUpTextBox.SetActive(true);
        isPowerUpTextBoxBeingDisplayed = true;
        yield return new WaitForSecondsRealtime(popUpDisplayDuration);
        powerUpTextBox.SetActive(false);
        isPowerUpTextBoxBeingDisplayed = false;
    }
}
