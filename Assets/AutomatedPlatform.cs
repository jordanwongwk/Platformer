using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomatedPlatform : MonoBehaviour {

    [SerializeField] float dropSpeed = 5.0f;
    [SerializeField] float riseSpeed = 1.0f;
    [Tooltip("Only works with 2 points, make sure the 1st child is at lower pos and 2nd child is at higher pos.")]
    [SerializeField] GameObject designatedMoveLine;
    [SerializeField] GameObject dropTriggerCollider;

    [Header("Advanced Settings")]
    [Tooltip("Should the object forced to reach the bottom even collider is null?")]
    [SerializeField] bool forcedHitBottom = false;
    [Tooltip("Should the object reach the top first before initiate another fall?")]
    [SerializeField] bool forcedReachTop = false;

    bool isMovingDown = false;
    float journeyCovered;
    BoxCollider2D myBoxCollider;

    List<GameObject> pointsList = new List<GameObject>();

    const int LOWER_POINT = 0;
    const int HIGHER_POINT = 1;

    void Start ()
    {
	    pointsList = designatedMoveLine.GetComponent<PlatformLine>().GetPlatformPointsList();
        myBoxCollider = dropTriggerCollider.GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        // Check if the platform should go up or down
        if (myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            if (!forcedReachTop)
            {
                isMovingDown = true;
            }
            else if (forcedReachTop && transform.position == pointsList[HIGHER_POINT].transform.position)
            {
                isMovingDown = true;
            }
        }
        else
        {
            if (!forcedHitBottom)
            {
                isMovingDown = false;
            }
            else if (forcedHitBottom && transform.position == pointsList[LOWER_POINT].transform.position)
            {
                StartCoroutine(CooldownBeforeMoving(false));
            }
        }

        // Automated movement
        if (isMovingDown)
        {
            Move(pointsList[LOWER_POINT], dropSpeed);
        }
        else 
        {
            Move(pointsList[HIGHER_POINT], riseSpeed);
        }
    }

    IEnumerator CooldownBeforeMoving(bool state)
    {
        yield return new WaitForSeconds(1.0f);          // TODO need variable?
        isMovingDown = state;
    }

    void Move(GameObject destination, float speed)
    {
        float journey = speed * Time.deltaTime;
        float totalDistance = Vector3.Distance(transform.position, destination.transform.position);
        journeyCovered = journey / totalDistance;

        transform.position = Vector3.Lerp(transform.position, destination.transform.position, journeyCovered);
    }
}
