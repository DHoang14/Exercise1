using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item : MonoBehaviour
{

    public List<Distance> distances = new List<Distance>();
    public Position[] neighbors;
    

    public List<Position> unseenNeighbors = new List<Position>();
    public bool checkingDistance = false;
    private Material ogMat;
    // Start is called before the first frame update
    void Start()
    {
        ogMat = GetComponent<Renderer>().material;

        Position newItem = new Position(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z, gameObject);


        distances = Player.instance.RegisterItem(newItem);

        //closest two neighbors
        neighbors = new Position[Player.instance.neighborsCount];
        for (int k = 0; k < distances.Count; k++)
        {
            if (k < Player.instance.neighborsCount)
            {
                neighbors[k] = new Position(distances[k].calculatedObj.transform.position.x, distances[k].calculatedObj.transform.position.y, distances[k].calculatedObj.transform.position.z, distances[k].calculatedObj);


                neighbors[k].referencedObj.GetComponent<Item>().UpdateUnseen(newItem);


            }
            else
            {
                break;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (checkingDistance)
        {
            float playerX = Player.instance.transform.position.x;
            float playerY = Player.instance.transform.position.y;
            float playerZ = Player.instance.transform.position.z;

            float distanceBtwnThis = Mathf.Sqrt(Mathf.Pow((playerX - gameObject.transform.position.x), 2.0f) + 
                Mathf.Pow((playerY - gameObject.transform.position.y), 2.0f) + 
                Mathf.Pow((playerZ - gameObject.transform.position.z), 2.0f));

            GameObject minObj = gameObject;
            float min = distanceBtwnThis;
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors != null)
                {
                    float distanceBtwnNeighbor = Mathf.Sqrt(Mathf.Pow((playerX - neighbors[i].x), 2.0f) +
                        Mathf.Pow((playerY - neighbors[i].y), 2.0f) +
                        Mathf.Pow((playerZ - neighbors[i].z), 2.0f));

                    if (distanceBtwnNeighbor < min)
                    {
                        minObj = neighbors[i].referencedObj;
                        min = distanceBtwnNeighbor;
                    }
                }
            }

            for (int j = 0; j < unseenNeighbors.Count; j++)
            {
                float distanceBtwnNeighbor = Mathf.Sqrt(Mathf.Pow((playerX - unseenNeighbors[j].x), 2.0f) +
                    Mathf.Pow((playerY - unseenNeighbors[j].y), 2.0f) +
                    Mathf.Pow((playerZ - unseenNeighbors[j].z), 2.0f));

                if (distanceBtwnNeighbor < min)
                {
                    minObj = unseenNeighbors[j].referencedObj;
                    min = distanceBtwnNeighbor;
                }
            }

            if (minObj != gameObject)
            {
                ColorChange switchColors = new ColorChange(minObj, gameObject, ogMat); 
                Player.instance.colorQueue.Enqueue(switchColors);

            }

        }
    }

    public void UpdateRankings(float newDistance, GameObject newGameObject)
    {

        Distance toAdd = new Distance(newDistance, newGameObject);
        bool inserted = false;
        
        //sorts rankings list by min
        for (int j = 0; j < distances.Count; j++)
        {
            if (newDistance <= distances[j].distance)
            {
                distances.Insert(j, toAdd);
                inserted = true;
                break;
            }
        }
        if (!inserted)
        {
            distances.Add(toAdd);
        }

        //updates closest two neighbors
        Position newItem = new Position(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z, gameObject);

        //before changing neighbors inform old ones to remove from unseen
        for (int l = 0; l < neighbors.Length; l++)
        {
            if (neighbors[l].referencedObj != null)
            {
                neighbors[l].referencedObj.GetComponent<Item>().UpdateRemove(newItem);
            }
        }

        //changed neighbors
        for (int k = 0; k < distances.Count; k++)
        {
            if (k < Player.instance.neighborsCount)
            {    

                neighbors[k] = new Position(distances[k].calculatedObj.transform.position.x, distances[k].calculatedObj.transform.position.y, distances[k].calculatedObj.transform.position.z, distances[k].calculatedObj);

                neighbors[k].referencedObj.GetComponent<Item>().UpdateUnseen(newItem);


            }
            else
            {
                break;
            }
        }
    }

   

    public void UpdateUnseen(Position newItem)
    {
        bool unique = true;
 
        for (int i = 0; i < neighbors.Length; i++)
        {

            if (neighbors[i].referencedObj != null)
            {


                if (neighbors[i].referencedObj == newItem.referencedObj)
                {
                    unique = false;
                }

            }
        }
        if (unique)
        {
            unseenNeighbors.Add(newItem);
        }

    }


    public void UpdateRemove(Position newItem)
    {
        if (unseenNeighbors.Contains(newItem))
        {
            unseenNeighbors.Remove(newItem);
        }
    }
}
