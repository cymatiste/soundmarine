using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public VolcanoSequence volcanoSequence;

    public GameObject creditsBtn;

    public AudioSource volcanoLoop;
    public AudioSource shuttleLoop;
    public AudioSource submarineLoop;

    public List<AudioClip> instrument_puzzle1;
    public List<AudioClip> instrument_puzzle2;
    public List<AudioClip> instrument_puzzle3;
    public List<AudioClip> instrument_puzzle4;
    public List<AudioClip> instrument_puzzle5;

    public List<Player> players;

    public GameObject diverBall;
    public GameObject dancerSeat;
    private GameObject whale1;
    private GameObject whale2;
    private GameObject whale3;
    private GameObject cam;

    public List<GameObject> spotSets;
    public List<GameObject> wordSets;

    public bool startInVolcano = false;

    public bool fastPlay = false;

    private Vector3 volcanoShuttlePos = new Vector3(0, -4.5f, -0.67f);
    // these one for straight jumps:
    //private Vector3 bathyspherePos = new Vector3(0,-2.6f,-0.67f);
    // private Vector3 shuttleCameraPos = new Vector3(0, 0.01f, -0.67f);
    // this one for LeanTween (???):
    private Vector3 bathyspherePos = new Vector3(0, -2.63f, -0.67f);
    private Vector3 shuttleCameraPos = new Vector3(0.018f, 0.02f, -0.67f);

    //private Vector3 dockPos = new Vector3(-0.0078f, 2.6f, 0f);
    private Vector3 dockPos = new Vector3(0f, -0.04f, 0.03f);

    private GameObject puzzle;
    private GameObject puzzleWords;
    private GameObject miniSub;
    private int puzzleNum = 0;
    private int numPuzzlesComplete = 0;

    // Start is called before the first frame update
    void Start()
    {

        creditsBtn.SetActive(false);

        puzzle = GameObject.Find("puzzle");
        miniSub = GameObject.Find("miniSub");
        cam = GameObject.Find("Main Camera");
        puzzleWords = wordSets[puzzleNum];
        miniSub.GetComponent<SubControl>().SetPlayer(players[puzzleNum]);
        Debug.Log(players.Count + " players");

        for(int i=0; i < wordSets.Count; i++)
        {
            wordSets[i].SetActive(false);
            spotSets[i].SetActive(false);
        }

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

    public Player GetActivePlayer()
    {
        return players[puzzleNum];
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
        miniSub.GetComponent<SubControl>().SetPlayer(players[puzzleNum]);
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

        players[puzzleNum].InitPuzzle(puzzleNum);
    }


    public void ShowButtons()
    {
        Debug.Log("please show buttons?");
        gameObject.GetComponent<VolcanoSequence>().ShowButtons(true);
    }

    public void PuzzleComplete()
    {
        Debug.Log("PuzzleComplete!");
        numPuzzlesComplete++;
        miniSub.GetComponent<SubControl>().SpeedBoost(numPuzzlesComplete);

        List<GameObject> wordsToHide = new();
        int t_i = 0;
        //Debug.Log(puzzleWords.name + " has " + puzzleWords.transform.childCount + " children for us to cycle through.");
        foreach (Transform t in puzzleWords.transform)
        {
            //Debug.Log(t_i + " : " + t.gameObject.name);
            // words in the completed puzzle should no longer be draggable
            t.gameObject.tag = "nodrag";
            // hide all the red herring words
            if (t.GetComponent<Word>() != null && t.GetComponent<Word>().GetSpot() == null)
            {
                t.gameObject.GetComponent<IdleWobble>().enabled = false;
                wordsToHide.Add(t.gameObject);
                LeanTween.scale(t.gameObject, 0.00001f * Vector3.zero, 0.5f).setDelay(Random.Range(0,2f));
                //Debug.Log("...hiding because " + (t.GetComponent<Word>() != null ? " a word; " : " not a word; ") + (t.GetComponent<Word>().GetSpot() == null ? " has no spot." : t.GetComponent<Word>().GetSpot().gameObject.name));
            }

            t_i++;            
        }
        StartCoroutine(HideObjectsAfter(wordsToHide, 2f));

        // words float away
        for (int i = 0; i <= puzzleNum; i++)
        {
            players[i].PlayLoop(false);
        }
        LeanTween.moveZ(puzzleWords, puzzleWords.transform.position.z - 0.25f, 1f).setEaseInCirc();
        LeanTween.moveY(puzzleWords, puzzleWords.transform.position.y + 1f, 8f).setDelay(fastPlay ? 0f : 8f).setEase(LeanTweenType.easeInCirc);
        StartCoroutine(FishFollowPuzzleIn(fastPlay ? 0f : 8f));

        gameObject.GetComponent<FollowingFish>().Dance();

    }

    private IEnumerator HideObjectsAfter(List<GameObject> objects, float delay)
    {
    foreach (GameObject g in objects)
    {
        g.transform.SetParent(g.transform.parent.parent);
    }

    yield return new WaitForSeconds(delay);
        foreach(GameObject g in objects)
        {
            g.SetActive(false);
        }
    }

    public IEnumerator FishFollowPuzzleIn(float secs)
    {
        yield return new WaitForSeconds(secs);
        gameObject.GetComponent<FollowingFish>().ReleaseAll();
        Advance();
    }

    private void WhalePass()
    {
        // one whale floats by
        whale1 = (GameObject)Instantiate(Resources.Load("whale"), miniSub.transform);
        whale1.SetActive(true);
        whale1.transform.localPosition = new Vector3(3f, whale1.transform.localPosition.y, whale1.transform.localPosition.z);
        LeanTween.moveLocalX(whale1, -3f, 16f).setOnComplete(HideWhales);
        /*
        GameObject subBody = GameObject.Find("miniSub_contents");
        LeanTween.rotateY(subBody, 45f, 2f);
        LeanTween.rotateY(subBody, 0f, 2f).setDelay(2f);
        */
        StartCoroutine(ContinueAfter(fastPlay ? 0f : 0f));
    }

    private void EnterCave()
    {
        miniSub.GetComponent<SubControl>().EnterCave(miniSub.transform.position.x);
        StartCoroutine(ContinueAfter(fastPlay ? 0f : 0f));
    }

    private void ExitCave()
    {
        miniSub.GetComponent<SubControl>().ExitCave();
        StartCoroutine(ContinueAfter(fastPlay ? 0f : 0f));
    }

    private void QuickFlip()
    {
        StartCoroutine(ContinueAfter(fastPlay ? 0f : 2f));
    }

    private void WhalePod()
    {
        StartCoroutine(BringInWhalePod());
    }

    private void Finale()
    {
        gameObject.GetComponent<FollowingFish>().EndGame();
        StartCoroutine(FullSong());
    }

    private IEnumerator FullSong()
    {
        yield return new WaitForSeconds(13f);
        // finale
        Debug.Log("finale");
        LeanTween.moveLocalZ(cam, -0.9f, 5f).setEaseInOutCirc();
        LeanTween.moveLocalY(cam, 0.02f, 5f).setEaseInOutCirc();
        players[1].loopBeats = 32;
        for (int i = 0; i < players.Count; i++)
        {

            players[i].finishedSet.gameObject.SetActive(true);
            //players[i].finishedSet.transform.localPosition = new Vector3(0, (-2 + i) * 0.03f, 0);
            players[i].InitFinal();

            wordSets[i].transform.localPosition = Vector3.zero;
            wordSets[i].SetActive(true);

            Debug.Log(" ---->  activated set " + i + ": " + players[i].finishedSet.gameObject.name + ", " + wordSets[i].transform.localPosition);
        }
        yield return new WaitForSeconds(10f);
        creditsBtn.SetActive(true);
    }

    private IEnumerator BringInWhalePod()
    {
        StartCoroutine(ContinueAfter(fastPlay ? 0f : 0f));
        // more whales

        whale3 = (GameObject)Instantiate(Resources.Load("whale"), miniSub.transform);
        whale3.transform.localPosition = new Vector3(2.8f, 1.8f, 7.88f);
        whale3.transform.localScale = Vector3.one * 0.7f;


        yield return new WaitForSeconds(2f);
        if(whale1 == null)
        {
            whale1 = (GameObject)Instantiate(Resources.Load("whale"), miniSub.transform);
        } else
        {
            whale1.SetActive(true);
        }
        whale1.transform.localPosition = new Vector3(2.5f, 1f, 6.88f);
        whale1.transform.localScale = Vector3.one * 1.26f;

        yield return new WaitForSeconds(2f);

        whale2 = (GameObject)Instantiate(Resources.Load("whale"), miniSub.transform);
        whale2.transform.localPosition = new Vector3(2.6f, 1.2f, 7.38f);
        whale2.transform.localScale = Vector3.one * 1f;
        
        LeanTween.moveLocalX(whale1, -2.81f, 16f);
        LeanTween.moveLocalY(whale1, 0f, 16f);

        yield return new WaitForSeconds(3f);
        
        LeanTween.moveLocalX(whale2, -2.71f, 17f);
        LeanTween.moveLocalY(whale2, 0.23f, 17f);

        yield return new WaitForSeconds(2.5f);
        
        LeanTween.moveLocalX(whale3, -2.51f, 16f);
        LeanTween.moveLocalY(whale3, 0.5f, 16f).setOnComplete(HideWhales);      
    }

    private void HideWhales()
    {
        if (whale1 != null)
        {
            whale1.SetActive(false);
        }
        if (whale2 != null)
        {
            whale2.SetActive(false);
        }
        if (whale3 != null)
        {
            whale3.SetActive(false);
        }
    }

    public void Advance()
    {
        switch (puzzleNum)
        {
            case 0: WhalePass(); break;
            case 1: EnterCave(); break;
            case 2: ExitCave(); break;
            case 3: WhalePod(); break;
            case 4: Finale(); break;
            default: WhalePass(); break;
        }
        
        
    }
    private IEnumerator ContinueAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (puzzleNum == 4)
        {
            Debug.Log("GAME OVER");
            ShowCredits();
        } else
        {
            Debug.Log("...please set up next puzzle: " + puzzleNum);
            puzzleNum++;
            SetUpShuttlePuzzle();
        }
        
    }

    public void ShowCredits()
    {
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

