using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Color color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f,1f), UnityEngine.Random.Range(0f, 1f), 1f);
        GetComponent<Renderer>().material.SetColor("_Color", color);
            ;
    }
}
