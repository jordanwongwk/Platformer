using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour {

    [SerializeField] float cloudMoveSpeed;
	
	// Update is called once per frame
	void Update () {
        transform.position += new Vector3(cloudMoveSpeed * Time.deltaTime, 0f, 0f);
	}
}
