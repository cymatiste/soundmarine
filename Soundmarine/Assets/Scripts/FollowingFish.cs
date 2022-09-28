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
    private float minZ = -0.0001f;
    private float maxZ = 0.0001f;
    private int BUMP_NUMBER = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void More()
    {
        numFollowing += BUMP_NUMBER;
    }

    public void Fewer()
    {
        numFollowing = Mathf.Max(0, numFollowing - BUMP_NUMBER);
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
            newFish.transform.localScale = newFish.transform.localScale * Random.Range(0.3f, 0.5f);
            Vector3 subPos = theSub.position;
            //newFish.SpawnAt(subPos.x + Random.Range(minX, maxX), subPos.y + Random.Range(minY, maxY), subPos.z + Random.Range(minZ, maxZ), -1, 0f);
            newFish.SpawnAt(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ), -1, 0f);
            newFish.Follow(true);
            newFish.name = "fishy" + followingFish.Count;
            followingFish.Add(newFish);
        }
    }

    private void Release(Fish f)
    {
        followingFish.Remove(f);
        freeFish.Add(f);
        Vector3 newSpawnPos = f.transform.position;
        f.Follow(false);
        f.transform.SetParent(GameObject.Find("fish").transform);
        f.SpawnAt(newSpawnPos.x, newSpawnPos.y, newSpawnPos.z, 1, 0.0001f);
    }
}
