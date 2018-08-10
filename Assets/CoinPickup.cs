using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Coins pickup!");
        Destroy(gameObject);
    }
}
