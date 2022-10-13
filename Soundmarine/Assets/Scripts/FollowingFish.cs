using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingFish : MonoBehaviour
{
    public Transform theSub;
    public Generator generator;
    [Range (0,50)]
    public int numFollowing = 0;
    

    private List<Fish> followingFish = new();
    private List<Fish> freeFish = new();
    private float minY = -0.05f;
    private float maxY = 0.09f;
    private float minX = 0.05f;
    private float maxX = 0.16f;
    private float frontMinZ = -0.06f;
    private float frontMaxZ = -0.03f;
    private float backMinZ = 0.03f;
    private float backMaxZ = 0.06f;
    private SubControl subControl;
    private GameObject sub;
    private bool endGame;

    private int BUMP_NUMBER = 9;

    // Start is called before the first frame update
    void Start()
    {
        sub = GameObject.Find("miniSub");
        subControl = sub.GetComponent<SubControl>();
    }

    public void EndGame()
    {
        endGame = true;
    }

    public void More()
    {
        numFollowing += BUMP_NUMBER;
    }

    public void Fewer()
    {
        numFollowing = Mathf.Max(0, numFollowing - BUMP_NUMBER);
    }

    public void ReleaseAll()
    {
        foreach(Fish f in followingFish)
        {
            f.Rise();
        }
        numFollowing = 0;
    }

    // Update is called once per frame
    void Update()
    {
        while(followingFish.Count > numFollowing)
        {
            Release(followingFish[0]);
        }
        while(followingFish.Count < numFollowing)
        {
            // to do: re-collect free fish here when they exist instead of spawning them 
            Fish newFish = generator.SpawnSingleFish(theSub);
            newFish.transform.localScale = newFish.transform.localScale * Random.Range(0.15f, 0.35f);
            Vector3 subPos = theSub.position;
            //newFish.SpawnAt(subPos.x + Random.Range(minX, maxX), subPos.y + Random.Range(minY, maxY), subPos.z + Random.Range(minZ, maxZ), -1, 0f);
            float targetZ = Random.Range(0f, 1f) > 0.5f ? Random.Range(frontMinZ, frontMaxZ) : Random.Range(backMinZ, backMaxZ);
            if (endGame)
            {
                targetZ = Random.Range(backMinZ, backMaxZ);
                minX = -0.25f;
                maxX = 0.25f;
                minY = -0.1f;
                maxY = 0.2f;
            }
            newFish.SpawnAt(Random.Range(minX, maxX), Random.Range(minY, maxY), targetZ, -1, 0f);
            newFish.Follow(true);
            newFish.name = "fishy" + followingFish.Count;
            followingFish.Add(newFish);
        }
    }

    public void Release(Fish f)
    {
        followingFish.Remove(f);
        freeFish.Add(f);
        
        f.transform.SetParent(GameObject.Find("fish").transform);
        Vector3 newSpawnPos = f.transform.localPosition;
        f.Follow(false);
        f.SpawnAt(newSpawnPos.x, newSpawnPos.y, newSpawnPos.z, -1, subControl.GetSpeed()*sub.transform.localScale.x);
        f.Release();
    }

    public void Dance()
    {
        Debug.Log("now dance!");
        foreach(Fish f in followingFish)
        {
            f.Dance();
        }
    }
}
