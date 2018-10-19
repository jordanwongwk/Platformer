using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    [SerializeField] GameObject powerUp;
    [SerializeField] List<GameObject> spawnPoint;

    public void SpawnPowerUp()
    {
        int randomPoint = Random.Range(0, spawnPoint.Count);
        GameObject powerUpInstance = Instantiate(powerUp, spawnPoint[randomPoint].transform.position, Quaternion.identity);
        powerUpInstance.transform.parent = gameObject.transform;
    }
}
