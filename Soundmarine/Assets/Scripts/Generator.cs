using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public List<GameObject> fishPrefabs;

    private float RightEdge = 2f;
    private float LeftEdge = -22f;
    private float TopEdge = -0.5f;
    private float BottomEdge = 0.5f;
    private float FrontEdge = -3.7f;
    private float BackEdge = 3.7f;
    
    // Start is called before the first frame update
    void Start()
    {
        SpawnFish();

    }

    public void SpawnFish()
    {
        for (int i = 0; i < fishPrefabs.Count; i++)
        {
            int schoolSize = Random.Range(5, 15);
            int schoolDir = Random.Range(0f, 1f) > 0.5f ? 1 : -1;
            float schoolSpeed = Random.Range(0.0002f, 0.0005f);
            float schoolX = Random.Range(LeftEdge, RightEdge);
            float schoolY = Random.Range(TopEdge, BottomEdge);
            float schoolZ = Random.Range(FrontEdge, BackEdge);
            float schoolXSpread = Mathf.Abs(RightEdge - LeftEdge) / 10;// 40;
            float schoolYSpread = Mathf.Abs(TopEdge - BottomEdge) / 10;
            float schoolZSpread = Mathf.Abs(FrontEdge - BackEdge) / 20;

            for (int j=0; j< schoolSize; j++)
            {
                GameObject newFish = Instantiate(fishPrefabs[i], GameObject.Find("fish").transform);
                newFish.GetComponent<Fish>().SpawnAt(schoolX + Random.Range(0f,schoolXSpread) - schoolXSpread/2, schoolY + Random.Range(0f,schoolYSpread)-schoolYSpread/2, schoolZ + Random.Range(0f,schoolZSpread)-schoolZSpread/2, schoolDir, schoolSpeed);
            }
            
        }
    }

    public Fish SpawnSingleFish(Transform fishParent)
    {
        GameObject newFish = Instantiate(fishPrefabs[Random.Range(0,fishPrefabs.Count)], fishParent);
        return newFish.GetComponent<Fish>();
    }

}
