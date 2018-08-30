using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Platformer/Background"))]
public class BackgroundTier : ScriptableObject {
    [SerializeField] List<GameObject> backgroundList;

    public GameObject GetBackground(int number)
    {
        return backgroundList[number];
    }

    public int GetBackgroundCount()
    {
        return backgroundList.Count;
    }
}
