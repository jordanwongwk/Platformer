using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Platformer/Obstacle")]
public class ObstacleTier : ScriptableObject {

    [SerializeField] List<GameObject> obstacleList;

    public GameObject GetObstacle (int number)
    {
        return obstacleList[number];
    }

    public int GetObstacleCount()
    {
        return obstacleList.Count;
    }
}
