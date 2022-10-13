using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform spotSet;
    public Transform finishedSet;
    //public List<float> loopSeconds;
    //public List<float> loopBeats;
    public float loopSeconds;
    public float loopBeats;
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
    private int numPuzzlesComplete;
    private bool activePuzzle;

    private List<Word> words;

    void Start()
    {
        playing = false;
    }

    public void InitPuzzle(int set)
    {
        puzzleNum = set;
        spots = new();
        dots = new();
        words = new();
        lastRun = false;
        activePuzzle = true;

        spotSet.gameObject.SetActive(true);
        spotSet.transform.localPosition = new Vector3(0, 0, 0);

        foreach (Transform t in spotSet)
        {
            if(t.gameObject.GetComponent<DropSpot>() != null)
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
        playing = true;
    }

    public void InitFinal()
    {
        List<Word> finalWords = new();
        spotSet.gameObject.SetActive(true);
        foreach (DropSpot ds in spots)
        {
            foreach(Word w in ds.GetWords())
            {
                finalWords.Add(w);
                GameObject bgFuzz = (GameObject)GameObject.Instantiate(Resources.Load("fuzzball"),w.transform);
                bgFuzz.transform.localPosition = new Vector3(0f, 0f, 0.1f);

            }
            ds.gameObject.SetActive(false);
        }

        DropSpot finalSpot = finishedSet.Find("Row").GetComponent<DropSpot>();
        finalSpot.Init();
        StartCoroutine(PrepopulateOneByOne(finalSpot, finalWords));
        activePuzzle = false;
        playing = true;
        lastRun = false;
        
    }


    private IEnumerator PrepopulateOneByOne(DropSpot spot, List<Word> testWords)
    {
        Debug.Log("Prepopulating " + spot.gameObject.name);
        for(int i=0; i<testWords.Count; i++)
        {
            yield return new WaitForSeconds(0.1f);
            testWords[i].PickUp();
            spot.PlaceWordAt(testWords[i], spot.GetDots()[i].transform.position, false);
            testWords[i].Wave(true);
        }
        spots = new();
        dots = new();
        spots.Add(spot);
        foreach (GameObject d in spot.GetDots())
        {
            dots.Add(d.GetComponent<DropDot>());
            d.GetComponent<MeshRenderer>().enabled = false;
        }
        spotSet = finishedSet;
        ColorWords();
    }

    public int NumPuzzlesComplete()
    {
        return numPuzzlesComplete;
    }

    public Transform GetSpotSet()
    {
        return spotSet;
    }

    public List<DropSpot> GetActiveSpots()
    {
        return spots;
    }

    public void ColorWords()
    {
        foreach(DropDot dot in dots)
        {
            if(dot.GetObj()== null)
            {
                continue;
            }
            Word word = dot.GetObj().GetComponent<Word>();

            if (!activePuzzle)
            {
                word.Yellow();
                continue;
            }

            if (NumWordsPlaced() < dots.Count)
            {
                word.Yellow();
            }
            else
            {
                if (dot.Correct())
                {
                    word.Green();
                }
                else
                {
                    bool correctRow = false;
                    bool correctPoem = false;
                    foreach (string s in dot.spot.targetWords)
                    {

                        if (word.wordText == s)
                        {
                            correctRow = true;
                            word.Blue();
                        }
                    }
                    if (!correctRow)
                    {
                        foreach (DropSpot ds in spots)
                        {
                            if (ds != dot.spot)
                            {
                                foreach (string s in ds.targetWords)
                                {
                                    if (word.wordText == s)
                                    {
                                        correctPoem = true;
                                        word.Violet();
                                    }
                                }
                            }
                        }
                        if (!correctPoem)
                        {
                            word.Red();
                        }
                    }


                }
            }

        }
    }

    void Update()
    {
        if (!playing)
        {
            return;
        }


        if (loopTimer > loopSeconds)
        {
            loopIndex = 0;
            loopTimer = 0f;

            if (activePuzzle)
            {
                Debug.Log("LOOP. "+NumWordsCorrect() + " correct out of " + dots.Count + ", " + words.Count + " placed.");
            }


            if (activePuzzle && NumWordsCorrect() == dots.Count)
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
                    Debug.Log(gameObject.name + " reporting PuzzleComplete");
                     

                    spotSet.transform.position = new Vector3(spotSet.transform.position.x, 10f, spotSet.transform.position.z);
                    numPuzzlesComplete++;
                    GameObject.Find("GameManager").GetComponent<GameManager>().PuzzleComplete();
                    lastRun = true;
                } else
                {
                    playing = false;
                    activePuzzle = false;
                    return;
                }
               
            }

            words.Clear();

        }

        int targetIndex = loopIndex;
        if ((loopTimer / loopSeconds) * loopBeats * 2 >= targetIndex)
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
                    if (activePuzzle)
                    {
                        activeDot.Highlight();
                    }
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

    public void ResetLoop()
    {
        loopIndex = 0;
        loopTimer = 0f;
    }

    public void PlayLoop(bool isPuzzleActive)
    {
        spotSet.gameObject.SetActive(true);
        ResetLoop();
        playing = true;
        activePuzzle = isPuzzleActive;
    }
    private void PlayWord(Word word)
    {
        if (word != null)
        {
            words.Add(word);
            //Debug.Log("))):  " + word.name);
            if (activePuzzle)
            {
                word.Highlight(activePuzzle);
            }
           
            if (!word.GetComponent<AudioSource>().isPlaying)
            {
                word.Speak();
            }
            if (lastRun)
            {
                word.Wave();
            }

            if (prevWord != null && activePuzzle)
            {
                prevWord.UnHighlight(activePuzzle);
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
