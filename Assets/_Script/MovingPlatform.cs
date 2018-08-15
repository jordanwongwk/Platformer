using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    [SerializeField] float speed = 5.0f;
    [SerializeField] float timeWaitAtPoint = 0.5f;
    [SerializeField] GameObject designatedMoveLine;
    [SerializeField] bool isMovingPositiveDirection = true;
    [Tooltip("One-Way means it will go back and forth at proper manner; Circular means it will proceed as if there's a track.")]
    [SerializeField] MovementType movePattern;
    [Tooltip("Starting point")]
    [SerializeField] int nextDestination = 0;

    public enum MovementType { OneWay, Circular }

    List<GameObject> pointsList = new List<GameObject>();

    float journeyCovered;
    bool isResting = false;     // Ensure coroutine only run once

    void Start()
    {
        pointsList = designatedMoveLine.GetComponent<PlatformLine>().GetPlatformPointsList();
    }

    // Update is called once per frame
    void Update () {
        if (!isResting)
        {
            Move();
            if (journeyCovered >= 1)
            {
                StartCoroutine(ChangeDestination());
            }
        }
    }

    private void Move()
    {
        float journey = speed * Time.deltaTime;
        float totalDistance = Vector3.Distance(transform.position, pointsList[nextDestination].transform.position);
        journeyCovered = journey / totalDistance;

        transform.position = Vector3.Lerp(transform.position, pointsList[nextDestination].transform.position, journeyCovered);
    }

    IEnumerator ChangeDestination()
    {
        isResting = true;
        yield return new WaitForSeconds (timeWaitAtPoint);
        isResting = false;

        switch (movePattern)
        {
            case MovementType.OneWay:
                if (isMovingPositiveDirection) { nextDestination++; }
                else if (!isMovingPositiveDirection) { nextDestination--; }

                // The reason for using +2 / -2 is because if we +1 / -1, the point will return to the previous point
                // This means it will remain at the point where the check is done. Thus, it will be backed by 2 units instead.
                if (nextDestination >= pointsList.Count)
                {
                    nextDestination -= 2;
                    isMovingPositiveDirection = false;
                }
                else if (nextDestination < 0)
                {
                    nextDestination += 2;
                    isMovingPositiveDirection = true;
                }
                break;

            case MovementType.Circular:
                if (isMovingPositiveDirection) { nextDestination++; }
                else if (!isMovingPositiveDirection) { nextDestination--; }

                if (nextDestination >= pointsList.Count)
                {
                    nextDestination = 1;
                }
                else if (nextDestination < 0)
                {
                    nextDestination = pointsList.Count - 2;
                }
                break;

            default:
                Debug.Log("Have you assigned a movement type?");
                break;
        }
    }
}
