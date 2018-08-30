using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGenerator : MonoBehaviour {

    [Header("General Settings")]
    [SerializeField] float navigateSpeed = 2.0f;
    [SerializeField] float generationCooldown = 2.0f;

    [Header("Tier Threshold")]
    [SerializeField] int backgroundCountToChange = 15;
    
    [Header("Background List")]
    [SerializeField] List<BackgroundTier> myBackgroundTier;

    List<GameObject> endlessModeBackground = new List<GameObject>();

    int currentBackgroundTier = 0;
    int currentBackgroundCount = 0;
    int currentGeneratedInt = 0;
    int lastGeneratedInt = 0;
    bool isSpawning = false;
    GameObject backgroundParent;
    BoxCollider2D generatorCol;

	void Start () {
        backgroundParent = GameObject.FindGameObjectWithTag("Backgrounds");
        generatorCol = GetComponent<BoxCollider2D>();
        RegisterEndlessModeBackground();
	}

    void RegisterEndlessModeBackground()
    {
        for (int i = 0; i < myBackgroundTier.Count; i++)
        {
            for (int j = 0; j < myBackgroundTier[i].GetBackgroundCount(); j++)
            {
                endlessModeBackground.Add(myBackgroundTier[i].GetBackground(j));
            }
        }
    }
	
	void Update () {
        Vector3 currentTrans = transform.position;
        transform.position = new Vector3(currentTrans.x, currentTrans.y + (navigateSpeed * Time.deltaTime), currentTrans.z);

        if (!isSpawning && !generatorCol.IsTouchingLayers(LayerMask.GetMask("TileBackground")))
        {
            if (currentBackgroundTier < myBackgroundTier.Count)
            {
                if (currentBackgroundCount < backgroundCountToChange)
                {
                    int generatedNumber = GetRandomGeneratedNumber(myBackgroundTier[currentBackgroundTier].GetBackgroundCount());
                    GameObject chosenBackground = myBackgroundTier[currentBackgroundTier].GetBackground(generatedNumber);
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
                int generatedNumber = GetRandomGeneratedNumber(endlessModeBackground.Count);
                GameObject chosenEndlessBackground = endlessModeBackground[generatedNumber];
                GenerateBackground(chosenEndlessBackground);
            }

        }
	}

    int GetRandomGeneratedNumber(int maxValue)
    {
        while (currentGeneratedInt == lastGeneratedInt)
        {
            currentGeneratedInt = Random.Range(0, maxValue);
        }
        lastGeneratedInt = currentGeneratedInt;
        return currentGeneratedInt;
    }

    void GenerateBackground(GameObject chosenBG)
    {
        GameObject generatedBG = Instantiate(chosenBG, transform.position, Quaternion.identity);
        generatedBG.transform.parent = backgroundParent.transform;
        StartCoroutine(SpawningCooldown());
    }

    IEnumerator SpawningCooldown()
    {
        isSpawning = true;
        yield return new WaitForSeconds(generationCooldown);
        isSpawning = false;
    }
}
