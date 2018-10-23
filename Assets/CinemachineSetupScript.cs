using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineSetupScript : MonoBehaviour {
    CinemachineVirtualCamera playerCamera;

    // Use this for initialization
    void Start()
    {
        playerCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void SetFollowCameraTarget(GameObject target)
    {
        playerCamera.Follow = target.transform;
    }
}
