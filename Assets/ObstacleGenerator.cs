using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour {

    [Header("General Settings")]
    [SerializeField] float navigateSpeed = 2.0f;
    [SerializeField] float generationCooldown = 2.0f;

    [Header("Tier Threshold")]
    [SerializeField] float changeToIntermediate = 250.0f;
    [SerializeField] float changeToAdvanced = 750.0f;

    [Header("Obstacles Array")]
    [SerializeField] GameObject[] simpleObstacles;
    [SerializeField] GameObject[] intermediateObstacles;
    [SerializeField] GameObject[] advancedObstacles;

    int lastGeneratedObsInt = 0;
    bool isSpawningObstacles = false;
    BoxCollider2D obsGeneratorCollider;

    void Start()
    {
        obsGeneratorCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        Vector3 currentTrans = transform.position;
        transform.position = new Vector3(currentTrans.x, currentTrans.y + (navigateSpeed * Time.deltaTime), currentTrans.z);

        if (!isSpawningObstacles && !obsGeneratorCollider.IsTouchingLayers(LayerMask.GetMask("ObstacleForeground")))
        {
            StartCoroutine(SpawningCooldown());

            if (transform.position.y <= changeToIntermediate)
            {
                Debug.Log("simple");
                SpawningObstacle(simpleObstacles);
            }
            else if (transform.position.y > changeToIntermediate && transform.position.y <= changeToAdvanced)
            {
                Debug.Log("intermediate");
                SpawningObstacle(intermediateObstacles);
            }
            else if (transform.position.y > changeToAdvanced)
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
        Instantiate(chosenObstacle, transform.position, Quaternion.identity);
    }

    IEnumerator SpawningCooldown()
    {
        isSpawningObstacles = true;
        yield return new WaitForSeconds(generationCooldown);
        isSpawningObstacles = false;
    }
}
