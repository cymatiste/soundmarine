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
    public GameObject whale;

    public List<GameObject> wordSets;

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
    private GameObject puzzleWords;
    
    private int puzzleNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        puzzle = GameObject.Find("puzzle");
        puzzleWords = wordSets[puzzleNum];
        player = puzzle.GetComponent<Player>();
        wordSets[0].gameObject.SetActive(true);
        whale.SetActive(false);

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
        LeanTween.moveLocal(diverBall, dockPos, 10f).setEase(LeanTweenType.easeInOutQuad);
        GameObject cable = GameObject.Find("cable");
        LeanTween.scaleY(cable, 0f, 10f).setEase(LeanTweenType.easeInOutQuad);
    }

    public void SetUpShuttlePuzzle()
    {
        Debug.Log("SetUpShuttlePuzzle "+puzzleNum);
        //Camera.main.transform.localPosition = shuttleCameraPos;
        diverBall.SetActive(false);
        dancerSeat.SetActive(true);
        for(int i=0; i<wordSets.Count; i++)
        {
            wordSets[i].gameObject.SetActive(i==puzzleNum);
        }
        
        int clipIndex = 0;
        List<AudioClip> instrument =
            (puzzleNum == 0 || puzzleNum == 3)
            ? instrument_puzzle1
            : (puzzleNum == 1 || puzzleNum == 4)
                ? instrument_puzzle2
                : instrument_puzzle3;
                  

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
        
        foreach(Transform t in puzzleWords.transform)
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
        LeanTween.moveY(puzzleWords, puzzleWords.transform.position.y + 1f, 8f).setDelay(12f).setEase(LeanTweenType.easeInCirc).setOnComplete(Advance);
        StartCoroutine(FishFollowPuzzleIn(8f));

        GameObject.Find("GameManager").GetComponent<FollowingFish>().Dance();

    }

    public IEnumerator FishFollowPuzzleIn(float secs)
    {
        yield return new WaitForSeconds(secs);
        GameObject.Find("GameManager").GetComponent<FollowingFish>().ReleaseAll();
    }

    public void Advance()
    {
        Debug.Log("whale time");
        GameObject cam = Camera.main.gameObject;
        //puzzle.SetActive(false);
        whale.SetActive(true);
        whale.transform.localPosition = new Vector3(3f, whale.transform.localPosition.y, whale.transform.localPosition.z);
        LeanTween.moveLocalX(whale, whale.transform.localPosition.x -6f, 12f);
        //LeanTween.moveY(cam, 0.05f, 1).setEaseInSine();
        //LeanTween.moveZ(cam, -0.5f, 1).setEaseInSine();
        //LeanTween.rotateX(cam, 2.43f, 1).setEaseInSine();
        StartCoroutine(ContinueAfter(15f));
    }
    private IEnumerator ContinueAfter(float delay)
    { 
        yield return new WaitForSeconds(delay);
        whale.SetActive(false);
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
