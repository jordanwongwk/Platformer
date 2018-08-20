using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGenerator : MonoBehaviour {

    [SerializeField] float navigateSpeed = 2.0f;
    [SerializeField] float generationCooldown = 2.0f;
    [SerializeField] GameObject[] backgroundTiles;

    bool isSpawning = false;
    GameObject backgroundParent;
    BoxCollider2D generatorCol;

	void Start () {
        backgroundParent = GameObject.FindGameObjectWithTag("Backgrounds");
        generatorCol = GetComponent<BoxCollider2D>();
	}
	
	void Update () {
        Vector3 currentTrans = transform.position;
        transform.position = new Vector3(currentTrans.x, currentTrans.y + (navigateSpeed * Time.deltaTime), currentTrans.z);

        if (!isSpawning && !generatorCol.IsTouchingLayers(LayerMask.GetMask("TileBackground")))
        {
            StartCoroutine(SpawningCooldown());         // Spawning cooldown to prevent repeated spawn for multiple colliders (SideWall)

            int generatedInt = Random.Range(0, backgroundTiles.Length);
            var chosenBackground = backgroundTiles[generatedInt];
            GameObject newBackground = Instantiate(chosenBackground, transform.position, Quaternion.identity);
            newBackground.transform.parent = backgroundParent.transform;
        }
	}

    IEnumerator SpawningCooldown()
    {
        isSpawning = true;
        yield return new WaitForSeconds(generationCooldown);
        isSpawning = false;
    }
}
