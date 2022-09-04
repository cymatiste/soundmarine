using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public List<GameObject> fishPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        SpawnFish();
    }

    public void SpawnFish()
    {
        for (int i = 0; i < fishPrefabs.Count; i++)
        {
            //GameObject newFish = Instantiate(fishPrefabs[Random.Range(0,fishPrefabs.Count)]);
            GameObject newFish = Instantiate(fishPrefabs[i], GameObject.Find("fish").transform);
            newFish.GetComponent<Fish>().Spawn(-1);
        }
    }

}
