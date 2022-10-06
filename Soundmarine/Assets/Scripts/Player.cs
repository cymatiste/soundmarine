using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public List<Transform> spotSets;
    public List<float> loopSeconds;
    public List<float> loopBeats;
    public AudioSource beat1;
    public AudioSource beat2;
    public bool playing = false;
    public bool lastRun = false;

    
    private float loopTimer = 0f;
    private int loopIndex = 0;
    private Word prevWord;
    private DropDot prevDot;
    private List<DropSpot> spots;
    private List<DropDot> dots;
    private int puzzleNum;


    private List<Word> words;

    void Start()
    {
        playing = false;
        foreach(Transform t in spotSets)
        {
            t.gameObject.SetActive(false);
        }
    }


    public void InitPuzzle(int set)
    {
        puzzleNum = set;
        spots = new();
        dots = new();
        words = new();
        lastRun = false;
        for (int i = 0; i < spotSets.Count; i++)
        {
            spotSets[i].gameObject.SetActive(i==puzzleNum);
        }
        foreach (Transform t in spotSets[puzzleNum])
        {
            if(t.gameObject != null && t.gameObject.GetComponent<DropSpot>() != null)
            {
                spots.Add(t.gameObject.GetComponent<DropSpot>());
            }
            
        }
        Debug.Log("found " + spots.Count + " spots");
        foreach (DropSpot spot in spots)
        {
            spot.Init();
            foreach (GameObject d in spot.GetDots())
            {
                dots.Add(d.GetComponent<DropDot>());
            }
        }
        Debug.Log("added " + dots.Count + " dots");
        playing = true;
    }

    public Transform GetSpotSet()
    {
        return spotSets[puzzleNum];
    }

    void Update()
    {
        if (!playing)
        {
            return;
        }
  

        if (loopTimer > loopSeconds[puzzleNum])
        {
            loopIndex = 0;
            loopTimer = 0f;

            //Debug.Log(NumWordsCorrect() + " correct out of " + dots.Count + ", " + words.Count + " placed.");

            if (NumWordsCorrect() == dots.Count)
            {
                if (!lastRun)
                {
                    /*
                    foreach (DropSpot spot in spots)
                    {
                        foreach (GameObject dot in spot.GetDots())
                        {
                            dot.SetActive(false);
                        }
                        spot.gameObject.SetActive(false);
                    }
                    */
                    spotSets[puzzleNum].gameObject.SetActive(false);
                    GameObject.Find("GameManager").GetComponent<GameManager>().PuzzleComplete();
                    lastRun = true;
                } else
                {
                    playing = false;
                    return;
                }
               
            }

            words.Clear();

        }

        int targetIndex = loopIndex;
        if ((loopTimer / loopSeconds[puzzleNum]) * loopBeats[puzzleNum] * 2 >= targetIndex)
        {
            //Debug.Log("*** " + loopIndex + ", " + loopTimer+", "+dots+", playing ? "+playing);
            AudioSource beat = (loopIndex % 2 == 0) ? beat1 : beat2;
            beat.Play();

            if (loopIndex % 2 == 0)
            {
                if (loopIndex / 2 < dots.Count)
                {
                    if (prevDot != null)
                    {
                        prevDot.UnHighlight();
                    }

                    //PlayWord(words[loopIndex / 2]);
                    DropDot activeDot = dots[loopIndex / 2];
                    if (activeDot.GetObj() != null)
                    {
                        PlayWord(activeDot.GetObj().GetComponent<Word>());
                    }
                    activeDot.Highlight();
                    prevDot = activeDot;
                    //Debug.Log("   ** highlighting " + activeDot.name);
                }
                else { 
                    if (prevWord != null)
                    {
                        prevWord.UnHighlight();
                        prevWord = null;
                    }
                    if (prevDot != null)
                    {
                        //Debug.Log("   ** UNhighlighting " + prevDot.name);
                        prevDot.UnHighlight();
                        prevDot = null;
                    }                  
                }
            }
            loopIndex++;
        }
        loopTimer += Time.deltaTime;
    }

    private void PlayWord(Word word)
    {
        if (word != null)
        {
            words.Add(word);
            Debug.Log("))):  " + word.name);
            word.Highlight();
            if (!word.GetComponent<AudioSource>().isPlaying)
            {
                word.Speak();
            }
            if (lastRun)
            {
                word.Wave();
            }

            if (prevWord != null)
            {
                prevWord.UnHighlight();
                prevDot.UnHighlight();
            }
            prevWord = word;
        }
    }

    public int NumWordsPlaced()
    {
        int numPlaced = 0;
        if(dots == null)
        {
            return 0;
        } else
        {
            foreach(DropDot d in dots)
            {
                if (d.GetObj() != null)
                {
                    numPlaced++;
                }
            }
        }
        return numPlaced;
       
    }

    public int NumWordsCorrect()
    {
        int numCorrect = 0;
        for(int i=0; i<words.Count; i++)
        {
            if (words[i].Correct())
            {
                numCorrect++;
            }
        }
        return numCorrect;
    }

}
