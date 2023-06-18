using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// data structure to record position information of a gameObject
/// </summary>
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

/// <summary>
/// data structure to record distance from gameObject
/// </summary>
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

/// <summary>
/// data structure to keep track of which gameObjects need to have their colors changed
/// </summary>
[System.Serializable]
public struct ColorChange
{

    public GameObject changeColor;
    public GameObject undoColor;


    public ColorChange(GameObject change, GameObject previous)
    {
        changeColor = change;
        undoColor = previous;

    }

};
