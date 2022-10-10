using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubControl : MonoBehaviour
{
    public Player player;

    // scenery edges (put these in a separate data class so they're not repeated in Fish etc.)
    private float LeftEdge = -22f;
    private float RightEdge = 2f;

    private float leftX = -21f;
    private float rightX = -2.6f;

    private float speed = 0.0003f;
    private float targetSpeed;
    private float workingSpeed;
    
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

    // Update is called once per frame
    void Update()
    {
        targetSpeed = speed * player.NumPuzzlesComplete() + speed * player.NumWordsPlaced();

        workingSpeed = workingSpeed + (targetSpeed - workingSpeed) / 60f;

        if (transform.position.x > leftX)
        {
            //Debug.Log("workingSpeed: " + workingSpeed+ ", targetSpeed: " + targetSpeed + ", "+ player.NumWordsPlaced()+" words placed.");
            // move left at a speed determined by the number of words placed
            transform.position = new Vector3(transform.position.x - workingSpeed, transform.position.y, transform.position.z);
            //transform.position = new Vector3(transform.position.x - speed * player.NumWordsCorrect(), transform.position.y, transform.position.z);

        } else
        {
            // the sub loops around from the left edge of the screen to a matching position on the right
            transform.position = new Vector3(rightX, transform.position.y, transform.position.z);

            // the fish loop around too
            foreach(Transform child in GameObject.Find("fish").transform)
            {
                float newX = child.position.x + rightX - leftX;
                if(runLength > RightEdge)
                {
                    newX -= RightEdge - LeftEdge;
                }
                child.position = new Vector3(newX, child.position.y, child.position.z);
            }
        }
    }
}
