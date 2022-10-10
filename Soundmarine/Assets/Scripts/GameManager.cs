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
    public List<AudioClip> instrument_puzzle2;
    public List<AudioClip> instrument_puzzle3;
    public List<AudioClip> instrument_puzzle4;
    public List<AudioClip> instrument_puzzle5;

    public GameObject diverBall;
    public GameObject dancerSeat;
    private GameObject whale1;
    private GameObject whale2;

    public List<GameObject> wordSets;

    public bool startInVolcano = false;


    private Vector3 volcanoShuttlePos = new Vector3(0, -4.5f, -0.67f);
    // these one for straight jumps:
    //private Vector3 bathyspherePos = new Vector3(0,-2.6f,-0.67f);
    // private Vector3 shuttleCameraPos = new Vector3(0, 0.01f, -0.67f);
    // this one for LeanTween (???):
    private Vector3 bathyspherePos = new Vector3(0, -2.63f, -0.67f);
    private Vector3 shuttleCameraPos = new Vector3(0.018f, 0.02f, -0.67f);

    //private Vector3 dockPos = new Vector3(-0.0078f, 2.6f, 0f);
    private Vector3 dockPos = new Vector3(0f, -0.04f, 0.03f);

    private Player player;
    private GameObject puzzle;
    private GameObject puzzleWords;
    private GameObject miniSub;
    private int puzzleNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        puzzle = GameObject.Find("puzzle");
        miniSub = GameObject.Find("miniSub");
        puzzleWords = wordSets[puzzleNum];
        player = puzzle.GetComponent<Player>();
        wordSets[0].gameObject.SetActive(true);

        Camera.main.transform.localPosition = shuttleCameraPos;

        if (!startInVolcano)
        {
            shuttleLoop.volume = 0f;
            shuttleLoop.Play();
            volcanoSequence.Exit();
            volcanoSequence.enabled = false;
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

        GameObject insideBall = GameObject.Find("ball");
        GameObject cable = GameObject.Find("cable");
        Vector3 ballScale = insideBall.transform.localScale;
        Vector3 dockScale = ballScale * 0.9f;
        Vector3 cablePos = cable.transform.localPosition;
        Vector3 dockCablePos = new Vector3(-0.0055f, cablePos.y, cablePos.y + 0.06f);
        LeanTween.moveLocal(insideBall, dockPos, 10f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.scale(insideBall, dockScale, 10f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.scaleY(cable, 0f, 9.985f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.moveLocal(cable, dockCablePos, 10f).setEase(LeanTweenType.easeInOutQuad);
    }

    public void SetUpShuttlePuzzle()
    {
        Debug.Log("SetUpShuttlePuzzle " + puzzleNum);
        //Camera.main.transform.localPosition = shuttleCameraPos;
        diverBall.SetActive(false);
        dancerSeat.SetActive(true);
        for (int i = 0; i < wordSets.Count; i++)
        {
            wordSets[i].gameObject.SetActive(i == puzzleNum);
        }

        int clipIndex = 0;

        List<AudioClip> instrument;
        switch (puzzleNum)
        {
            case 0: instrument = instrument_puzzle1; break;
            case 1: instrument = instrument_puzzle2; break;
            case 2: instrument = instrument_puzzle3; break;
            case 3: instrument = instrument_puzzle4; break;
            case 4: instrument = instrument_puzzle5; break;
            default: instrument = instrument_puzzle2; break;
        }



        puzzleWords = wordSets[puzzleNum];
        puzzleWords.SetActive(true);
        foreach (Transform word in puzzleWords.transform)
        {
            word.GetComponent<Word>().Init();
            if (word.GetComponent<AudioSource>() != null)
            {
                word.GetComponent<AudioSource>().clip = instrument[clipIndex % instrument.Count];
                clipIndex++;
            }

        }
        player.InitPuzzle(puzzleNum);
    }


    public void ShowButtons()
    {
        Debug.Log("please show buttons?");
        gameObject.GetComponent<VolcanoSequence>().ShowButtons(true);
    }

    public void PuzzleComplete()
    {
        Debug.Log("PuzzleComplete!");

        foreach (Transform t in puzzleWords.transform)
        {
            // words in the completed puzzle should no longer be draggable
            t.gameObject.tag = "nodrag";
            // hide all the red herring words
            if (t.GetComponent<Word>() != null && t.GetComponent<Word>().GetSpot() == null)
            {
                t.gameObject.SetActive(false);
            }
        }


        LeanTween.moveZ(puzzleWords, puzzleWords.transform.position.z - 0.25f, 1f).setEaseInCirc();
        LeanTween.moveY(puzzleWords, puzzleWords.transform.position.y + 1f, 8f).setDelay(13f).setEase(LeanTweenType.easeInCirc);
        StartCoroutine(FishFollowPuzzleIn(13f));

        GameObject.Find("GameManager").GetComponent<FollowingFish>().Dance();

    }

    public IEnumerator FishFollowPuzzleIn(float secs)
    {
        yield return new WaitForSeconds(secs);
        GameObject.Find("GameManager").GetComponent<FollowingFish>().ReleaseAll();
        Advance();
    }

    private void WhalePass()
    {
        // one whale floats by
        whale1 = (GameObject)Instantiate(Resources.Load("whale"), miniSub.transform);
        whale1.SetActive(true);
        whale1.transform.localPosition = new Vector3(2.5f, whale1.transform.localPosition.y, whale1.transform.localPosition.z);
        LeanTween.moveLocalX(whale1, -2.5f, 16f);
        StartCoroutine(ContinueAfter(18f));
    }

    private void WhalePod()
    {
        // more whales
    }

    private void EnterCave()
    {
        // enter cave
    }

    private void ExitCave()
    {
        // exit cave
    }

    private void Finale()
    {
        // finale
    }




    public void Advance()
    {
        WhalePass();
        
    }
    private IEnumerator ContinueAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        whale1.SetActive(false);
        if (puzzleNum == 4)
        {
            Debug.Log("GAME OVER");
            SceneManager.LoadScene("Credits");
        } else
        {
            Debug.Log("please set up next puzzle: " + puzzleNum);
            puzzleNum++;
            SetUpShuttlePuzzle();
        }
        
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
