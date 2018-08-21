using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPlayer : MonoBehaviour {
    
    [SerializeField] Vector3[] movePoints;
    [SerializeField] float runSpeed = 2f;
    [SerializeField] float timeToWait = 3f;

    int currentDestination = 1;
    float distanceTravelled;
    bool isResting;

    // Update is called once per frame
    void Update () {
        if (!isResting)
        {
            Move();
            if (distanceTravelled >= 1)
            {
                GetComponent<Animator>().SetBool("isWalking", false);
                StartCoroutine(Resting());
            }
        }

	}

    void Move()
    {
        float journey = runSpeed * Time.deltaTime;
        float distance = Vector3.Distance(transform.position, movePoints[currentDestination]);
        distanceTravelled = journey / distance;

        transform.position = Vector3.Lerp(transform.position, movePoints[currentDestination], distanceTravelled);

        GetComponent<Animator>().SetBool("isWalking", true);
    }

    IEnumerator Resting()
    {
        isResting = true;

        yield return new WaitForSeconds(timeToWait);

        if (currentDestination == 1)
        {
            currentDestination--;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (currentDestination == 0)
        {
            currentDestination++;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        isResting = false;
    }
}
