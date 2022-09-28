using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public List<DropSpot> spots = new();
    public float loopSeconds = 20f;
    public AudioSource beat1;
    public AudioSource beat2;
    public bool playing = true;

    private float loopTimer = 0f;
    private int loopIndex = 0;
    private int loopBeats = 16;
    private Word prevWord;
    

    private List<Word> words = new();

    void Start()
    {
    }

    void Update()
    {
        if (!playing)
        {
            return;
        }

        words.Clear();
        foreach (DropSpot spot in spots)
        {
            foreach (Word w in spot.GetWords())
            {
                words.Add(w);
            }
        }
       
        //Debug.Log("WORDS: " + wordTracer);

        if (loopTimer > loopSeconds)
        {
            loopIndex = 0;
            loopTimer = 0f;

        }

        int targetIndex = loopIndex;
        if ((loopTimer / loopSeconds) * loopBeats*2 >= targetIndex)
        {
            //Debug.Log("*** " + loopIndex + ", " + loopTimer);
            AudioSource beat = (loopIndex % 2 == 0) ? beat1 : beat2;
            beat.Play();

            if (loopIndex % 2 == 0)
            { 
                if(loopIndex / 2 < words.Count)
                {
                    PlayWord(words[loopIndex / 2]);
                }
                else if(prevWord != null)
                {
                    prevWord.UnHighlight();
                    prevWord = null;
                }
            }
            

            loopIndex++;
        }
        loopTimer += Time.deltaTime;
    }

    private void PlayWord(Word word)
    {
        if (word != null && !word.GetComponent<AudioSource>().isPlaying)
        {

            //Debug.Log("))):  " + word.wordText);
            word.GetComponent<AudioSource>().Play();
            word.Highlight();

            Transform effect = word.gameObject.transform.Find("effect");
            if (effect != null)
            {
                effect.GetComponent<SpriteRenderer>().enabled = true;
                effect.GetComponent<Animator>().Play("Liquid", 0, 0f);
            }

            if (prevWord != null)
            {
                prevWord.UnHighlight();
            }
            prevWord = word;
        }
    }

    public int NumWordsPlaced()
    {
        return words.Count;
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
