using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLine : MonoBehaviour {

    [SerializeField] float lineWidth = 0.1f;

    LineRenderer line;
    List<GameObject> pointsList = new List<GameObject>();

	// Use this for initialization
	void Start () {
        line = GetComponent<LineRenderer>();

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = transform.childCount;

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            line.SetPosition(i, transform.GetChild(i).transform.position);
            pointsList.Add(transform.GetChild(i).gameObject);
        }
    }

    public List<GameObject> GetPlatformPointsList()
    {
        return pointsList;
    }

    private void OnDrawGizmos()
    {
        Vector3 firstPos = transform.GetChild(0).position;
        Vector3 lastPos = firstPos;

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Vector3 currentPos = transform.GetChild(i).position;

            Gizmos.DrawWireSphere(currentPos, 0.05f);
            Gizmos.DrawLine(lastPos, currentPos);

            lastPos = currentPos;
        }
    }
}
