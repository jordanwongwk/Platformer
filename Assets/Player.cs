using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour {

    [SerializeField] float walkSpeed = 5.0f;
    [SerializeField] float jumpSpeed = 5.0f;

    Rigidbody2D myRigidBody;
    BoxCollider2D myFeetCollider;
    CapsuleCollider2D myBodyCollider;
    Animator myAnimator;

    float initialGravityScale;

    // Use this for initialization
    void Start () {
        myRigidBody = GetComponent<Rigidbody2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myAnimator = GetComponent<Animator>();

        initialGravityScale = myRigidBody.gravityScale;
	}
	
	// Update is called once per frame
	void Update () {
        Walking();
        Jumping();
        Climbing();
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
            myScale = new Vector3(Mathf.Sign(horizontalMove), myScale.y, myScale.z);
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
            Vector2 jumpVelocity = new Vector2(0f, jumpSpeed);
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
}
