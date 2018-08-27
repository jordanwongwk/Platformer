using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingTide : MonoBehaviour {

    [SerializeField] float risingSpeed = 1.0f;

    Player player;
    GameManager gameManager;

	// Use this for initialization
	void Start () {
        player = FindObjectOfType<Player>();
        gameManager = FindObjectOfType<GameManager>();
    }
	
	// Update is called once per frame
	void Update () {
        transform.position += new Vector3(0f, risingSpeed * Time.deltaTime, 0f);
        float distanceDiff = player.transform.position.y - transform.position.y;
        gameManager.WaterLevelUpdate(distanceDiff);
	}

    public void RisingWaterSpeed(float addedSpeed)
    {
        risingSpeed += addedSpeed;
    }
}
