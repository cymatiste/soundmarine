using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IntroFade : MonoBehaviour
{
    public UnityEngine.UI.Image splashScreen;

    private bool splashFading = true;

    // Start is called before the first frame update
    void Awake()
    {
        splashScreen.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (splashFading)
        {
            Color color = splashScreen.color;
            color.a -= 0.005f;
            splashScreen.color = color;

            if (color.a == 0f)
            {
                splashFading = false;
                splashScreen.enabled = false;
            }
        }

    }
}