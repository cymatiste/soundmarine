using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubControl : MonoBehaviour
{
    Transform sub;
    Vector3 subPos;

    // Start is called before the first frame update
    void Start()
    {
        sub = transform.Find("miniSub_contents");
        subPos = sub.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(subPos.x, subPos.y + 0.002f*Mathf.Sin(Time.time), subPos.z);
    }
}
