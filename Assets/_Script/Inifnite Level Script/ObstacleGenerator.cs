using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour {

    [Header("General Settings")]
    [SerializeField] float navigateSpeed = 2.0f;
    [SerializeField] float generationCooldown = 2.0f;

    [Header("Tier Threshold")]
    [SerializeField] int obstacleCountToChange = 15;

    [Header("Obstacles Array")]
    [SerializeField] GameObject[] simpleObstacles;
    [SerializeField] GameObject[] intermediateObstacles;
    [SerializeField] GameObject[] advancedObstacles;

    int currentObstacleCount = 0;
    int lastGeneratedObsInt = 0;
    bool isSpawningObstacles = false;
    GameObject obstaclesParent;
    BoxCollider2D obsGeneratorCollider;

    void Start()
    {
        obstaclesParent = GameObject.FindGameObjectWithTag("Obstacles");
        obsGeneratorCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        Vector3 currentTrans = transform.position;
        transform.position = new Vector3(currentTrans.x, currentTrans.y + (navigateSpeed * Time.deltaTime), currentTrans.z);

        if (!isSpawningObstacles && !obsGeneratorCollider.IsTouchingLayers(LayerMask.GetMask("ObstacleForeground")))
        {
            if (currentObstacleCount < obstacleCountToChange)
            {
                Debug.Log("simple");
                SpawningObstacle(simpleObstacles);
            }
            else if (currentObstacleCount < obstacleCountToChange * 2)
            {
                Debug.Log("intermediate");
                SpawningObstacle(intermediateObstacles);
            }
            else 
            {
                Debug.Log("advanced");
                SpawningObstacle(advancedObstacles);
            }
        }
    }

    void SpawningObstacle(GameObject[] obstacles)
    {
        // RNG for an integer
        int generatedInt = Random.Range(0, obstacles.Length);

        // No repeating tiles immediately
        if (generatedInt == lastGeneratedObsInt) { return; }

        // Generating tile
        lastGeneratedObsInt = generatedInt;

        var chosenObstacle = obstacles[generatedInt];
        GameObject newObstacle = Instantiate(chosenObstacle, transform.position, Quaternion.identity);
        newObstacle.transform.parent = obstaclesParent.transform;

        currentObstacleCount++;
        Debug.Log("Current Count: " + currentObstacleCount);
        StartCoroutine(SpawningCooldown());             // Put this here to prevent being called before this function is called
    }

    IEnumerator SpawningCooldown()
    {
        isSpawningObstacles = true;
        yield return new WaitForSeconds(generationCooldown);
        isSpawningObstacles = false;
    }
}
