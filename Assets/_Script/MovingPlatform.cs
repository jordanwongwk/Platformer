using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    public float speed = 5.0f;
    [SerializeField] Movement movementType;
    [SerializeField] Vector2 movementMin;
    [SerializeField] Vector2 movementMax;

    public enum Movement { Horizontal, Vertical, Both }
	
	// Update is called once per frame
	void Update () {
        switch (movementType)
        {
            case Movement.Horizontal:
                if (transform.position.x < movementMin.x || transform.position.x >= movementMax.x)
                {
                    speed = -speed;
                }
                transform.position = new Vector2(transform.position.x + speed * Time.deltaTime, transform.position.y);
                break;

            case Movement.Vertical:
                if (transform.position.y < movementMin.y || transform.position.y >= movementMax.y)
                {
                    speed = -speed;
                }
                transform.position = new Vector2(transform.position.x, transform.position.y + speed * Time.deltaTime);
                break;

            default:
                Debug.Log("No log");
                break;
        }
	}
}
