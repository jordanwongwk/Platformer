using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteGenerator : MonoBehaviour {

    [SerializeField] float navigateSpeed = 2.0f;
    [SerializeField] GameObject backgroundTiles;

    bool isSpawning = false;
    BoxCollider2D generatorCol;

	void Start () {
        generatorCol = GetComponent<BoxCollider2D>();
	}
	
	void Update () {
        Vector3 currentTrans = transform.position;
        transform.position = new Vector3(currentTrans.x, currentTrans.y + (navigateSpeed * Time.deltaTime), currentTrans.z);

        if (!isSpawning && !generatorCol.IsTouchingLayers(LayerMask.GetMask("TileBackground")))
        {
            StartCoroutine(SpawningCooldown());         // Spawning cooldown to prevent repeated spawn for multiple colliders (SideWall)
            Instantiate(backgroundTiles, transform.position, Quaternion.identity);
        }
	}

    IEnumerator SpawningCooldown()
    {
        isSpawning = true;
        yield return new WaitForSeconds(2.0f);
        isSpawning = false;
    }
}
