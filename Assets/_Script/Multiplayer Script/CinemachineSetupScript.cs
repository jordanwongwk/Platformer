using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineSetupScript : MonoBehaviour {
    float cameraScreenXOffset = 0.5f;
    CinemachineVirtualCamera playerCamera;

    // Use this for initialization
    void Start()
    {
        playerCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void SetFollowCameraTarget(GameObject target, int id)
    {
        playerCamera.Follow = target.transform;

        if (id == 1) { cameraScreenXOffset = 0.4f; }
        else if (id == 2) { cameraScreenXOffset = 0.6f; }

        playerCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = cameraScreenXOffset;
    }
}
