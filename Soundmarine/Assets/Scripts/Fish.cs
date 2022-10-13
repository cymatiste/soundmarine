using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    private int dir = -1;
    private float speed = 0.0001f;
    private float targetSpeed = 0.0001f;
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
    private bool dancing = false;
    private bool rising = false;
    private bool released = false;
    private bool wobbling = false;
    private float wobbleAmount = 0f;
    private float wobbleSpeed = 0f;
    private float wobbleHeight = 0f;
    private float wobbleStartTime = 0f;
    private float releaseAcceleration;
    private float releaseX;
    private Vector3 baseScale;

    //private Transform guide;


    // Start is called before the first frame update
    void Start()
    {
        // GameObject emptyGO = new GameObject();
        //guide = emptyGO.transform;
        transform.localScale = transform.localScale * Random.Range(0.75f, 1.25f);
        baseScale = transform.localScale;
        //baselineX = transform.position.x;
        //baselineY = transform.position.y;
        //baselineZ = transform.position.z;
        sineAdjust = Random.Range(0f, 5f);
        yVariance = following ? 0.01f : Random.Range(0.1f, 0.3f);
        followBobSpeed = Random.Range(0.5f, 2f);
        tiltVariance = 0.003f;// Random.Range(5f, 10f);
        tiltAdjust = Random.Range(0f, 5f);
        releaseAcceleration = Random.Range(0.01f, 0.03f);
    }

    public void Dance()
    {
        //Debug.Log("   .dancing!");
        //dancing = true;
    }

    public void Release()
    {
        //following = false;
        //entering = false;
        releaseX = transform.localPosition.x;
        released = true;
        targetSpeed = GameObject.Find("miniSub").GetComponent<SubControl>().GetSpeed() * 1.5f;
    }

    public void Rise()
    {
        rising = true;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void SpawnAt(float spawnX, float spawnY, float spawnZ, int spawnDir, float spawnSpeed)
    {
        //Debug.Log("spawning " + gameObject.name + " at x " + spawnX + ", y " + spawnY + ", z " + spawnZ + ", dir " + spawnDir + ", speed " + spawnSpeed);
        transform.localPosition = new Vector3(spawnX, spawnY, spawnZ);
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x)* spawnDir, transform.localScale.y, transform.localScale.z);
        baseScale = transform.localScale;
        if (wobbling)
        {
            transform.localScale = new Vector3(Random.Range(0.7f,0.9f) * baseScale.x, Random.Range(0.7f, 0.9f) * baseScale.y,baseScale.z);
            LeanTween.scale(gameObject, baseScale, 0.5f);
        }
        
        baselineX = spawnX;
        baselineY = spawnY;
        baselineZ = spawnZ;
        dir = spawnDir;
        if (!following)
        {
            targetSpeed = spawnSpeed;
            speed = spawnSpeed;
        }
    }

    public void Follow(bool toFollow)
    {
        following = toFollow;
        entering = toFollow;
        yVariance = 0.01f;
        targetSpeed = 0f;
        speed = 0f;
        // start off-screen and swim in
        transform.localPosition = new Vector3(transform.localPosition.x + Random.Range(0.3f,0.6f), transform.localPosition.y, transform.localPosition.z);
    }


    // Update is called once per frame
    void Update()
    {
        speed = speed + (targetSpeed - speed) / 120f;
        if (released)
        {
            Debug.Log(gameObject.name + " released! speed: " + speed + ", x "+transform.position.x);

            //targetSpeed += releaseAcceleration;
            if(Mathf.Abs(transform.localPosition.x - releaseX) > 3f)
            {
                //Debug.Log(gameObject.name + "far enough away and deactivating ( "+transform.position.x+" VS "+releaseX+" )");
                gameObject.SetActive(false);
            }
        }
        if (following || entering)
        {
            Bob();
        }  
        else 
        {
            if (transform.position.x < LeftEdge)
            {
                if (wobbling)
                {
                    if ((Time.time - wobbleStartTime) < (Mathf.PI * 2) / wobbleSpeed)
                    {

                        wobbleAmount = wobbleHeight * Mathf.Sin((Time.time - wobbleStartTime) * wobbleSpeed);
                    }
                    else
                    {
                        wobbling = false;
                    }

                }
                transform.position = new Vector3(RightEdge, transform.position.y+wobbleAmount, transform.position.z);
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
            //Debug.Log(gameObject.name + " arrived.");
        }
    }

    public void Wobble()
    {
        wobbling = true;
        wobbleStartTime = Time.time;
        wobbleHeight = Random.Range(0.001f, 0.003f)*(Random.Range(0.0f,1.0f) >0.5 ? -1 : 1);
        wobbleSpeed = Random.Range(3f, 10f);
        
        GameObject.Find("GameManager").GetComponent<FollowingFish>().Release(gameObject.GetComponent<Fish>());
    }
   
    private void Bob()
    {
        float targetY = wobbleAmount + baselineY + yVariance * Mathf.Sin(Time.time * (following ? followBobSpeed : 0.1f) + sineAdjust) + (rising ?  speed : 0);
        float targetZ = transform.localPosition.z;// + 0.0001f * Mathf.Sin(transform.position.x);
     
        if(following || entering)
        {
            float targetX = entering
            ? Mathf.Max(baselineX, transform.localPosition.x - Mathf.Min(0.0005f, (transform.localPosition.x - baselineX) / 200f))
            : baselineX + (speed * dir);// + 0.005f*Mathf.Sin(Time.time * followBobSpeed/200f + sineAdjust*5f) 

            //guide.position = new Vector3(targetX - 0.1f, targetY * 2f, targetZ);
            //transform.LookAt(guide);

            transform.localPosition = new Vector3(targetX, targetY, targetZ);
        } else
        {
            transform.localPosition = new Vector3(transform.position.x + (speed * dir), targetY, targetZ);
        }
        
        

        if (following) {
            //Debug.Log(gameObject.name + " bobbing from baselineY "+baselineY+" with yVariance " + yVariance);
        }

        //transform.Rotate(0f, tiltVariance * Mathf.Sin(Time.time * (100000f * speed) + tiltAdjust), 0f);
    }
}
