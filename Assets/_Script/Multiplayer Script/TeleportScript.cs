using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportScript : MonoBehaviour {

    [SerializeField] float teleportDistanceThreshold = 1.0f;
    [SerializeField] float teleportCheckerMovementSpeed = 4.0f;
    [SerializeField] AudioClip teleportOutAudio;

    bool isTriggeredToMove = false;
    Vector3 lastTouchPosition;
    NetworkPlayer thisPlayer;
    BoxCollider2D teleportCollider;
    AudioSource myAudioSource;

    void Start()
    {
        thisPlayer = GetComponentInParent<NetworkPlayer>();
        myAudioSource = GetComponent<AudioSource>();
        myAudioSource.volume = PlayerPrefsManager.GetSoundVolume();
    }

    void Update ()
    {
        if (isTriggeredToMove)
        {
            float yChange = teleportCheckerMovementSpeed * Time.deltaTime;
            transform.localPosition = new Vector3(0f, transform.localPosition.y + yChange, 0f);

            if (transform.localPosition.y >= 5.0f)
            {
                isTriggeredToMove = false;
                if (Mathf.Abs(lastTouchPosition.y - thisPlayer.transform.position.y) <= teleportDistanceThreshold)
                {
                    thisPlayer.UnableToTeleportRefund();                    
                }
                else
                {
                    myAudioSource.PlayOneShot(teleportOutAudio);
                    thisPlayer.TeleportToLocation(lastTouchPosition);
                }
            }
        }
	}

    // Start / Trigger Teleport Search
    public void StartFindingForTeleportLocation()
    {
        isTriggeredToMove = true;
        transform.localPosition = new Vector3(0f, 0f, 0f);
        lastTouchPosition = thisPlayer.transform.position;
    }

    public float GetTeleportOutAudioLength()
    {
        return teleportOutAudio.length;
    }

    // This only collides with a layer call "PlayerTeleportLayer"
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isTriggeredToMove)
        {
            lastTouchPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
    }
}
