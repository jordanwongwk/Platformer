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
    [SerializeField] List<GameObject> simpleObstacles;
    [SerializeField] List<GameObject> intermediateObstacles;
    [SerializeField] List<GameObject> advancedObstacles;
    [SerializeField] List<GameObject> eliteObstacles;
    
    [SerializeField] List<GameObject> endlessModeObstacles;

    int currentObstacleCount = 0;
    int lastGeneratedObsInt = 0;
    bool isSpawningObstacles = false;
    bool isInEndlessMode = false;
    GameObject obstaclesParent;
    BoxCollider2D obsGeneratorCollider;

    void Start()
    {
        obstaclesParent = GameObject.FindGameObjectWithTag("Obstacles");
        obsGeneratorCollider = GetComponent<BoxCollider2D>();
        RegisteringObstaclesToEndless();
    }

    private void RegisteringObstaclesToEndless()
    {
        endlessModeObstacles.AddRange(simpleObstacles);
        endlessModeObstacles.AddRange(intermediateObstacles);
        endlessModeObstacles.AddRange(advancedObstacles);
        endlessModeObstacles.AddRange(eliteObstacles);
        // Add more obstacles when there are more tiers
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
            else if (currentObstacleCount < obstacleCountToChange * 3)
            {
                Debug.Log("advanced");
                SpawningObstacle(advancedObstacles);
            }
            else if (currentObstacleCount < obstacleCountToChange * 4)
            {
                Debug.Log("elite");
                SpawningObstacle(eliteObstacles);
            }
            else
            {
                Debug.Log("endless");
                SpawningObstacle(endlessModeObstacles);
                if (!isInEndlessMode) { isInEndlessMode = true; Debug.Log("activating endless mode"); }
            }
        }
    }

    void SpawningObstacle(List<GameObject> obstacles)
    {
        // RNG for an integer
        int generatedInt = Random.Range(0, obstacles.Count);

        // No repeating tiles immediately
        if (generatedInt == lastGeneratedObsInt) { return; }

        // Generating tile
        lastGeneratedObsInt = generatedInt;

        var chosenObstacle = obstacles[generatedInt];
        GameObject newObstacle = Instantiate(chosenObstacle, transform.position, Quaternion.identity);
        newObstacle.transform.parent = obstaclesParent.transform;

        currentObstacleCount++;
        StartCoroutine(SpawningCooldown());             // Put this here to prevent being called before this function is called
    }

    IEnumerator SpawningCooldown()
    {
        isSpawningObstacles = true;
        yield return new WaitForSeconds(generationCooldown);
        isSpawningObstacles = false;
    }
}
