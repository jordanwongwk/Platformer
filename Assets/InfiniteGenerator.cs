using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteGenerator : MonoBehaviour {

    [SerializeField] float navigateSpeed = 2.0f;
    [SerializeField] GameObject backgroundTiles;

    bool isSpawning = false;
    BoxCollider2D generatorCol;

	// Use this for initialization
	void Start () {
        generatorCol = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 currentTrans = transform.position;
        transform.position = new Vector3(currentTrans.x, currentTrans.y + (navigateSpeed * Time.deltaTime), currentTrans.z);
        if (!isSpawning && !generatorCol.IsTouchingLayers(LayerMask.GetMask("TileBackground")))
        {
            Debug.Log("Blah");
            isSpawning = true;
            Instantiate(backgroundTiles, transform.position, Quaternion.identity);
            StartCoroutine(SpawningCooldown());
        }
	}

    IEnumerator SpawningCooldown()
    {
        yield return new WaitForSeconds(2.0f);
        isSpawning = false;
    }
}
