using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGeneratorScript : MonoBehaviour {

    [Header("General Configurations")]
    [SerializeField] float navigateSpeed = 2.0f;
    [SerializeField] float generationCooldown = 2.0f;

    [Header("Obstacles Configurations")]
    [SerializeField] int obstaclesCountToChangeTier = 15;
    [SerializeField] List<ObstacleTier> obstacleTierLists;

    [Header("Backgrounds Configurations")]
    [SerializeField] int backgroundsCountToChangeTier = 15;
    [SerializeField] List<BackgroundTier> backgroundTierLists;

    // Endless Mode Setup
    List<GameObject> endlessModeObstacles = new List<GameObject>();
    List<GameObject> endlessModeBackgrounds = new List<GameObject>();

    // Private var for General
    BoxCollider2D generatorCol;

    // Private var for Obstacles
    int currentObstacleTier = 0;
    int currentObstacleCount = 0;
    int currentGeneratedObstacleInt = 0;
    int lastGeneratedObstacleInt = 0;
    int powerUpGeneratingRNG = 10;
    bool isSpawningObstacles = false;
    GameObject obstaclesParent;

    // Private var for Backgrounds
    int currentBackgroundTier = 0;
    int currentBackgroundCount = 0;
    int currentGeneratedBackgroundInt = 0;
    int lastGeneratedBackgroundInt = 0;
    bool isSpawningBackground = false;
    GameObject backgroundParent;


    // Use this for initialization
    void Start ()
    {
        backgroundParent = GameObject.FindGameObjectWithTag("Backgrounds");
        obstaclesParent = GameObject.FindGameObjectWithTag("Obstacles");
        generatorCol = GetComponent<BoxCollider2D>();
        RegisterEndlessMode();
    }

    void RegisterEndlessMode()
    {
        // Obstacles
        for (int i = 0; i < obstacleTierLists.Count; i++)
        {
            for (int j = 0; j < obstacleTierLists[i].GetObstacleCount(); j++)
            {
                endlessModeObstacles.Add(obstacleTierLists[i].GetObstacle(j));
            }
        }

        // Backgrounds
        for (int i = 0; i < backgroundTierLists.Count; i++)
        {
            for (int j = 0; j < backgroundTierLists[i].GetBackgroundCount(); j++)
            {
                endlessModeBackgrounds.Add(backgroundTierLists[i].GetBackground(j));
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 currentTrans = transform.position;
        transform.position = new Vector3(currentTrans.x, currentTrans.y + (navigateSpeed * Time.deltaTime), currentTrans.z);

        // Spawn Obstacles
        if (!isSpawningObstacles && !generatorCol.IsTouchingLayers(LayerMask.GetMask("ObstacleForeground")))
        {
            if (currentObstacleTier < obstacleTierLists.Count)
            {
                if (currentObstacleCount < obstaclesCountToChangeTier)
                {
                    int numberToSpawn = GeneratedNumber(obstacleTierLists[currentGeneratedObstacleInt].GetObstacleCount());
                    GameObject objectToSpawn = obstacleTierLists[currentObstacleTier].GetObstacle(numberToSpawn);
                    GenerateObstacle(objectToSpawn);
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

        // Spawn Backgrounds
        if (!isSpawningBackground && !generatorCol.IsTouchingLayers(LayerMask.GetMask("TileBackground")))
        {
            if (currentBackgroundTier < backgroundTierLists.Count)
            {
                if (currentBackgroundCount < backgroundsCountToChangeTier)
                {
                    int generatedNumber = GetRandomGeneratedNumber(backgroundTierLists[currentBackgroundTier].GetBackgroundCount());
                    GameObject chosenBackground = backgroundTierLists[currentBackgroundTier].GetBackground(generatedNumber);
                    GenerateBackground(chosenBackground);
                    currentBackgroundCount++;
                }
                else
                {
                    currentBackgroundCount = 0;
                    currentBackgroundTier++;
                }
            }
            else
            {
                int generatedNumber = GetRandomGeneratedNumber(endlessModeBackgrounds.Count);
                GameObject chosenEndlessBackground = endlessModeBackgrounds[generatedNumber];
                GenerateBackground(chosenEndlessBackground);
            }
        }
    }

    #region Spawning Obstacles
    int GeneratedNumber(int maxValue)
    {
        while (currentGeneratedObstacleInt == lastGeneratedObstacleInt)
        {
            currentGeneratedObstacleInt = Random.Range(0, maxValue);
        }
        lastGeneratedObstacleInt = currentGeneratedObstacleInt;
        return currentGeneratedObstacleInt;
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

        StartCoroutine(SpawningObstacleCooldown());             // Put this here to prevent being called before this function is called
    }

    IEnumerator SpawningObstacleCooldown()
    {
        isSpawningObstacles = true;
        yield return new WaitForSeconds(generationCooldown);
        isSpawningObstacles = false;
    }
    #endregion

    #region Spawning Background
    int GetRandomGeneratedNumber(int maxValue)
    {
        while (currentGeneratedBackgroundInt == lastGeneratedBackgroundInt)
        {
            currentGeneratedBackgroundInt = Random.Range(0, maxValue);
        }
        lastGeneratedBackgroundInt = currentGeneratedBackgroundInt;
        return currentGeneratedBackgroundInt;
    }

    void GenerateBackground(GameObject chosenBG)
    {
        GameObject generatedBG = Instantiate(chosenBG, transform.position, Quaternion.identity);
        generatedBG.transform.parent = backgroundParent.transform;
        StartCoroutine(SpawningBackgroundCooldown());
    }

    IEnumerator SpawningBackgroundCooldown()
    {
        isSpawningBackground = true;
        yield return new WaitForSeconds(generationCooldown);
        isSpawningBackground = false;
    }
    #endregion
}
