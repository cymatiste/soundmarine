using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSound : MonoBehaviour
{ 
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}
