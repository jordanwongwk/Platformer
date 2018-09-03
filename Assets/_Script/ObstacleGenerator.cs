using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour {

    [Header("General Settings")]
    [SerializeField] float navigateSpeed = 2.0f;
    [SerializeField] float generationCooldown = 2.0f;

    [Header("Tier Threshold")]
    [SerializeField] int obstacleCountToChange = 15;

    [Header("Obstacles List")]
    [SerializeField] List<ObstacleTier> myObstacleTier;
    
    List<GameObject> endlessModeObstacles = new List<GameObject>();

    int currentObstacleTier = 0;
    int currentObstacleCount = 0;
    int currentGeneratedInt = 0;
    int lastGeneratedInt = 0;
    int powerUpGeneratingRNG = 10;
    bool isSpawningObstacles = false;
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
        for (int i = 0; i < myObstacleTier.Count; i++)
        {
            for (int j = 0; j < myObstacleTier[i].GetObstacleCount(); j++)
            {
                endlessModeObstacles.Add(myObstacleTier[i].GetObstacle(j));
            }
        }
    }

    void Update()
    {
        Vector3 currentTrans = transform.position;
        transform.position = new Vector3(currentTrans.x, currentTrans.y + (navigateSpeed * Time.deltaTime), currentTrans.z);

        if (!isSpawningObstacles && !obsGeneratorCollider.IsTouchingLayers(LayerMask.GetMask("ObstacleForeground")))
        {
            if (currentObstacleTier < myObstacleTier.Count)
            {
                if (currentObstacleCount < obstacleCountToChange)
                {
                    int numberToSpawn = GeneratedNumber(myObstacleTier[currentGeneratedInt].GetObstacleCount());
                    GameObject objectToSpawn = myObstacleTier[currentObstacleTier].GetObstacle(numberToSpawn);
                    GenerateObstacle (objectToSpawn);
                    currentObstacleCount++;                        
                }
                else
                {
                    currentObstacleCount = 0;
                    currentObstacleTier++;
                }
            }
            else
            {
                int numberToSpawn = GeneratedNumber(endlessModeObstacles.Count);
                GameObject endlessObjectToSpawn = endlessModeObstacles[numberToSpawn];
                GenerateObstacle(endlessObjectToSpawn);
            }
        }
    }

    int GeneratedNumber(int maxValue)
    {
        while (currentGeneratedInt == lastGeneratedInt)
        {
            currentGeneratedInt = Random.Range(0, maxValue);
        }
        lastGeneratedInt = currentGeneratedInt;
        return currentGeneratedInt;
    }

    void GenerateObstacle(GameObject obstacleToGenerate)
    {
        GameObject newObstacle = Instantiate(obstacleToGenerate, transform.position, Quaternion.identity);
        newObstacle.transform.parent = obstaclesParent.transform;

        // Randomly generate power-up
        int randomRNG = Random.Range(1, 100);
        if (randomRNG <= powerUpGeneratingRNG)
        {
            newObstacle.GetComponent<Obstacle>().SpawnPowerUp();
            powerUpGeneratingRNG = 10;
        }
        else
        {
            powerUpGeneratingRNG += 10;
        }

        StartCoroutine(SpawningCooldown());             // Put this here to prevent being called before this function is called
    }

    IEnumerator SpawningCooldown()
    {
        isSpawningObstacles = true;
        yield return new WaitForSeconds(generationCooldown);
        isSpawningObstacles = false;
    }
}
