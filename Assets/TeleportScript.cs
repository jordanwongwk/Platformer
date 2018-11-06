using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportScript : MonoBehaviour {

    [SerializeField] float teleportDistanceThreshold = 1.0f;
    [SerializeField] float teleportCheckerMovementSpeed = 4.0f;

    public bool toggleStartTeleport = false;
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
        FindForSuitableLocation(); // TEST

        if (isTriggeredToMove)
        {
            float yChange = teleportCheckerMovementSpeed * Time.deltaTime;
            transform.localPosition = new Vector3(0f, transform.localPosition.y + yChange, 0f);

            if (transform.localPosition.y >= 5.0f)
            {
                isTriggeredToMove = false;
                if (Mathf.Abs(lastTouchPosition.y - thisPlayer.transform.position.y) <= teleportDistanceThreshold)
                {
                    Debug.Log("Unable to teleport");
                    // Refund PowerUp
                }
                else
                {
                    Debug.Log("Teleport Axis: " + lastTouchPosition);
                    thisPlayer.TeleportToLocation(lastTouchPosition);
                }
                // Send instruction to enable player movement (maybe set a bool false)
                // thisPlayer.SetIsTeleport(false);
            }
        }
	}

    public void FindForSuitableLocation()
    {
        if (toggleStartTeleport)
        {
            isTriggeredToMove = true;
            transform.localPosition = new Vector3(0f, 0f, 0f);
            lastTouchPosition = thisPlayer.transform.position;
            // Send instruction to Player to Stop Moving (maybe set a bool true)
            // thisPlayer.SetIsTeleport(true);

            toggleStartTeleport = false; // For Toggle
        }
    }

    // This only collides with a layer call "PlayerTeleportLayer"
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isTriggeredToMove)
        {
            Debug.Log("Log spot " + transform.position);
            lastTouchPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
    }
}
