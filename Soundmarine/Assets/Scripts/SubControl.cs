using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubControl : MonoBehaviour
{

    public float speed = 0.00001f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x > -21f)
        {
            transform.position = new Vector3(transform.position.x-speed, transform.position.y, transform.position.z);
        }
    }
}
