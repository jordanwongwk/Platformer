using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomatedPlatform : MonoBehaviour {

    [SerializeField] float dropSpeed = 5.0f;
    [SerializeField] float riseSpeed = 1.0f;
    [Tooltip("Only works with 2 points, make sure the 1st child is the destination and 2nd child is the resting pos.")]
    [SerializeField] GameObject designatedMoveLine;
    [SerializeField] GameObject dropTriggerCollider;
    [SerializeField] AudioClip triggerSound;

    [Header("Advanced Settings")]
    [Tooltip("Should the object forced to reach the destination even collider is null?")]
    [SerializeField] bool forcedReachDestination = false;
    [Tooltip("Should the object reach the initial point before initiate another trigger?")]
    [SerializeField] bool forcedReturnToInitial = false;

    bool isTriggeredToMove = false;
    bool isObjectBusy = false;          // use to control the number of times coroutine is being called, once is enough for performance
    float journeyCovered;
    BoxCollider2D myBoxCollider;
    AudioSource myAudioSource;

    List<GameObject> pointsList = new List<GameObject>();

    const int DESTINATION_POINT = 0;
    const int INITIAL_POINT = 1;

    void Start ()
    {
	    pointsList = designatedMoveLine.GetComponent<PlatformLine>().GetPlatformPointsList();
        myBoxCollider = dropTriggerCollider.GetComponent<BoxCollider2D>();

        if (triggerSound != null)
        {
            myAudioSource = GetComponent<AudioSource>();
            myAudioSource.volume = PlayerPrefsManager.GetSoundVolume();
        }
    }

    void Update()
    {
        // Trigger object to go destination
        // The check for "isTriggeredToMove" is to make sure that it doesn't call the functions when its not needed (idle, moving back etc)
        if (!isTriggeredToMove && myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            if (!forcedReturnToInitial)
            {
                TriggerAudioClip();             // Consider Removing this?
                isTriggeredToMove = true;
            }
            else if (forcedReturnToInitial && transform.position == pointsList[INITIAL_POINT].transform.position)
            {
                if (!isObjectBusy)
                {
                    TriggerAudioClip();
                    StartCoroutine(CooldownBeforeMoving(true, 0f));
                }
            }
        }
        // Object only move back when player moves away from trigger
        else if (isTriggeredToMove && !myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            if (!forcedReachDestination)
            {
                isTriggeredToMove = false;
            }
            else if (forcedReachDestination && transform.position == pointsList[DESTINATION_POINT].transform.position)
            {
                if (!isObjectBusy)
                { 
                    StartCoroutine(CooldownBeforeMoving(false, 1.0f));
                }
            }
        }
        
        // Automated movement
        if (isTriggeredToMove)
        {
            Move(pointsList[DESTINATION_POINT], dropSpeed);
        }
        else 
        {
            Move(pointsList[INITIAL_POINT], riseSpeed);
        }
    }

    private void TriggerAudioClip()
    {
        if (triggerSound != null && myAudioSource.isPlaying == false)
        {
            myAudioSource.PlayOneShot(triggerSound);
        }
    }

    IEnumerator CooldownBeforeMoving(bool state, float delay)
    {
        isObjectBusy = true;
        yield return new WaitForSeconds(delay);
        isTriggeredToMove = state;
        isObjectBusy = false;
    }

    void Move(GameObject destination, float speed)
    {
        float journey = speed * Time.deltaTime;
        float totalDistance = Vector3.Distance(transform.position, destination.transform.position);
        journeyCovered = journey / totalDistance;

        transform.position = Vector3.Lerp(transform.position, destination.transform.position, journeyCovered);
    }
}
