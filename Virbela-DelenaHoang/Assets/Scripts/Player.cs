using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //One player instance is made static in order to allow all bots and items to quickly find the 
    //player and register themselves with it. This allows the player to have a working list of current
    //bots and items that they could use to notify previous bots and items of new bots and items, so that 
    //they can update their respective minimum distance rankings. It is also more efficient than having
    //every item and bot use Find() every time they are created to find the player as it does not need to look
    //through every gameObject in the scene.
    public static Player instance = null;

    //needed to instantiate new items and bots
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

    //lowest and highest x, y, z values for creating new items and bots in random positions
    public Vector3 minPos;
    public Vector3 maxPos;

    void Awake()
    {
        //Ensures there is only one player instance and it is available to all items and bots for quick reference.
        //Needs to be in Awake() instead of Start() in case the Item or Bot scripts on the items and bots is run
        //before the Player script is run at the start of runtime.
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        //changes item highlight color to specified color
        itemColor = Resources.Load("Materials/ItemColor", typeof(Material)) as Material;
        itemColor.color = itemHighlight;

        //changes bot highlight color to specified color
        botColor = Resources.Load("Materials/BotColor", typeof(Material)) as Material;
        botColor.color = botHighlight;
    }

    // Update is called once per frame
    void Update()
    {
        //if a color change request has been queued, change colors FIFO
        if (colorQueue.Count > 0)
        {
            while (colorQueue.Count > 0) //while there are still requests in the queue
            {
                //retrieve first request
                ColorChange head = colorQueue.Dequeue();

                //checks if request is item or bot
                if (head.undoColor.GetComponent<Item>() != null)
                {
                    //tells the previous closest item to stop keeping track of who out of its neighbors and itself
                    //is closet to the player
                    head.undoColor.GetComponent<Item>().checkingDistance = false;
                    
                }
                else
                {
                    //tells the previous closest bot to stop keeping track of who out of its neighbors and itself
                    //is closet to the player
                    head.undoColor.GetComponent<Bot>().checkingDistance = false;

                }

                //returns previous closest item/bot to its original material aka unhighlights it
                head.undoColor.GetComponent<Renderer>().material = head.originalMat;

                //checks if request is item or bot
                if (head.changeColor.GetComponent<Item>() != null)
                {
                    //Tells new closest item to start keeping track of who is closer to the player between it 
                    //and its neighbors. Also highlights it with the item highlight color.
                    head.changeColor.GetComponent<Item>().checkingDistance = true;
                    head.changeColor.GetComponent<Renderer>().material = itemColor;
                }
                else
                {
                    //Tells new closest bot to start keeping track of who is closer to the player between it 
                    //and its neighbors. Also highlights it with the item highlight color.
                    head.changeColor.GetComponent<Bot>().checkingDistance = true;
                    head.changeColor.GetComponent<Renderer>().material = botColor;
                }
                
            }

        }

        //if z is pressed generate new item at a random position
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

        //if x is pressed generate new bots at random position
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

    //registers item to player's list of items
    public List<Distance> RegisterItem(Position item)
    {
        List<Distance> returnList = new List<Distance>();

        //if item list is not empty
        if(itemPositions.Count > 0)
        {
            for(int i = 0; i < itemPositions.Count; i++)
            {
                //calculates distance between new item and item in player's list of items
                float distanceBtwn = (itemPositions[i].pos - item.pos).sqrMagnitude;
                Distance newDistance = new Distance(distanceBtwn, itemPositions[i].referencedObj);

                //insertion sort works best when list is already sorted (O(n)). It seemed more efficient to sort the minimum
                //rankings list as it is being created.
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

                //notifies all items in player's list of items to update their minimum distance rankings to include the new item.
                //the distance calculated is provided so the item does not need to recalculate it.
                itemPositions[i].referencedObj.GetComponent<Item>().UpdateRankings(distanceBtwn, item.referencedObj);
                
            }
        }
        else //if nothing is in the item list, the first item will be the closest one to the player and is highlighted
        {
            item.referencedObj.GetComponent<Renderer>().material = itemColor;
            item.referencedObj.GetComponent<Item>().checkingDistance = true; //very first item needs to keep track of distance, so that it can pass it on to its closer neighbors later
        }

        //adds item to registered item list
        itemPositions.Add(item);

        return returnList;
    }

    //registers new bot to player's bot list
    public List<Distance> RegisterBot(Position bot)
    {
        List<Distance> returnList = new List<Distance>();

        //if bot list is not empty
        if (botPositions.Count > 0)
        {
            for (int i = 0; i < botPositions.Count; i++)
            {
                //calculates distance between new bot and bot in player's list of bot
                float distanceBtwn = (botPositions[i].pos - bot.pos).sqrMagnitude;
                Distance newDistance = new Distance(distanceBtwn, botPositions[i].referencedObj);

                //insertion sort works best when list is already sorted (O(n)). It seemed more efficient to sort the minimum
                //rankings list as it is being created.
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

                //notifies all bots in player's list of bots to update their minimum distance rankings to include the new bot.
                //the distance calculated is provided so the bot does not need to recalculate it.
                botPositions[i].referencedObj.GetComponent<Bot>().UpdateRankings(distanceBtwn, bot.referencedObj);

            }
        }
        else //if nothing is in the bot list, the first bot will be the closest one to the player and is highlighted
        {
            bot.referencedObj.GetComponent<Renderer>().material = botColor;
            bot.referencedObj.GetComponent<Bot>().checkingDistance = true;//very first bot needs to keep track of distance, so that it can pass it on to its closer neighbors later

        }

        //adds bot to registered bot list
        botPositions.Add(bot);

        return returnList;
    }
}
