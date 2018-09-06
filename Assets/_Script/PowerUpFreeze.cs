using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpFreeze : MonoBehaviour {

    [SerializeField] float waterFrozenTime = 10.0f;
    [SerializeField] float moveThreshold = 0.25f;
    [SerializeField] float speed = 0.1f;

    float direction = 1f;
    bool _isTriggered = false;
    Vector3 initialPosition;

	void Start () {
        initialPosition = transform.position;	
	}

	void Update ()
    {
        FloatingMovement(initialPosition.y - moveThreshold, initialPosition.y + moveThreshold);
	}

    void FloatingMovement (float minY, float maxY)
    {
        if (transform.position.y <= minY) { direction = 1.0f; }
        else if (transform.position.y >= maxY) { direction = -1.0f; }

        transform.position += new Vector3(0f, direction * Time.deltaTime * speed, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isTriggered)
        {
            _isTriggered = true;
            FindObjectOfType<RisingTide>().FreezeWater(waterFrozenTime);
            Destroy(gameObject);
        }
    }
}
