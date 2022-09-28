using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public VolcanoSequence volcanoSequence;

    public AudioSource volcanoLoop;
    public AudioSource shuttleLoop;
    public AudioSource submarineLoop;

    public List<AudioClip> instrument_puzzle1;

    public GameObject diverBall;
    public GameObject dancerSeat;
    public GameObject whale;

    public bool startInVolcano = false;

    private Vector3 volcanoShuttlePos = new Vector3(0, -4.5f, -0.67f);
    // these one for straight jumps:
    //private Vector3 bathyspherePos = new Vector3(0,-2.6f,-0.67f);
    // private Vector3 shuttleCameraPos = new Vector3(0, 0.01f, -0.67f);
    // this one for LeanTween (???):
    private Vector3 bathyspherePos = new Vector3(0,-2.63f,-0.67f);
    private Vector3 shuttleCameraPos = new Vector3(0.018f, 0.02f, -0.67f);

    private Vector3 dockPos = new Vector3(-0.0078f, 2.6f, 0f);

    private Player player;
    private GameObject puzzle;

    // Start is called before the first frame update
    void Start()
    {
        player = gameObject.GetComponent<Player>();
        puzzle = GameObject.Find("puzzle");
        whale.SetActive(false);

        Camera.main.transform.localPosition = shuttleCameraPos;

        if (!startInVolcano)
        {
            shuttleLoop.volume = 0f;
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
            volcanoLoop.Play();
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
        shuttleLoop.volume = 0f;
        shuttleLoop.Play();
        volcanoSequence.Exit();
        LeanTween.moveLocal(Camera.main.gameObject, shuttleCameraPos, 10f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(SetUpShuttlePuzzle);
        LeanTween.rotateY(Camera.main.gameObject, -1.34f, 10f);
        float ballTargetY = diverBall.transform.position.y + (shuttleCameraPos.y - bathyspherePos.y);
        LeanTween.moveLocal(diverBall, dockPos, 10f).setEase(LeanTweenType.easeInOutQuad);
    }

    public void SetUpShuttlePuzzle()
    {
        Debug.Log("do it");
        Camera.main.transform.localPosition = shuttleCameraPos;
        diverBall.SetActive(false);
        dancerSeat.SetActive(true);
    }


    public void ShowButtons()
    {
        gameObject.GetComponent<VolcanoSequence>().ShowButtons(true);
    }

    public void PuzzleComplete()
    {
        LeanTween.moveZ(puzzle, puzzle.transform.position.z - 0.25f, 1f).setEaseInCirc();
        LeanTween.moveY(puzzle, puzzle.transform.position.y + 1f, 8f).setDelay(2f).setEase(LeanTweenType.easeInCirc).setOnComplete(GameOver);

    }

    public void GameOver()
    {
        Debug.Log("GAME OVER");
        GameObject cam = Camera.main.gameObject;
        puzzle.SetActive(false);
        whale.SetActive(true);
        LeanTween.moveLocalX(whale, whale.transform.localPosition.x -8f, 12f);
        //LeanTween.moveY(cam, 0.05f, 1).setEaseInSine();
        //LeanTween.moveZ(cam, -0.5f, 1).setEaseInSine();
        //LeanTween.rotateX(cam, 2.43f, 1).setEaseInSine();
        StartCoroutine(ShowCreditsAfter(15f));
    }
    private IEnumerator ShowCreditsAfter(float delay)
    { 
        yield return new WaitForSeconds(delay);
        Debug.Log("that's all she wrote");
        SceneManager.LoadScene("Credits");
    }


    // Update is called once per frame
    void Update()
    {
        if(shuttleLoop.isPlaying && shuttleLoop.volume < 1f)
        {
            shuttleLoop.volume = shuttleLoop.volume + 0.001f;
            volcanoLoop.volume = Mathf.Max(0f, volcanoLoop.volume - 0.001f);
        }
    }


}
