using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleWobble : MonoBehaviour
{
    public Transform wobbler;
    public bool vertical = true;
    public bool horizontal = true;
    public float amount = 0.002f;
    public float speed = 1;

    private Vector3 centre;
    private int xDir;
    private int yDir;
    private float amountVariation;
    private float speedVariation;

    // Start is called before the first frame update
    void Start()
    {
        centre = wobbler.localPosition;
        xDir = Random.Range(0, 1f) > 0.5f ? 1 : -1;
        yDir = Random.Range(0, 1f) > 0.5f ? 1 : -1;
        amountVariation = Random.Range(0.5f, 1.5f);
        speedVariation = Random.Range(1, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        float workingAmt = amount * amountVariation;
        float workingSpeed = speed * speedVariation;
        float xWobble = horizontal ? xDir * workingAmt * Mathf.Cos(Time.time * workingSpeed) : 0;
        float yWobble = vertical ? yDir * workingAmt * Mathf.Sin(Time.time * workingSpeed) : 0;
        wobbler.localPosition = new Vector3(centre.x + xWobble, centre.y + yWobble, centre.z);
    }

    public void ResetPos()
    {
        wobbler.localPosition = centre;
    }
}
