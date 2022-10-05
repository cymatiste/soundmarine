using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubControl : MonoBehaviour
{

    public float speed = 0.00001f;
    public Player player;

    // scenery edges (put these in a separate data class so they're not repeated in Fish etc.)
    private float LeftEdge = -22f;
    private float RightEdge = 2f;

    private float leftX = -21f;
    private float rightX = -2.6f;
    float runLength;


    // Start is called before the first frame update
    void Start()
    {
        runLength = rightX - leftX;
    }

    // Update is called once per frame
    void Update()
    {
        

        if (transform.position.x > leftX)
        {
            // move left at a speed determined by the number of words placed
            transform.position = new Vector3(transform.position.x-speed*player.NumWordsPlaced(), transform.position.y, transform.position.z);
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
