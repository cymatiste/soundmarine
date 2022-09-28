using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    private int dir = -1;
    private float speed = 0.0001f;
    private float RightEdge = 2f;
    private float LeftEdge = -22f;
    private float baselineX;
    private float baselineY;
    private float baselineZ;
    private float sineAdjust;
    private float yVariance;
    private float followBobSpeed;
    public float tiltVariance;
    private float tiltAdjust;
    private bool following = false;
    private bool entering = false;

    
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = transform.localScale * Random.Range(0.75f, 1.25f);
        //baselineX = transform.position.x;
        //baselineY = transform.position.y;
        //baselineZ = transform.position.z;
        sineAdjust = Random.Range(0f,5f);
        yVariance = following ? 0.01f : Random.Range(0.1f, 0.3f);
        followBobSpeed = Random.Range(0.5f, 2f);
        tiltVariance = 0.003f;// Random.Range(5f, 10f);
        tiltAdjust = Random.Range(0f, 5f);
    }

    public void SpawnAt(float spawnX, float spawnY, float spawnZ, int spawnDir, float spawnSpeed)
    {
        transform.localPosition = new Vector3(spawnX, spawnY, spawnZ);
        transform.localScale = new Vector3(transform.localScale.x * spawnDir, transform.localScale.y, transform.localScale.z);
        baselineX = spawnX;
        baselineY = spawnY;
        baselineZ = spawnZ;
        dir = spawnDir;
        if (!following)
        {
            speed = spawnSpeed;
        }
    }

    public void Follow(bool toFollow)
    {
        following = toFollow;
        entering = toFollow;
        yVariance = 0.01f;
        speed = 0f;
        // start off-screen and swim in
        transform.localPosition = new Vector3(transform.localPosition.x + Random.Range(0.3f,0.6f), transform.localPosition.y, transform.localPosition.z);
    }


    // Update is called once per frame
    void Update()
    {
        if (following || entering)
        {
            Bob();
        }  
        else 
        {
            if (transform.position.x < LeftEdge)
            {
                transform.position = new Vector3(RightEdge, transform.position.y, transform.position.z);
            }
            else if (transform.position.x > RightEdge)
            {
                transform.position = new Vector3(LeftEdge, transform.position.y, transform.position.z);
            }
            else
            {
                Bob();
            }
        }
        if(entering && Mathf.Abs(transform.localPosition.x - baselineX) < 0.001f)
        {
            entering = false;
            Debug.Log(gameObject.name + " arrived.");
        }
    }

   
    private void Bob()
    {
        float targetY = baselineY + yVariance * Mathf.Sin(Time.time * (following ? followBobSpeed : 0.1f) + sineAdjust);
        float targetZ = transform.localPosition.z;// + 0.0001f * Mathf.Sin(transform.position.x);
        if(following || entering)
        {
            float targetX = entering
            ? Mathf.Max(baselineX, transform.localPosition.x - Mathf.Min(0.0005f, (transform.localPosition.x - baselineX) / 200f))
            : baselineX;// + 0.005f*Mathf.Sin(Time.time * followBobSpeed/200f + sineAdjust*5f) 

            transform.localPosition = new Vector3(targetX, targetY, targetZ);
        } else
        {
            transform.localPosition = new Vector3(transform.position.x + (speed * dir), targetY, targetZ);
        }
        
        

        if (following) {
            //Debug.Log(gameObject.name + " bobbing from baselineY "+baselineY+" with yVariance " + yVariance);
        }

        transform.Rotate(0f, tiltVariance * Mathf.Sin(Time.time * (100000f * speed) + tiltAdjust), 0f);
    }
}
