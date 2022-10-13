using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubControl : MonoBehaviour
{
    private Player player;
    public GameObject cave;

    // scenery edges (put these in a separate data class so they're not repeated in Fish etc.)
    private float LeftEdge = -22f;
    private float RightEdge = 2f;

    private float WestCaveLeftX = -44.81f;
    private float WestCaveRightX = -26f;
    private float EastCaveRightX = 29f;
    private float EastCaveLeftX = 6f;

    private float leftX = -21f;
    private float rightX = -2.6f;

    private float speed = 0.0002f;
    private float targetSpeed;
    private float workingSpeed;
    private float exitSpeed = 0f;
    private float puzzleCompletionBoost = 0f;

    private bool caveScene = false;
    
    float runLength;


    // Start is called before the first frame update
    void Start()
    {
        runLength = rightX - leftX;
    }

    public float GetSpeed()
    {
        return workingSpeed;
    }

    public void SpeedBoost(int numPuzzlesComplete)
    {
        // nm keep it slow
        //puzzleCompletionBoost = (speed / 2) * numPuzzlesComplete;
        puzzleCompletionBoost = numPuzzlesComplete == 2 ? speed*5 : (speed / 2);
    }

    public void EnterCave(float subX)
    {
        caveScene = true;

        cave.transform.position = new Vector3(subX - 3f, cave.transform.position.y, cave.transform.position.z);
        
        WestCaveRightX = cave.transform.position.x;
        WestCaveLeftX = WestCaveRightX -(44.81f - 26f);
    }

    public void ExitCave()
    {
        Debug.Log("EXITING CAVE soon");
        float newSubX = transform.position.x + EastCaveRightX - WestCaveRightX;
        transform.position = new Vector3(newSubX, transform.position.y, transform.position.z);


        cave.transform.position = new Vector3(-26f, cave.transform.position.y, cave.transform.position.z);
        WestCaveLeftX = -44.81f;
        WestCaveRightX = -26f;
        cave.SetActive(false);
        caveScene = false;
    }

    public void SetPlayer(Player p)
    {
        player = p;
    }

    // Update is called once per frame
    void Update()
    {
        targetSpeed = puzzleCompletionBoost + exitSpeed + speed * player.NumWordsPlaced();

            workingSpeed = workingSpeed + (targetSpeed - workingSpeed) / (targetSpeed > workingSpeed ? 60f : 300f);

        float leftLoop = (caveScene ? WestCaveLeftX : leftX);
        if (transform.position.x > leftLoop)
        {

            if (!caveScene && transform.position.x < WestCaveRightX || transform.position.x > EastCaveLeftX)
            {
                if(transform.position.x < WestCaveRightX)
                {
                    // first jump to the east
                    float newSubX = transform.position.x + EastCaveRightX - WestCaveRightX;
                    transform.position = new Vector3(newSubX, transform.position.y, transform.position.z);
                }
                // if we're still in the cave and we shouldn't be, get out of there!
                //exitSpeed += 0.00002f;
                //Debug.Log("increasing speed by " + exitSpeed);
            } else
            {
                exitSpeed = 0f;
            }

                transform.position = new Vector3(transform.position.x - workingSpeed, transform.position.y, transform.position.z);

            if (!caveScene && transform.position.x < WestCaveRightX)
            //if (transform.position.x < WestCaveRightX)
            {
                // exiting cave
                Debug.Log("actually looping to the east cave now");
                float newSubX = transform.position.x + EastCaveRightX - WestCaveRightX;
                transform.position = new Vector3(newSubX, transform.position.y, transform.position.z);

                
                cave.transform.position = new Vector3(-26f, cave.transform.position.y, cave.transform.position.z);
                WestCaveLeftX = -44.81f;
                WestCaveRightX = -26f;
                cave.SetActive(false);
                caveScene = false;
            }
        }
        else if (caveScene)
        {
            Debug.Log("loop within cave");
            float newSubX = transform.position.x + WestCaveRightX - WestCaveLeftX;
            transform.position = new Vector3(newSubX, transform.position.y, transform.position.z);

            // the fish loop around with the sub
            foreach (Transform child in GameObject.Find("fish").transform)
            {
                float newX = child.position.x + WestCaveRightX - WestCaveLeftX;

                if (newX > WestCaveRightX + 1f)
                {
                    newX -= WestCaveRightX - WestCaveLeftX;
                }
                child.position = new Vector3(newX, child.position.y, child.position.z);
            }
        } else
        {
            if (transform.position.x < WestCaveRightX)
            {
                // exiting cave
                Debug.Log("looping to the east cave now");
                float newSubX = transform.position.x + EastCaveRightX - WestCaveRightX;
                transform.position = new Vector3(newSubX, transform.position.y, transform.position.z);


                cave.transform.position = new Vector3(-26f, cave.transform.position.y, cave.transform.position.z);
                WestCaveLeftX = -44.81f;
                WestCaveRightX = -26f;
                cave.SetActive(false);
            } else
            {
                Debug.Log("loop in open sea");
                GameObject volcano = GameObject.Find("volcano");
                if(volcano != null) volcano.SetActive(false);
                float newSubX = transform.position.x + rightX - leftX;
                transform.position = new Vector3(newSubX, transform.position.y, transform.position.z);
            }
             
            // the fish loop around with the sub
            foreach (Transform child in GameObject.Find("fish").transform)
            {
                float newX = child.position.x + rightX - leftX;
                if (newX > RightEdge)
                {
                    newX -= RightEdge - LeftEdge;
                }
                child.position = new Vector3(newX, child.position.y, child.position.z);
            }
        }
            
    }
}
