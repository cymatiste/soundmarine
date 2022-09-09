using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFade : MonoBehaviour
{
    public UnityEngine.UI.Image btn1;
    public UnityEngine.UI.Image btn2;
    private float alpha = 0;
    private bool fading = true;

    // Start is called before the first frame update
    void Start()
    {
        Color col1 = btn1.color;
        Color col2 = btn2.color;
        col1.a = 0;
        col2.a = 0;
        btn1.color = col1;
        btn2.color = col2;
    }

    // Update is called once per frame
    void Update()
    {
        if (fading)
        {
            alpha += 0.002f;
            Color col1 = btn1.color;
            Color col2 = btn2.color;
            col1.a = alpha;
            col2.a = alpha;
            btn1.color = col1;
            btn2.color = col2;

            if (alpha >= 1f)
            {
                fading = false;
            }
        }
        
    }
}
