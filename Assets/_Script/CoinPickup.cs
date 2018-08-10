using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour {

    [SerializeField] int coinScore = 100;

    bool hasClaim = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasClaim)
        {
            FindObjectOfType<GameManager>().ScoreUpdate(coinScore);
            //TODO Play SFX
            hasClaim = true;
            Destroy(gameObject);
        }
    }
}
