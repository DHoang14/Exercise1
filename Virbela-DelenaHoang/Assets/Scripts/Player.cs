using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance = null;

    public GameObject itemPrefab;
    public GameObject botPrefab;

    public List<Position> itemPositions;
    public List<Position> botPositions;
    public int neighborsCount;
    
    
    public Color itemHighlight;
    private Material itemColor;

    public Color botHighlight;
    private Material botColor;

    public Queue<ColorChange> colorQueue = new Queue<ColorChange>();

    bool isZPressed = false;
    bool isXPressed = false;

    public Vector3 minPos;
    public Vector3 maxPos;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        itemColor = Resources.Load("Materials/ItemColor", typeof(Material)) as Material;
        itemColor.color = itemHighlight;

        botColor = Resources.Load("Materials/BotColor", typeof(Material)) as Material;
        botColor.color = botHighlight;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (colorQueue.Count > 0)
        {
            while (colorQueue.Count > 0)
            {
                ColorChange head = colorQueue.Dequeue();
                if (head.undoColor.GetComponent<Item>() != null)
                {
                    head.undoColor.GetComponent<Item>().checkingDistance = false;
                    
                }
                else
                {
                    head.undoColor.GetComponent<Bot>().checkingDistance = false;

                }

                head.undoColor.GetComponent<Renderer>().material = head.originalMat;

                if (head.changeColor.GetComponent<Item>() != null)
                {
                    head.changeColor.GetComponent<Item>().checkingDistance = true;
                    head.changeColor.GetComponent<Renderer>().material = itemColor;
                }
                else
                {
                    head.changeColor.GetComponent<Bot>().checkingDistance = true;
                    head.changeColor.GetComponent<Renderer>().material = botColor;
                }
                
            }

        }

        isZPressed = Input.GetKeyDown("z");
        if (isZPressed)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(minPos.x, maxPos.x),
                Random.Range(minPos.y, maxPos.y),
                Random.Range(minPos.z, maxPos.z)
            );
            Debug.Log("Item has been spawned at " + randomPos.x + ", " + randomPos.y + ", " + randomPos.z);
            Instantiate(itemPrefab, randomPos, Quaternion.identity);
        }

        isXPressed = Input.GetKeyDown("x");
        if (isXPressed)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(minPos.x, maxPos.x),
                Random.Range(minPos.y, maxPos.y),
                Random.Range(minPos.z, maxPos.z)
            );
            Debug.Log("Bot has been spawned at " + randomPos.x + ", " + randomPos.y + ", " + randomPos.z);

            Instantiate(botPrefab, randomPos, Quaternion.identity);
        }
    }

    public List<Distance> RegisterItem(Position item)
    {
        List<Distance> returnList = new List<Distance>();

        //calculates distances from all other registered Position
        if(itemPositions.Count > 0)
        {
            for(int i = 0; i < itemPositions.Count; i++)
            {
                //calculates distance
                float distanceBtwn = Mathf.Sqrt(Mathf.Pow((itemPositions[i].x - item.x), 2.0f) + Mathf.Pow((itemPositions[i].y - item.y), 2.0f) + Mathf.Pow((itemPositions[i].z - item.z), 2.0f));
                Distance newDistance = new Distance(distanceBtwn, itemPositions[i].referencedObj);

                //sorts new distance rankings
                bool inserted = false;
                for (int j = 0; j < returnList.Count; j++)
                {
                    if (distanceBtwn <= returnList[j].distance)
                    {
                        returnList.Insert(j, newDistance);
                        inserted = true;
                        break;
                    }
                }
                if (!inserted)
                {
                    returnList.Add(newDistance);
                }

                itemPositions[i].referencedObj.GetComponent<Item>().UpdateRankings(distanceBtwn, item.referencedObj);
                
            }
        }
        else
        {
            item.referencedObj.GetComponent<Renderer>().material = itemColor;
            item.referencedObj.GetComponent<Item>().checkingDistance = true;
        }
        //adds item to registered Position list
        itemPositions.Add(item);

        return returnList;
    }

    public List<Distance> RegisterBot(Position bot)
    {
        List<Distance> returnList = new List<Distance>();

        //calculates distances from all other registered Position
        if (botPositions.Count > 0)
        {
            for (int i = 0; i < botPositions.Count; i++)
            {
                //calculates distance
                float distanceBtwn = Mathf.Sqrt(Mathf.Pow((botPositions[i].x - bot.x), 2.0f) + Mathf.Pow((botPositions[i].y - bot.y), 2.0f) + Mathf.Pow((botPositions[i].z - bot.z), 2.0f));
                Distance newDistance = new Distance(distanceBtwn, botPositions[i].referencedObj);

                //sorts new distance rankings
                bool inserted = false;
                for (int j = 0; j < returnList.Count; j++)
                {
                    if (distanceBtwn <= returnList[j].distance)
                    {
                        returnList.Insert(j, newDistance);
                        inserted = true;
                        break;
                    }
                }
                if (!inserted)
                {
                    returnList.Add(newDistance);
                }

                botPositions[i].referencedObj.GetComponent<Bot>().UpdateRankings(distanceBtwn, bot.referencedObj);

            }
        }
        else
        {
            bot.referencedObj.GetComponent<Renderer>().material = botColor;
            bot.referencedObj.GetComponent<Bot>().checkingDistance = true;
        }
        //adds item to registered Position list
        botPositions.Add(bot);

        return returnList;
    }
}

[System.Serializable]
public struct Position
{

    public float x;
    public float y;
    public float z;
    public GameObject referencedObj;

    public Position(float xTransform, float yTransform, float zTransform, GameObject obj)
    {
        x = xTransform;
        y = yTransform;
        z = zTransform;
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
