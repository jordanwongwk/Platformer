using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPowerUpObject : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Need to make sure that this is local because the opponent avatar will collide with it
            if (collision.gameObject.GetComponent<NetworkPlayer>().GetPlayerAuthority())
            {
                collision.gameObject.GetComponent<PowerUpScript>().RandomizedPower();
                gameObject.SetActive(false);        // Using Destroy(..) will cause client side's powerup to disable for some odd reasons.
            }
        }
    }
}
