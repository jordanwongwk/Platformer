using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shredder : MonoBehaviour {

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ObstacleForeground" || collision.gameObject.tag == "ObstacleBackground")
        {
            Destroy(collision.gameObject);
        }
    }
}
