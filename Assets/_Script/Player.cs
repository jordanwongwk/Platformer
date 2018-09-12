using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour {

    [SerializeField] float walkSpeed = 5.0f;
    [SerializeField] float jumpSpeed = 5.0f;
    [SerializeField] float deathLaunch = 10.0f;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip deathSound;

    Rigidbody2D myRigidBody;
    BoxCollider2D myFeetCollider;
    CapsuleCollider2D myBodyCollider;
    Animator myAnimator;
    AudioSource myAudioSource;

    Vector3 respawnPoint;

    int limitedLifeValue;
    int hazardHitCounts = 0;
    float initialGravityScale;
    float initialSpriteScale;
    bool isAlive = true;
    bool isMortal = false;
    bool deathByDrowning;

    const float DEATH_DELAY = 3.0f;
    const int LIMITED_LIFE_AMOUNT = 5;

    void Start ()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myAnimator = GetComponent<Animator>();
        myAudioSource = GetComponent<AudioSource>();

        initialGravityScale = myRigidBody.gravityScale;
        initialSpriteScale = transform.localScale.x;
        myAudioSource.volume = GameManager.GetSoundVolume();

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
	
	void Update ()
    {
        if (isAlive)
        {
            Walking();
            Jumping();
            Climbing();
            CheckForPlayerDeath();
        }
	}

    void Walking()
    {
        float horizontalMove = CrossPlatformInputManager.GetAxis("Horizontal");
        Vector2 playerMove = new Vector2( horizontalMove * walkSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerMove;

        if (Mathf.Abs(horizontalMove) > Mathf.Epsilon)
        {
            myAnimator.SetBool("isWalking", true);
            Vector3 myScale = transform.localScale;
            myScale = new Vector3(Mathf.Sign(horizontalMove) * initialSpriteScale, myScale.y, myScale.z);
            transform.localScale = myScale;
        }
        else
        {
            myAnimator.SetBool("isWalking", false);
        }
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

        float verticalMove = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 playerMove = new Vector2(myRigidBody.velocity.x, verticalMove * walkSpeed);
        myRigidBody.velocity = playerMove;

        if (Mathf.Abs(verticalMove) > Mathf.Epsilon)
        {
            myAnimator.SetBool("isClimbing", true);
        }
    }

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
        } 
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        transform.parent = null;
    }
    // End attaching to moving platform

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
}
