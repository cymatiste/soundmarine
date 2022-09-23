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

    public GameObject diverBall;
    public GameObject dancerSeat;

    public bool startInVolcano = false;

    private Vector3 volcanoShuttlePos = new Vector3(0, -4.5f, -0.67f);
    // these one for straight jumps:
    //private Vector3 bathyspherePos = new Vector3(0,-2.6f,-0.67f);
    // private Vector3 shuttleCameraPos = new Vector3(0, 0.01f, -0.67f);
    // this one for LeanTween (???):
    private Vector3 bathyspherePos = new Vector3(0,-2.63f,-0.67f);
    private Vector3 shuttleCameraPos = new Vector3(0.018f, 0.02f, -0.67f);

    private Vector3 dockPos = new Vector3(-0.0078f, 2.6f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.transform.localPosition = shuttleCameraPos;

        if (!startInVolcano)
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

        if (startInVolcano)
        {
            dancerSeat.SetActive(false);
            Camera.main.transform.localPosition = volcanoShuttlePos;
            LeanTween.moveY(Camera.main.gameObject, bathyspherePos.y, 10f).setEase(LeanTweenType.easeOutCirc).setOnComplete(ShowButtons);
            
        }
        else
        {
            Camera.main.transform.localPosition = shuttleCameraPos;
        }
    }

    public void PanToShuttle()
    {
        volcanoLoop.Play();
        volcanoSequence.Exit();
        LeanTween.moveLocal(Camera.main.gameObject, shuttleCameraPos, 10f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(SetUpShuttlePuzzle);
        float ballTargetY = diverBall.transform.position.y + (shuttleCameraPos.y - bathyspherePos.y);
        LeanTween.moveLocal(diverBall, dockPos, 10f).setEase(LeanTweenType.easeInOutQuad);
    }

    public void SetUpShuttlePuzzle()
    {
        Debug.Log("do it");
        Camera.main.transform.localPosition = shuttleCameraPos;
        volcanoLoop.Stop();
        shuttleLoop.Play();
        diverBall.SetActive(false);
        dancerSeat.SetActive(true);
    }


    public void ShowButtons()
    {
        gameObject.GetComponent<VolcanoSequence>().ShowButtons(true);
    }


    // Update is called once per frame
    void Update()
    {
        
    }


}
