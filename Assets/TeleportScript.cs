using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportScript : MonoBehaviour {

    bool isTriggeredToMove = false;
    bool isInGround = false;
    Vector3 lastTouchPosition;
    NetworkPlayer thisPlayer;
    BoxCollider2D teleportCollider;

    void Start()
    {
        thisPlayer = GetComponentInParent<NetworkPlayer>();
        teleportCollider = GetComponent<BoxCollider2D>();
    }

    void Update ()
    {
        if (isTriggeredToMove)
        {
            float yChange = 2 * Time.deltaTime;
            transform.localPosition = new Vector3(0f, transform.localPosition.y + yChange, 0f);

            if (transform.localPosition.y >= 5.0f)
            {
                isTriggeredToMove = false;
                Debug.Log("Teleport Axis: " + lastTouchPosition);
                //thisPlayer.TeleportToLocation(lastTouchPosition);
            }

            if (teleportCollider.bounds.Contains(transform.position))
            {
                Debug.Log("Aye");
            }
        }
	}

    public void FindForSuitableLocation()
    {
        isTriggeredToMove = true;
        transform.localPosition = new Vector3(0f, 0f, 0f);
        lastTouchPosition = transform.position;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Looking");
        if (!isInGround)
        {
            Debug.Log("In ground");
            isInGround = true;
        }
        else
        {
            Debug.Log("Register spot: " + transform.position);
            lastTouchPosition = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
            isInGround = false;
        }         
    }
}
