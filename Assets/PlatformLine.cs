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
}
