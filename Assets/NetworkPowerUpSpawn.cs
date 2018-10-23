using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPowerUpSpawn : MonoBehaviour {

    [SerializeField] List<GameObject> powerUpSpawnPoints;

    public Vector3 GetPowerUpSpawnPoint()
    {
        int randomPoint = Random.Range(0, powerUpSpawnPoints.Count);
        GameObject chosenSpawnPoint = powerUpSpawnPoints[randomPoint];
        return chosenSpawnPoint.transform.position;
    }
}
