using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropSpot : MonoBehaviour
{
    public List<string> targetWords;
    private List<Word> words = new List<Word>();
    private List<GameObject> dots = new List<GameObject>();

    private float spotWidth;
    private float spotLeftEdgeX;
    private float combinedWordWidth;
    private float combinedDotWidth;
    private float minSpace = 0.003f;

    private Vector3 centerDotPos;

    private void Start()
    {
        spotWidth = WidthOf(gameObject);
        spotLeftEdgeX = transform.localPosition.x - spotWidth / 2;

        centerDotPos = new Vector3(transform.localPosition.x,  transform.localPosition.y-0.0015f, transform.localPosition.z - 0.000001f);

        GameObject spots = GameObject.Find("spots");
        for (int i=0; i< targetWords.Count; i++)
        {
            GameObject dot = (GameObject)Instantiate(Resources.Load("dropDot"));
            dots.Add(dot);
            dot.transform.localScale = 0.0015f * Vector3.one;
            dot.transform.localPosition = centerDotPos;
            dot.transform.parent = spots.transform;
            dot.GetComponent<DropDot>().SetTargetWord(targetWords[i]);
        }
        combinedDotWidth = WidthOf(dots[0])* targetWords.Count;

        SpaceEvenly(dots, centerDotPos, combinedDotWidth);
    }

    public void PlaceWordAt(Word word, Vector3 clickPos)
    {

        Vector3 targetPos = new Vector3(clickPos.x, transform.localPosition.y, transform.localPosition.z - 0.002f);

        DropDot closestDot = ClosestObjectTo(clickPos, dots).GetComponent<DropDot>();
        
 
        if(closestDot.GetObj() != null)
        {
            ClearWord(closestDot.GetObj().GetComponent<Word>(), true);
        }

        closestDot.SetObj(word.gameObject);
        if (closestDot.Correct())
        {
            // give + feedback
            // e.g. word.play particle effect
            // word move accordingly
            word.SetMood(1);
            // somebody add fish
        } else
        {
            // give - feedback
            // 
            word.SetMood(0);
        }
        
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

        List<GameObject> wordObjs = new();
        for (int i = 0; i < words.Count; i++)
        {
            //Debug.Log("   "+i + ":  " + words[i].wordText);
            wordObjs.Add(words[i].gameObject);
        }


        // and then space all words out evenly.



        //SpaceEvenly(wordObjs, targetPos, combinedWordWidth);
        SpaceAllEvenly(targetPos);

        word.transform.rotation = transform.rotation;

        
    }

    public void ClearWord(Word w, bool andReset)
    {
        words.Remove(w);
        combinedWordWidth -= WidthOf(w.gameObject);
        w.ClearSpot();

        foreach (GameObject d in dots)
        {
            DropDot dd = d.GetComponent<DropDot>();
            if (dd.GetObj() == w.gameObject)
            {
                if (dd.Correct())
                {
                    // adjust my overall correctness downward (less fish)
                } else
                {
                    // any negative effects here that need to be undone?
                    // stop any word shaking etc.
                }
                dd.SetObj(null);
            }
        }

        SpaceAllEvenly(w.transform.localPosition);

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
        return ((words.Count < targetWords.Count) && (combinedWordWidth + minSpace*words.Count + WidthOf(w.gameObject) < spotWidth));
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
        List<GameObject> wordObjects = new();
        foreach(Word w in words)
        {
            wordObjects.Add(w.gameObject);
        }
        return ClosestObjectTo(clickPos, wordObjects).GetComponent<Word>();
    }

    private GameObject ClosestObjectTo(Vector3 clickPos, List<GameObject> objects )
    {

        float closestDist = 9999f;
        GameObject closestObj = null;
        foreach (GameObject o in objects)
        {
            float dist = Mathf.Abs(o.transform.position.x - clickPos.x);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestObj = o;
            }
        }
        return closestObj;
    }


    private void SpaceEvenly(List<GameObject> objects, Vector3 targetPos, float combinedObjectsWidth)
    {

        float spaceBetween = (spotWidth - combinedObjectsWidth) / (objects.Count + 1);

        //Debug.Log("placing " + objects.Count + " items with " + spaceBetween + " between");
        for (int i = 0; i < objects.Count; i++)
        {

            float neighbourEdgeX = (i == 0) ? spotLeftEdgeX : objects[i - 1].transform.localPosition.x + WidthOf(objects[i - 1]) / 2;
            float newTargetX = neighbourEdgeX + spaceBetween + WidthOf(objects[i]) / 2;

            objects[i].transform.localPosition = new Vector3(newTargetX, targetPos.y, targetPos.z);

            //Debug.Log("placed " + objects[i].name + " at " + objects[i].transform.localPosition);
        }
    }

    private void SpaceAllEvenly(Vector3 targetPos)
    {
        List<GameObject> widest = new();

        float combinedWidth = 0f;
        for (int i = 0; i < dots.Count; i++)
        {
            GameObject placedObj = dots[i].GetComponent<DropDot>().GetObj();
            if (placedObj == null)
            {
                combinedWidth += WidthOf(dots[i]);
                widest.Add(dots[i]);
            } else
            {
                combinedWidth += WidthOf(placedObj);
                widest.Add(placedObj);
            }
        }

        float spaceBetween = (spotWidth - combinedWidth) / (dots.Count + 1);

        for (int i = 0; i < dots.Count; i++)
        {
            
            float neighbourEdgeX = (i == 0) ? spotLeftEdgeX : widest[i - 1].transform.localPosition.x + WidthOf(widest[i - 1]) / 2;
            float newTargetX = neighbourEdgeX + spaceBetween + WidthOf(widest[i]) / 2;

            if (widest[i] != dots[i])
            {
                widest[i].transform.localPosition = new Vector3(newTargetX, targetPos.y, targetPos.z);
            } 
            dots[i].transform.localPosition = new Vector3(newTargetX, centerDotPos.y, centerDotPos.z);

        }
    }
}
