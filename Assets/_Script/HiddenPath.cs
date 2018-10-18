using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenPath : MonoBehaviour {

    Color transparentWhite = Color.white;
    Color fullwhite = Color.white;

    private void Start()
    {
        transparentWhite.a = 0.5f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponent<Tilemap>().color = transparentWhite;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GetComponent<Tilemap>().color = fullwhite;
    }
}
