using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    private int dir = -1;
    private float speed = 0.0001f;
    private float RightEdge = 2f;
    private float LeftEdge = -22f;
    private float baselineY;
    private float baselineZ;
    private float sineAdjust;
    private float yVariance;
    private float tiltVariance;
    private float tiltAdjust;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = transform.localScale * Random.Range(0.75f, 1.25f);
        baselineY = transform.position.y;
        baselineZ = transform.position.z;
        sineAdjust = Random.Range(0f,5f);
        yVariance = Random.Range(0.1f, 0.3f);
        tiltVariance = 0.2f;// Random.Range(5f, 10f);
        tiltAdjust = Random.Range(0f, 5f);
    
    }

    public void SpawnAt(float spawnX, float spawnY, float spawnZ, int spawnDir, float spawnSpeed)
    {
        transform.position = new Vector3(spawnX, spawnY, spawnZ);
        transform.localScale = new Vector3(transform.localScale.x * spawnDir, transform.localScale.y, transform.localScale.z);
        baselineY = spawnY;
        baselineZ = spawnZ;
        dir = spawnDir;
        speed = spawnSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < LeftEdge)
        {
            transform.position = new Vector3(RightEdge, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > RightEdge) 
        {
            transform.position = new Vector3(LeftEdge, transform.position.y, transform.position.z);
        } else      
        {
            transform.position = new Vector3(transform.position.x + speed*dir, baselineY + yVariance*Mathf.Sin(transform.position.x+sineAdjust), transform.position.z + 0.0001f * Mathf.Sin(transform.position.x));
            transform.Rotate(0f, tiltVariance * Mathf.Sin(Time.time + tiltAdjust), 0f);
        }
    }
}
