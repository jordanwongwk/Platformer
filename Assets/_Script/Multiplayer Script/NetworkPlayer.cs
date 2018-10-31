using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

public class NetworkPlayer : NetworkBehaviour {

    [Header ("Player Stat")]
    [SerializeField] float walkSpeed = 5.0f;
    [SerializeField] float jumpSpeed = 5.0f;
    [SerializeField] float deathLaunch = 10.0f;

    [Header("Player Setup")]
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] GameObject playerIndicator;
    [SerializeField] GameObject confusionEffect;
    [SerializeField] GameObject shieldEffect;
    [SerializeField] GameObject weakenEffect;
    [SerializeField] GameObject blindEffect;
    [SerializeField] GameObject slipperyEffect;

    [Header("Opponent Config")]
    [SerializeField] float opponentVisibility = 0.5f;

    // Player Configuration Variables
    Rigidbody2D myRigidBody;
    BoxCollider2D myFeetCollider;
    CapsuleCollider2D myBodyCollider;
    Animator myAnimator;
    AudioSource myAudioSource;

    Vector3 respawnPoint;

    int limitedLifeValue;
    int hazardHitCounts = 0;
    float horizontalMove;
    float verticalMove;
    float initialGravityScale;
    float initialSpriteScale;
    bool isAlive = true;
    bool isMortal = false;
    bool deathByDrowning;

    // Network Configurations
    int playerID;
    int slipperyPreventionWallMask;         // Mask for Slippery Power Up (Prevent slipping over wall / ladder)
    float initialWalkSpeed;
    float initialJumpSpeed;
    public float slidingDistX;             // For Slippery

    NetworkPowerUpUI myPowerUpUI;
    PowerUpScript myPowerUpScript;
    [SyncVar] Color indicatorColor;
    [SyncVar] bool isThisPlayerReady = false;
    [SyncVar] bool isFrozen = false;
    [SyncVar] bool isConfused = false;
    [SyncVar] bool isShielded = false;
    [SyncVar] bool isWeaken = false;
    [SyncVar] bool isBlinded = false;
    [SyncVar] public bool isSlippery = false;

    Coroutine frozenCoroutine;
    Coroutine confusedCoroutine;
    Coroutine shieldedCoroutine;
    Coroutine weakenCoroutine;
    Coroutine blindedCoroutine;
    Coroutine slipperyCoroutine;

    //TEST
    public GameObject testTrack;

    // Constants
    const float DEATH_DELAY = 3.0f;
    const int LIMITED_LIFE_AMOUNT = 5;

    #region Initialization
    private void Awake()
    {
        if (GameSettingsManager.GetHandicapLimitedLives())
        {
            isMortal = true;
            limitedLifeValue = LIMITED_LIFE_AMOUNT;
        }
        else if (GameSettingsManager.GetHandicapOneLife())
        {
            isMortal = true;
            limitedLifeValue = 1;
        }
    }

    void Start ()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myAnimator = GetComponent<Animator>();
        myAudioSource = GetComponent<AudioSource>();
        myPowerUpScript = GetComponent<PowerUpScript>();

        initialGravityScale = myRigidBody.gravityScale;
        initialSpriteScale = transform.localScale.x;
        initialWalkSpeed = walkSpeed;
        initialJumpSpeed = jumpSpeed;

        SettingUpPlayerIndicator();
        SettingLayerMaskForSliding();

        if (!hasAuthority)
        {
            myAudioSource.volume = 0f;
            return;
        }

        CmdSetPlayerColor();
        myAudioSource.volume = GameManager.GetSoundVolume();
	}

    // Keep looping this method until SyncVar is ready then proceed to set color
    private void SettingUpPlayerIndicator()
    {
        Debug.Log("Calling Set Up");
        if (isThisPlayerReady)
        {
            SetPlayerIndicatorColor(indicatorColor);
        }
        else
        {
            StartCoroutine(AttemptToExecuteSettingUpAgain());
        }
    }

    IEnumerator AttemptToExecuteSettingUpAgain()
    {
        yield return new WaitForEndOfFrame();       // Call ASAP
        SettingUpPlayerIndicator();
    }

    private void SetPlayerIndicatorColor(Color playerColor)
    {
        if (!hasAuthority)
        {
            Color mySpriteColor = GetComponent<SpriteRenderer>().color;
            mySpriteColor.a = opponentVisibility;
            playerColor.a = opponentVisibility;
            GetComponent<SpriteRenderer>().color = mySpriteColor;
        }

        playerIndicator.SetActive(true);
        playerIndicator.GetComponent<SpriteRenderer>().color = playerColor;
    }

    // TODO check if this set-up interrupt other set up
    // To set up certain layer objects prevent the momentum of slipping to maintain (Act as forced stop for slipping)
    void SettingLayerMaskForSliding()
    {
        int foreground = 9;
        int climbing = 12;

        int foregroundMask = 1 << foreground;
        int climbingMask = 1 << climbing;

        slipperyPreventionWallMask = foregroundMask | climbingMask;
    }
    #endregion

    #region Getter and Setter
    // General Player Get/Set
    public int GetHazardHits()
    {
        return hazardHitCounts;
    }

    public int GetLifeCount()
    {
        return limitedLifeValue;
    }

    public bool GetIsPlayerAlive()
    {
        return isAlive;
    }

    // Network Player Get/Set
    public void SetThisPlayerPowerUpUI(NetworkPowerUpUI thisPowerUpUI)
    {
        myPowerUpUI = thisPowerUpUI;
    }

    public NetworkPowerUpUI GetThisPlayerPowerUpUI()
    {
        return myPowerUpUI;
    }

    public void SetPlayerID(int id)
    {
        playerID = id;
    }

    public int GetPlayerID()
    {
        return playerID;
    }

    public bool GetPlayerAuthority()
    {
        return hasAuthority;
    }

    public bool GetPlayerIsShielded()
    {
        return isShielded;
    }
    #endregion

    void Update ()
    {
        if (!hasAuthority) { return; }

        if (isAlive && !isFrozen)
        {
            Walking();
            Jumping();
            Climbing();
            CheckForPlayerDeath();
            Sliding();
        }
	}

    #region Update And Controls
    void Walking()
    {
        GettingPlayerHorizontalInput();
        
        Vector2 playerMove = new Vector2( horizontalMove * walkSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerMove;

        if (Mathf.Abs(horizontalMove) > Mathf.Epsilon)
        {
            myAnimator.SetBool("isWalking", true);
            Vector3 myScale = transform.localScale;
            myScale = new Vector3(Mathf.Sign(horizontalMove) * initialSpriteScale, myScale.y, myScale.z);
            transform.localScale = myScale;

            float direction = Mathf.Sign(horizontalMove);
            CmdUpdateWalkAnimation(true, direction);
        }
        else
        {
            myAnimator.SetBool("isWalking", false);
            float direction = Mathf.Sign(horizontalMove);         
            CmdUpdateWalkAnimation(false, direction);
        }
    }

    void GettingPlayerHorizontalInput()
    {
        if (!isConfused) { horizontalMove = CrossPlatformInputManager.GetAxis("Horizontal"); }
        else if (isConfused) { horizontalMove = -CrossPlatformInputManager.GetAxis("Horizontal"); }
    }

    void Jumping()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Foreground"))) { return; }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            myAnimator.SetTrigger("isJumping");
            Vector2 jumpVelocity = new Vector2(0f, jumpSpeed);
            myAudioSource.PlayOneShot(jumpSound);
            myRigidBody.velocity += jumpVelocity;
            CmdJumpAnimationTrigger();
        }
    }

    void Climbing()
    {
        if (!myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myAnimator.SetBool("isOnLadder", false);
            myAnimator.SetBool("isClimbing", false);
            myRigidBody.gravityScale = initialGravityScale;
            return;
        }
        else if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myAnimator.SetBool("isOnLadder", true);
            myRigidBody.gravityScale = 0f;
        }

        GettingPlayerVerticalMovement();

        Vector2 playerMove = new Vector2(myRigidBody.velocity.x, verticalMove * walkSpeed);
        myRigidBody.velocity = playerMove;

        if (Mathf.Abs(verticalMove) > Mathf.Epsilon)
        {
            myAnimator.SetBool("isClimbing", true);
        }
    }

    void GettingPlayerVerticalMovement()
    {
        if (!isConfused) { verticalMove = CrossPlatformInputManager.GetAxis("Vertical"); }
        else if (isConfused) { verticalMove = -CrossPlatformInputManager.GetAxis("Vertical"); }
    }

    void Sliding()
    {
        if (isSlippery)
        {
            float slipperyMultiplier = myPowerUpScript.GetSlipperyMultiplier();

            // Raycast check to see if player has collided with wall / ladder
            float raycastLength = 0.35f;
            float rayDirection = Mathf.Sign(slidingDistX - transform.position.x);
            Vector2 raycastDirection = new Vector2(rayDirection, 0f);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, raycastDirection, raycastLength, slipperyPreventionWallMask);

            if (hit.collider != null)
            {
                slidingDistX = transform.position.x;
            }

            if (Mathf.Abs(horizontalMove) > Mathf.Epsilon)
            {
                // Sliding will not occur when player is climbing. 
                // NOTE: When climbing, the edge of the platform still trigger this. Take note!
                if (!myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
                {
                    float direction = Mathf.Sign(horizontalMove);
                    slidingDistX += (Time.deltaTime * slipperyMultiplier * walkSpeed * direction);
                }
            }
            else
            {
                if (Mathf.Abs(transform.position.x - slidingDistX) > Mathf.Epsilon)
                {
                    float lerpingX = Mathf.Lerp(transform.position.x, slidingDistX, Time.deltaTime);
                    Vector3 slidingVector = new Vector3(lerpingX, transform.position.y, transform.position.z);
                    transform.position = slidingVector;
                }
            }
            testTrack.transform.position = new Vector3(slidingDistX, transform.position.y, transform.position.z);
        }
        else
        {
            SetSlidingDistanceXToInitial();
        }
    }

    private void SetSlidingDistanceXToInitial()
    {
        slidingDistX = transform.position.x;
    }


    #endregion

    #region Death Check
    void CheckForPlayerDeath()
    {
        if (myRigidBody.IsTouchingLayers(LayerMask.GetMask("Hazard")))
        {
            if (!isMortal)
            {
                StartCoroutine(ProcessPlayerDeath());
                Debug.Log("Immortal");
            }
            else 
            {
                limitedLifeValue--;
                FindObjectOfType<GameManager>().UpdateLifeCount();

                if (limitedLifeValue <= 0)
                {
                    hazardHitCounts++;
                    deathByDrowning = false;
                    StartCoroutine(ProcessPlayerPermenantDeath());
                }
                else
                {
                    StartCoroutine(ProcessPlayerDeath());
                }
            }
        }
        else if (myRigidBody.IsTouchingLayers(LayerMask.GetMask("RisingTide")))
        {
            deathByDrowning = true;
            StartCoroutine(ProcessPlayerPermenantDeath());
        }
    }

    IEnumerator ProcessPlayerDeath()
    {
        PlayerDeathSequence();
        hazardHitCounts++;
        yield return new WaitForSeconds(DEATH_DELAY);      
        myAnimator.SetTrigger("isRespawned");
        transform.position = respawnPoint;
        StartCoroutine(ChangeIsAliveStatus());

        // Reset Slippery Point
        SetSlidingDistanceXToInitial();
    }

    // To prevent death on 2nd time when respawning
    IEnumerator ChangeIsAliveStatus()
    {
        yield return new WaitForSeconds(0.5f);
        isAlive = true;                             
    }

    IEnumerator ProcessPlayerPermenantDeath()
    {
        PlayerDeathSequence();
        FindObjectOfType<GameManager>().GameOverPanelUpdate(deathByDrowning);
        yield return new WaitForSeconds(DEATH_DELAY);
        Time.timeScale = 0f;
        FindObjectOfType<GameManager>().OpenGameOverPanel();
    }

    private void PlayerDeathSequence()
    {
        isAlive = false;
        myAudioSource.PlayOneShot(deathSound);
        myRigidBody.velocity = new Vector2(0f, deathLaunch);
        myAnimator.SetTrigger("isDying");
        myRigidBody.gravityScale = initialGravityScale;
    }
    #endregion

    // Register the current obstacle the player is on
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAlive && collision.gameObject.tag == "ObstacleForeground")
        {
            GameObject currentObstacle = collision.gameObject;
            respawnPoint = currentObstacle.transform.Find("RespawnPoint").position;
        }
    }

    // For attaching to moving platform!
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            transform.parent = collision.transform;
            SetSlidingDistanceXToInitial();
        } 
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        transform.parent = null;
    }
    // End attaching to moving platform

    #region Power-Ups Region
    // 1 - Freeze
    public void FreezePlayer(float duration)
    {
        Debug.Log("Freeze");
        // If is not under shield effect
        if (!isShielded)
        {
            if (hasAuthority)
            {
                myPowerUpUI.TurnOnIndicationAndDurationImage(1, duration);
                myPowerUpUI.TargetPowerUpSuccesfullyBeenInflictedText(PowerUps.freeze);
            }

            if (!isFrozen)
            {
                isFrozen = true;
                myRigidBody.gravityScale = 0f;
                myAnimator.SetBool("isFrozen", true);
                CmdCallFreezeParameters(0f, true);
                frozenCoroutine = StartCoroutine(FreezeDuration(duration));
            }
            else if (isFrozen)
            {
                StopCoroutine(frozenCoroutine);
                frozenCoroutine = StartCoroutine(FreezeDuration(duration));
            }
        }
        else if (isShielded)
        {
            if (hasAuthority) { myPowerUpUI.TargetPowerUpNegatedText(); }

            DebuffNegatedByShield();
        }
    }

    IEnumerator FreezeDuration(float debuffDuration)
    {
        yield return new WaitForSecondsRealtime(debuffDuration);
        EndsFreezeDebuff();
    }

    private void EndsFreezeDebuff()
    {
        isFrozen = false;
        myRigidBody.gravityScale = 1.0f;
        myAnimator.SetBool("isFrozen", false);
        CmdCallFreezeParameters(1.0f, false);
    }
    // 1 END - Freeze 

    // 2 - Confusion
    public void ConfusePlayer(float duration)
    {
        Debug.Log("Confuse");
        if (!isShielded)
        {
            if (hasAuthority)
            {
                myPowerUpUI.TurnOnIndicationAndDurationImage(2, duration);
                myPowerUpUI.TargetPowerUpSuccesfullyBeenInflictedText(PowerUps.confuse);
            }

            if (!isConfused)
            {
                isConfused = true;
                confusionEffect.SetActive(isConfused);
                CmdCallConfusionEffect(isConfused);
                confusedCoroutine = StartCoroutine(ConfuseDuration(duration));
            }
            else if (isConfused)
            {
                StopCoroutine(confusedCoroutine);
                confusedCoroutine = StartCoroutine(ConfuseDuration(duration));
            }
        }
        else if (isShielded)
        {
            if (hasAuthority) { myPowerUpUI.TargetPowerUpNegatedText(); }

            DebuffNegatedByShield();
        }
    }

    IEnumerator ConfuseDuration(float debuffDuration)
    {
        yield return new WaitForSecondsRealtime(debuffDuration);
        EndsConfuseDebuff();
    }

    private void EndsConfuseDebuff()
    {
        isConfused = false;
        confusionEffect.SetActive(isConfused);
        CmdCallConfusionEffect(isConfused);
    }
    // 2 END - Confuse

    // 3 - Shield
    // Setting Up Shield
    public void ShieldPlayer(float duration)
    {
        if (!hasAuthority)
        {
            return;
        }
        else if (hasAuthority)
        {
            myPowerUpUI.TurnOnIndicationAndDurationImage(3, duration);
            myPowerUpUI.TargetPowerUpSuccesfullyBeenInflictedText(PowerUps.shield);
        }

        if (!isShielded)
        {
            isShielded = true;
            shieldEffect.SetActive(isShielded);
            CmdCallPlayerIsShielded(isShielded);
            shieldedCoroutine = StartCoroutine(ShieldDuration(duration));
        }
        else
        {
            StopCoroutine(shieldedCoroutine);
            shieldedCoroutine = StartCoroutine(ShieldDuration(duration));
        }
    }

    IEnumerator ShieldDuration(float buffDuration)
    {
        yield return new WaitForSecondsRealtime(buffDuration);
        EndsShieldBuff();
    }

    void EndsShieldBuff()
    {
        isShielded = false;
        CmdCallPlayerIsShielded(isShielded);
    }

    // Negate Debuff
    private void DebuffNegatedByShield()
    {
        Debug.Log("No effect");
        if (shieldedCoroutine != null)
        {
            StopCoroutine(shieldedCoroutine);
        }

        if (hasAuthority)
        {
            myPowerUpUI.PowerUpDurationEnd(3);
        }

        EndsShieldBuff();
    }
    // 3 END - Shield

    // 4 - Weaken
    public void WeakenPlayer(float duration)
    {
        Debug.Log("Weaken");
        // If is not under shield effect
        if (!isShielded)
        {
            if (hasAuthority)
            {
                myPowerUpUI.TurnOnIndicationAndDurationImage(4, duration);
                myPowerUpUI.TargetPowerUpSuccesfullyBeenInflictedText(PowerUps.weaken);
            }

            if (!isWeaken)
            {
                isWeaken = true;
                WeakenActivationEffect(isWeaken);
                weakenEffect.SetActive(isWeaken);
                CmdCallWeakenEffect(isWeaken);
                weakenCoroutine = StartCoroutine(WeakenDuration(duration));
            }
            else if (isWeaken)
            {
                StopCoroutine(weakenCoroutine);
                weakenCoroutine = StartCoroutine(WeakenDuration(duration));
            }
        }
        else if (isShielded)
        {
            if (hasAuthority) { myPowerUpUI.TargetPowerUpNegatedText(); }

            DebuffNegatedByShield();
        }
    }

    IEnumerator WeakenDuration(float debuffDuration)
    {
        yield return new WaitForSecondsRealtime(debuffDuration);
        EndWeakenDebuff();
    }

    private void EndWeakenDebuff()
    {
        isWeaken = false;
        WeakenActivationEffect(isWeaken);
        weakenEffect.SetActive(isWeaken);
        CmdCallWeakenEffect(isWeaken);
    }

    void WeakenActivationEffect(bool weakenStatus)
    {
        if (weakenStatus)
        {
            walkSpeed = initialWalkSpeed / myPowerUpScript.GetWeakenWalkSpeedReductionMultiplier();
            jumpSpeed = initialJumpSpeed / myPowerUpScript.GetWeakenJumpSpeedReductionMultiplier();
        }
        else
        {
            walkSpeed = initialWalkSpeed;
            jumpSpeed = initialJumpSpeed;
        }
    }
    // 4 END - Weaken

    // 5 - Blind
    public void BlindPlayer(float duration)
    {
        if (!hasAuthority) { return; }

        Debug.Log("Blind");
        if (!isShielded)
        {
            if (hasAuthority)
            {
                myPowerUpUI.TurnOnIndicationAndDurationImage(5, duration);
                myPowerUpUI.TargetPowerUpSuccesfullyBeenInflictedText(PowerUps.blind);
            }

            if (!isBlinded)
            {
                isBlinded = true;
                blindEffect.SetActive(true);
                blindedCoroutine = StartCoroutine(BlindDuration(duration));
            }
            else if (isBlinded)
            {
                StopCoroutine(blindedCoroutine);
                blindedCoroutine = StartCoroutine(BlindDuration(duration));
            }
        }
        else if (isShielded)
        {
            if (hasAuthority) { myPowerUpUI.TargetPowerUpNegatedText(); }

            DebuffNegatedByShield();
        }
    }

    IEnumerator BlindDuration(float debuffDuration)
    {
        yield return new WaitForSecondsRealtime(debuffDuration);
        EndsBlindDebuff();
    }

    private void EndsBlindDebuff()
    {
        isBlinded = false;
        blindEffect.SetActive(isBlinded);
    }
    // 5 END - Blind


    // 6 - Slippery
    public void SlipperyPlayer(float duration)
    {
        Debug.Log("Slippery");
        if (!isShielded)
        {
            if (hasAuthority)
            {
                myPowerUpUI.TurnOnIndicationAndDurationImage(6, duration);
                myPowerUpUI.TargetPowerUpSuccesfullyBeenInflictedText(PowerUps.slippery);
            }

            if (!isSlippery)
            {
                isSlippery = true;
                slipperyEffect.SetActive(isSlippery);
                CmdCallSlipperyEffect(isSlippery);
                slipperyCoroutine = StartCoroutine(SlipperyDuration(duration));
            }
            else if (isSlippery)
            {
                StopCoroutine(slipperyCoroutine);
                slipperyCoroutine = StartCoroutine(SlipperyDuration(duration));
            }
        }
        else if (isShielded)
        {
            if (hasAuthority) { myPowerUpUI.TargetPowerUpNegatedText(); }

            DebuffNegatedByShield();
        }
    }

    IEnumerator SlipperyDuration(float debuffDuration)
    {
        yield return new WaitForSecondsRealtime(debuffDuration);
        EndsSlipperyDebuff();
    }

    private void EndsSlipperyDebuff()
    {
        isSlippery = false;
        slipperyEffect.SetActive(isSlippery);
        CmdCallSlipperyEffect(isSlippery);
    }
    // 6 END - Slippery
    #endregion

    #region Command
    // Setup Command : Call to set color and boolean to SyncVar
    [Command]
    void CmdSetPlayerColor()
    {
        isThisPlayerReady = true;
        if (playerID == 1) { indicatorColor = Color.blue; }
        else { indicatorColor = Color.red; }
    }


    // Power Ups Commands
    [Command]
    void CmdCallFreezeParameters(float gravityScale, bool animationBool)
    {
        myRigidBody.gravityScale = gravityScale;
        RpcCallFreezeParameters(gravityScale, animationBool);
    }

    [Command]
    void CmdCallConfusionEffect(bool effectBool)
    {
        RpcCallConfusionEffect(effectBool);
    }

    [Command]
    void CmdCallPlayerIsShielded(bool isPlayerShielded)
    {
        isShielded = isPlayerShielded;
        RpcCallPlayerIsShielded(isPlayerShielded);
    }

    [Command]
    void CmdCallWeakenEffect(bool effectBool)
    {
        RpcCallWeakenEffect(effectBool);
    }

    [Command]
    void CmdCallSlipperyEffect(bool effectBool)
    {
        RpcCallSlipperyEffect(effectBool);
    }


    // Animations related Commands
    [Command]
    void CmdJumpAnimationTrigger()
    {
        RpcJumpAnimationTrigger();
    }

    [Command]
    void CmdUpdateWalkAnimation(bool playerRunning, float runningDirection)
    {
        RpcUpdateWalkAnimation(playerRunning, runningDirection);
    }

    #endregion

    #region RPC
    // Power Ups RPC
    [ClientRpc]
    void RpcCallFreezeParameters(float gravityScale, bool animationBool)
    {
        myRigidBody.gravityScale = gravityScale;
        myAnimator.SetBool("isFrozen", animationBool);
    }

    [ClientRpc]
    void RpcCallConfusionEffect(bool effectBool)
    {
        confusionEffect.SetActive(effectBool);
    }

    [ClientRpc]
    void RpcCallPlayerIsShielded(bool effectBool)
    {
        shieldEffect.SetActive(effectBool);
    }

    [ClientRpc]
    void RpcCallWeakenEffect(bool effectBool)
    {
        weakenEffect.SetActive(effectBool);
    }

    [ClientRpc]
    void RpcCallSlipperyEffect(bool effectBool)
    {
        slipperyEffect.SetActive(effectBool);
        // TODO Forgot something here...
    }


    // Animation RPC
    [ClientRpc]
    void RpcJumpAnimationTrigger()
    {
        myAnimator.SetTrigger("isJumping");
    }

    [ClientRpc]
    void RpcUpdateWalkAnimation(bool playerRunning, float runningDirection)
    {
        if (isThisPlayerReady)
        {
            myAnimator.SetBool("isWalking", playerRunning);
            if (playerRunning)
            {
                Vector3 myScale = transform.localScale;
                transform.localScale = new Vector3(runningDirection * initialSpriteScale, myScale.y, myScale.z);
            }
        }
    }
    #endregion

}
