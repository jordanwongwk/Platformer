using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePickup : MonoBehaviour {

    bool hasClaim = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasClaim)
        {
            FindObjectOfType<GameManager>().LifeUpdate(1);
            //TODO Play SFX
            hasClaim = true;
            Destroy(gameObject);
        }
    }
}
