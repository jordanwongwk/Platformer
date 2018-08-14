using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour {

    [SerializeField] float navigateSpeed = 2.0f;
    [SerializeField] float generationCooldown = 2.0f;
    [SerializeField] GameObject[] obstacleTiles;

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
            // RNG for an integer
            int generatedInt = Random.Range(0, obstacleTiles.Length);
            
            // No repeating tiles immediately
            if (generatedInt == lastGeneratedObsInt)
            {
                return;
            }

            // Generating tile
            lastGeneratedObsInt = generatedInt;
            Debug.Log("Obstacle generated. Current int: " + lastGeneratedObsInt);
            StartCoroutine(SpawningCooldown());
            var chosenObstacle = obstacleTiles[generatedInt];
            Instantiate(chosenObstacle, transform.position, Quaternion.identity);
        }
    }

    IEnumerator SpawningCooldown()
    {
        isSpawningObstacles = true;
        yield return new WaitForSeconds(generationCooldown);
        isSpawningObstacles = false;
    }
}
