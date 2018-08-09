using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScroll : MonoBehaviour {

    [SerializeField] float speed = 1f;
    [SerializeField] float xPosToReset;
    Vector3 startPos;

	// Use this for initialization
	void Start () {
        startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate((new Vector3(-1f, 0, 0)) * speed * Time.deltaTime);

        if (transform.position.x < xPosToReset)
        {
            transform.position = startPos;
        }
	}
}
