using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropSpot : MonoBehaviour
{
    private List<Word> words = new List<Word>();
    private float spotWidth;
    private float spotLeftEdgeX;
    private float combinedWordWidth;
    private float minSpace = 0.003f;


    private void Start()
    {
        spotWidth = WidthOf(gameObject);
        spotLeftEdgeX = transform.localPosition.x - spotWidth / 2;
    }

    public void PlaceWordAt(Word word, Vector3 clickPos)
    {

        Vector3 targetPos = new Vector3(clickPos.x, transform.localPosition.y, transform.localPosition.z - 0.002f);
        
        // first determine where the new word belongs in the order on this row
        int targetIndex = 0;
        for (int i = 0; i < words.Count; i++)
        {
            if ((words[i].transform.position.x < targetPos.x))
            {
                targetIndex++;
            }
        }
        if (!words.Contains(word))
        {
            if (!RoomFor(word))
            {
                ClearWord(ClosestWordTo(clickPos), true);
            }
            words.Insert(targetIndex, word);
            combinedWordWidth += WidthOf(word.gameObject);
        }
        for (int i = 0; i < words.Count; i++)
        {
            Debug.Log("   "+i + ":  " + words[i].wordText);
        }
            

        // and then space all words out evenly.
        float spaceBetween = (spotWidth - combinedWordWidth)/(words.Count+1);

        //Debug.Log("spaceBetween: " + spaceBetween);

        for (int i = 0; i < words.Count; i++)
        {

            float neighbourEdgeX = (i==0) ? spotLeftEdgeX : words[i - 1].transform.localPosition.x + WidthOf(words[i - 1].gameObject) / 2;
            float newTargetX = neighbourEdgeX + spaceBetween + WidthOf(words[i].gameObject)/ 2;

            Debug.Log("word "+i+", " + words[i].wordText + ":  neighbourEdgeX: "+neighbourEdgeX+", WidthOf word i: "+WidthOf(words[i].gameObject));


            words[i].transform.localPosition = new Vector3(newTargetX, targetPos.y, targetPos.z);
        }

        word.transform.rotation = transform.rotation;

        
    }

    public void ClearWord(Word w, bool andReset)
    {
        words.Remove(w);
        combinedWordWidth -= WidthOf(w.gameObject);
        w.ClearSpot();

        if (andReset && w.gameObject.GetComponent<IdleWobble>() != null)
        {
            w.gameObject.GetComponent<IdleWobble>().enabled = true;
            w.gameObject.GetComponent<IdleWobble>().ResetPos();
        }
        
    }

    public List<Word> GetWords()
    {
        return words;
    }

    public Word GetWordAt(Vector3 clickPos)
    {
        return ClosestWordTo(clickPos);
    }

    private bool RoomFor(Word w)
    {
        return (combinedWordWidth + minSpace*words.Count + WidthOf(w.gameObject) < spotWidth);
    }

    private float WidthOf(GameObject g)
    {
        float totalWidth = 0f;
        int numChildrenRendering = 0;


        if (g.GetComponent<Renderer>() != null)
        {
            totalWidth += g.GetComponent<Renderer>().bounds.size.x;
            numChildrenRendering++;
        }
            
        foreach (Transform child in g.transform)
        {
            if(child.GetComponent<Renderer>() != null)
            {
                totalWidth += child.gameObject.GetComponent<Renderer>().bounds.size.x;
                numChildrenRendering++;
            }
        }
        return totalWidth / numChildrenRendering;
    }

    private Word ClosestWordTo(Vector3 clickPos)
    {

        float closestDist = 9999f;
        Word closestWord = null;
        foreach(Word w in words)
        {
            float dist = Mathf.Abs(w.transform.position.x - clickPos.x);
            if(dist < closestDist)
            {
                closestDist = dist;
                closestWord = w;
            }
        }
        return closestWord;
    }
}
