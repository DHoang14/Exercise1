using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public List<Distance> distances = new List<Distance>();
    public Position[] neighbors;
    public List<Position> unseenNeighbors = new List<Position>();
    public bool checkingDistance = false;
    
    // Start is called before the first frame update
    void Start()
    {


        //records bot's position for registration with player
        Position newBot = new Position(gameObject.transform.position, gameObject);
        distances = Player.instance.RegisterBot(newBot);

        //finds closest k neighbors where k is determined by the number of neighbors specified in the player's inspector
        neighbors = new Position[Player.instance.neighborsCount];
        for (int k = 0; k < distances.Count; k++)
        {
            if (k < Player.instance.neighborsCount)
            {
                neighbors[k] = new Position(distances[k].calculatedObj.transform.position, distances[k].calculatedObj);

                //Informs neighbor that they have been registered as a neighbor.
                //This is important in making sure that the neighbor is aware 
                //they need to keep track of this bot as well in the case that 
                //this bot is not registered as a neighbor by any other bots.
                //In the case it is not registered as a neighbor by anyone, it is 
                //possible that it will be completely overlooked when the player approaches it.
                neighbors[k].referencedObj.GetComponent<Bot>().UpdateUnseen(newBot);


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
        //O(1) but more specifically O(k) where k is represented as the specified constant number of neighbors in the
        //player's inspector. k should be less than the number of bots.
        //Only performs current distance calculations between the player, itself and its k neighbors at every update.
        //This is more efficient than constantly calculating what bot out of all bots is closest one by one as the
        //the number of calculations is signficantly reduced down to k + 2 where k < n.
        //Additionally, the calculations are only performed by one bot at a time (closest bot), so bots that are farther than the
        //the scope of this bot and its neighbors do not perform any unnecessary calculations. After all, if the player is 
        //closest to this bot and its neighbors, there shouldn't be any need to keep trying to calculate the distance between the
        //the player and an bot even farther than this bot and its neighbors as it would obviously not be the closest one.
        if (checkingDistance)
        {

            //calculates distance between player and this bot
            //technically the distance formula uses a square root after squaring the distances, but using a square root would be more computationally expensive.
            //it also isn't necessary to use the square root for this use case as it is only important to get a relative idea how far the objects are from each other and not know the exact distance.
            float distanceBtwnThis = (Player.instance.transform.position - gameObject.transform.position).sqrMagnitude;


            //initialize minimum to this bot
            GameObject minObj = gameObject;
            float min = distanceBtwnThis;

            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors != null) //if there is a neighbor in array of neighbors
                {
                    //calculates minimum between neighbor and the player
                    //technically the distance formula uses a square root after squaring the distances, but using a square root would be more computationally expensive.
                    //it also isn't necessary to use the square root for this use case as it is only important to get a relative idea how far the objects are from each other and not know the exact distance.
                    float distanceBtwnNeighbor = (Player.instance.transform.position - neighbors[i].pos).sqrMagnitude;

                    //if distance between neighbor and player is closer than the minimum, overwrite the minimum and set the closest bot to that neighbor
                    if (distanceBtwnNeighbor < min)
                    {
                        minObj = neighbors[i].referencedObj;
                        min = distanceBtwnNeighbor;
                    }
                }
            }


            //iterates through unseen neighbors (bots that registered this bot as a neighbor but might not be registered as being a neighbor with others itself)
            for (int j = 0; j < unseenNeighbors.Count; j++)
            {
                //calculates minimum between unseen neighbor and the player
                //technically the distance formula uses a square root after squaring the distances, but using a square root would be more computationally expensive.
                //it also isn't necessary to use the square root for this use case as it is only important to get a relative idea how far the objects are from each other and not know the exact distance.
                float distanceBtwnNeighbor = (Player.instance.transform.position - unseenNeighbors[j].pos).sqrMagnitude;


                //if distance between unseen neighbor and player is closer than the minimum, overwrite the minimum and set the closest bot to that unseen neighbor
                if (distanceBtwnNeighbor < min)
                {
                    minObj = unseenNeighbors[j].referencedObj;
                    min = distanceBtwnNeighbor;
                }
            }

            //if this bot is no longer the closest to the player, ask the player to switch the closest to the new closest
            if (minObj != gameObject)
            {
                ColorChange switchColors = new ColorChange(minObj, gameObject);
                Player.instance.colorQueue.Enqueue(switchColors);
            }

        }
    }

    //Updates minimum distance ranking. This is important in being able to check who the closest neighbors are for closest calculations.
    //Updated everytime there is a new bot in case it is spawned even closer to the player than this bot.
    public void UpdateRankings(float newDistance, GameObject newGameObject)
    {

        Distance toAdd = new Distance(newDistance, newGameObject);
        bool inserted = false;

        //insertion sort works best when list is already sorted (O(n)). This method only adds one more bot to already sorted minimum distance list
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

        //records own position in order to inform old neighbors to remove it from the list of unseen neighbors if it's there
        Position self = new Position(gameObject.transform.position, gameObject);

        //before changing neighbors inform old ones to remove from unseen
        for (int l = 0; l < neighbors.Length; l++)
        {
            //if neighbor exists in array, notify it to remove this bot from its list of unseen neighbors
            if (neighbors[l].referencedObj != null)
            {
                neighbors[l].referencedObj.GetComponent<Bot>().UpdateRemove(self);
            }
        }

        //change neighbors to new k closest neighbors according to new rankings
        for (int k = 0; k < distances.Count; k++)
        {
            if (k < Player.instance.neighborsCount)
            {

                neighbors[k] = new Position(distances[k].calculatedObj.transform.position, distances[k].calculatedObj);

                //inform new neighbors that they might need to add this bot as an unseen neighbor if they are not already neighbors
                neighbors[k].referencedObj.GetComponent<Bot>().UpdateUnseen(self);


            }
            else
            {
                break;
            }
        }
    }


    //if two bots are not already neighbors, register the new bot as its unseen neighbor
    public void UpdateUnseen(Position newBot)
    {
        bool unique = true;

        for (int i = 0; i < neighbors.Length; i++)
        {

            if (neighbors[i].referencedObj != null)
            {


                if (neighbors[i].referencedObj == newBot.referencedObj) //already neighbors
                {
                    unique = false;
                }

            }
        }
        if (unique) //if not already neighbors add to list of unseen neighbors
        {
            unseenNeighbors.Add(newBot);
        }

    }

    //remove from unseen neighbors if new bot is already in the list
    public void UpdateRemove(Position newBot)
    {
        if (unseenNeighbors.Contains(newBot))
        {
            unseenNeighbors.Remove(newBot);
        }
    }
}
