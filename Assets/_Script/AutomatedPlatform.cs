using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomatedPlatform : MonoBehaviour {

    [SerializeField] float dropSpeed = 5.0f;
    [SerializeField] float riseSpeed = 1.0f;
    [Tooltip("Only works with 2 points, make sure the 1st child is the destination and 2nd child is the resting pos.")]
    [SerializeField] GameObject designatedMoveLine;
    [SerializeField] GameObject dropTriggerCollider;

    [Header("Advanced Settings")]
    [Tooltip("Should the object forced to reach the destination even collider is null?")]
    [SerializeField] bool forcedReachDestination = false;
    [Tooltip("Should the object reach the initial point before initiate another trigger?")]
    [SerializeField] bool forcedReturnToInitial = false;
    [SerializeField] float triggerMoveDelay = 0f;
    [SerializeField] float returnInitialDelay = 1.0f;

    [Header("Optional: Sounds")]
    [SerializeField] AudioClip moveToDestinationSound;
    [SerializeField] AudioClip triggerSound;

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
        myAudioSource = GetComponent<AudioSource>();
        myAudioSource.volume = GameManager.GetSoundVolume();
    }

    void Update()
    {
        // Trigger object to go destination
        // The check for "isTriggeredToMove" is to make sure that it doesn't call the functions when its not needed (idle, moving back etc)
        if (!isTriggeredToMove && myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            if (!forcedReturnToInitial)
            {
                PlayAudioClip(moveToDestinationSound);             // stepping platform midway will fall AND play drop sound
                isTriggeredToMove = true;
            }
            else if (forcedReturnToInitial && transform.position == pointsList[INITIAL_POINT].transform.position)
            {
                if (!isObjectBusy)
                {
                    PlayAudioClip(triggerSound);
                    StartCoroutine(CooldownBeforeMoving(true, triggerMoveDelay, true));
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
                    StartCoroutine(CooldownBeforeMoving(false, returnInitialDelay, false));
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

    private void PlayAudioClip(AudioClip clipToPlay = null)
    {
        if (clipToPlay != null && myAudioSource.isPlaying == false)
        {
            myAudioSource.PlayOneShot(clipToPlay);
        }
    }

    IEnumerator CooldownBeforeMoving(bool state, float delay, bool playSound)
    {
        isObjectBusy = true;
        yield return new WaitForSeconds(delay);
        isTriggeredToMove = state;

        if (playSound)
        {
            PlayAudioClip(moveToDestinationSound);
        }

        isObjectBusy = false;
    }

    void Move(GameObject destination, float speed)
    {
        if (Time.deltaTime != 0)            // Time.timescale = 0 will cause error of output result being (NaN, NaN, NaN)
        {
            float journey = speed * Time.deltaTime;
            float totalDistance = Vector3.Distance(transform.position, destination.transform.position);
            journeyCovered = journey / totalDistance;

            transform.position = Vector3.Lerp(transform.position, destination.transform.position, journeyCovered);
        }
    }
}
