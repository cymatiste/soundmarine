using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    private float RightEdge = 2f;
    private float LeftEdge = -22f;
    private float TopEdge = -0.5f;
    private float BottomEdge = 0.5f;
    private float FrontEdge = -3.7f;
    private float BackEdge = 3.7f;
    private int dir = -1;
    private float speed = 0.0001f;
    private float baselineY;
    private float baselineZ;
    private float sineAdjust;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = transform.localScale * Random.Range(0.5f, 2f);
        baselineY = transform.position.y;
        baselineZ = transform.position.z;
        sineAdjust = Random.Range(0f,5f);

    }

    public void SpawnAt(float spawnX, float spawnY, float spawnZ, int spawnDir)
    {
        transform.position = new Vector3(spawnX, spawnY, spawnZ);
        transform.localScale = new Vector3(transform.localScale.x * spawnDir, transform.localScale.y, transform.localScale.z);
        baselineY = spawnY;
        baselineZ = spawnZ;
        dir = spawnDir;
    }

    public void Spawn(int spawnDir)
    {
        SpawnAt(spawnDir < 0 ? RightEdge : LeftEdge, Random.Range(TopEdge, BottomEdge), Random.Range(FrontEdge, BackEdge), spawnDir);
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
            transform.position = new Vector3(transform.position.x + speed*dir, baselineY + 0.03f*Mathf.Sin(transform.position.x+sineAdjust), transform.position.z);
        }
    }
}
