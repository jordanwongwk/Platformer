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
    [SerializeField] List<ObstacleTier> myObstacleTier;
    
    List<GameObject> endlessModeObstacles;

    int currentObstacleTier = 0;
    int currentObstacleCount = 0;
    int lastGeneratedObsInt = 0;
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
                    SpawningObstacle(myObstacleTier[currentObstacleTier]);
                }
                else
                {
                    currentObstacleCount = 0;
                    currentObstacleTier++;
                }
            }
            else
            {
                SpawningEndlessObstacle();
            }
        }
    }

    void SpawningObstacle(ObstacleTier tier)
    {
        // RNG for an integer
        int generatedInt = Random.Range(0, tier.GetObstacleCount());

        // No repeating tiles immediately
        if (generatedInt == lastGeneratedObsInt) { return; }

        // Generating tile
        lastGeneratedObsInt = generatedInt;

        var chosenObstacle = tier.GetObstacle(generatedInt);
        GameObject newObstacle = Instantiate(chosenObstacle, transform.position, Quaternion.identity);
        newObstacle.transform.parent = obstaclesParent.transform;

        currentObstacleCount++;                         // Prevent the return function at repeating value for increasing the count
        StartCoroutine(SpawningCooldown());             // Put this here to prevent being called before this function is called
    }

    IEnumerator SpawningCooldown()
    {
        isSpawningObstacles = true;
        yield return new WaitForSeconds(generationCooldown);
        isSpawningObstacles = false;
    }

    void SpawningEndlessObstacle()
    {
        int generatedInt = Random.Range(0, endlessModeObstacles.Count);
        if (generatedInt == lastGeneratedObsInt) { return; }
        lastGeneratedObsInt = generatedInt;

        var chosenObstacle = endlessModeObstacles[generatedInt];
        GameObject newObstacle = Instantiate(chosenObstacle, transform.position, Quaternion.identity);
        newObstacle.transform.parent = obstaclesParent.transform;

        StartCoroutine(SpawningCooldown());           
    }
}
