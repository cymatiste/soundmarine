using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public VolcanoSequence volcanoSequence;

    public AudioSource volcanoLoop;
    public AudioSource shuttleLoop;
    public AudioSource submarineLoop;

    public List<AudioClip> instrument_puzzle1;


    public bool showBall = false;

    private Vector3 volcanoShuttlePos = new Vector3(0, -4.5f, -0.67f);
    // this one for straight jumps:
    //private Vector3 bathyspherePos = new Vector3(0,-2.6f,-0.67f);
    // this one for LeanTween (???):
    private Vector3 bathyspherePos = new Vector3(0,-2.63f,-0.67f);
    private Vector3 shuttleCameraPos = new Vector3(0, 0.01f, -0.67f);

   
    

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.transform.localPosition = shuttleCameraPos;

        if (!showBall)
        {
            shuttleLoop.Play();
            volcanoSequence.Exit();
            volcanoSequence.enabled = false;
        } 

        GameObject wordsParent = GameObject.Find("words");
        Debug.Log("wordsParent: " + wordsParent);
        int clipIndex = 0;
        foreach (Transform word in wordsParent.transform)
        {
            if(word.GetComponent<AudioSource>() != null)
            {
                word.GetComponent<AudioSource>().clip = instrument_puzzle1[clipIndex % instrument_puzzle1.Count];
                clipIndex++;
            }
            
        }

        if (showBall)
        {
            //Camera.main.transform.localPosition = bathyspherePos;

            Camera.main.transform.localPosition = volcanoShuttlePos;
            LeanTween.moveY(Camera.main.gameObject, bathyspherePos.y, 10f).setEase(LeanTweenType.easeOutCirc).setOnComplete(ShowButtons);
            
        }
        else
        {
            Camera.main.transform.localPosition = shuttleCameraPos;
        }

    }

    public void ShowButtons()
    {
        gameObject.GetComponent<VolcanoSequence>().ShowButtons();
    }


    // Update is called once per frame
    void Update()
    {
        
    }


}
