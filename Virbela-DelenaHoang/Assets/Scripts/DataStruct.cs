using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Position
{

    public Vector3 pos;
    public GameObject referencedObj;

    public Position(Vector3 position, GameObject obj)
    {
        pos = position;
        referencedObj = obj;
    }

};

[System.Serializable]
public struct Distance
{

    public float distance;
    public GameObject calculatedObj;

    public Distance(float distances, GameObject obj)
    {
        distance = distances;
        calculatedObj = obj;
    }

};

[System.Serializable]
public struct ColorChange
{

    public GameObject changeColor;
    public GameObject undoColor;
    public Material originalMat;

    public ColorChange(GameObject change, GameObject previous, Material ogMat)
    {
        changeColor = change;
        undoColor = previous;
        originalMat = ogMat;
    }

};
