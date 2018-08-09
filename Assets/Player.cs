using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour {

    [SerializeField] float walkSpeed = 5.0f;
    [SerializeField] float jumpSpeed = 5.0f;

    Rigidbody2D myRigidBody;
    BoxCollider2D myFeetCollider;
    Animator myAnimator;

    // Use this for initialization
    void Start () {
        myRigidBody = GetComponent<Rigidbody2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        myAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        Walk();
        Jump();
	}

    void Walk()
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

    void Jump()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Foreground"))) { return; }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocity = new Vector2(0f, jumpSpeed);
            myRigidBody.velocity += jumpVelocity;
        }
    }
}
