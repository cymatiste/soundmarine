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
    private float minSpace = 0f;

    private Vector3 centerDotPos;

    public Player player;

    public void Init()
    {

        spotWidth = WidthOf(gameObject);
        spotLeftEdgeX = transform.localPosition.x - spotWidth / 2;

        centerDotPos = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.001f, transform.localPosition.z - 0.000001f);

        Transform spots = player.GetSpotSet();
        for (int i = 0; i < targetWords.Count; i++)
        {
            GameObject dot = (GameObject)Instantiate(Resources.Load("dropDot"));
            dot.name = gameObject.name + "_dot_" + i;
                        dots.Add(dot);
            dot.transform.localScale = 0.0015f * Vector3.one;
            dot.transform.localPosition = centerDotPos;
            dot.transform.parent = spots;
            dot.GetComponent<DropDot>().Init();
            dot.GetComponent<DropDot>().spot = gameObject.GetComponent<DropSpot>();
            dot.GetComponent<DropDot>().SetTargetWord(targetWords[i]);
            dot.GetComponent<DropDot>().SetObj(null);
        }
        combinedDotWidth = WidthOf(dots[0]) * targetWords.Count;

        SpaceEvenly(dots, centerDotPos, combinedDotWidth);
        Debug.Log("DropSpot " + gameObject.name + " started with "+dots.Count+" dots, child of "+spots.gameObject.name);
    }

    public List<GameObject> GetDots()
    {
        Debug.Log("      " + gameObject.name + " returning " + dots.Count + " dot(s)");
        return dots;
    }
    public bool PlaceWordAt(Word word, Vector3 clickPos, bool replacing = false)
    {
        //Debug.Log("Place /"+word.wordText+"/ at "+clickPos);

        Vector3 targetPos = new Vector3(clickPos.x, transform.localPosition.y, transform.localPosition.z - 0.012f);

        DropDot closestDot = ClosestObjectTo(clickPos, dots).GetComponent<DropDot>();

        Word placedWord = null;
        if (closestDot.GetObj() != null && !replacing)
        {
            //ClearWord(closestDot.GetObj().GetComponent<Word>(), true, false);
            placedWord = closestDot.GetObj().GetComponent<Word>();
            ClearWord(placedWord, true, true);
        } else if (closestDot.GetObj() != null)
        {
            Debug.Log("!!! THIS SHOULD NEVER HAPPEN");
            word.PutDown();
            return false;
        }

        closestDot.SetObj(word.gameObject);
        word.SetSpot(gameObject.GetComponent<DropSpot>());
        word.SetDot(closestDot);
        if (closestDot.Correct())
        {
            word.SetMood(1);
            GameObject.Find("GameManager").GetComponent<FollowingFish>().More();
        } else
        {
            word.SetMood(0);
        }

        // first determine where the new word belongs in the order on this row
        if (!words.Contains(word))
        {

            //Debug.Log("about to check for room");
            if (!RoomFor(word))
            {
                Word closestWord = ClosestWordTo(clickPos);
                if(closestWord != null)
                {
                    Debug.Log("not enough room, bouncing [ " + ClosestWordTo(clickPos).wordText + " ]");

                    ClearWord(closestWord, true, true);
                } else
                {
                    Debug.Log("not enough room and no words to bounce.");
                    return false;
                }  
            }
            int targetIndex = 0;
            for (int i = 0; i < words.Count; i++)
            {
                if ((words[i].transform.position.x < targetPos.x))
                {
                    targetIndex++;
                }
            }
            //Debug.Log("wanna insert at " + targetIndex + " of " + words.Count);
            words.Insert(targetIndex, word);
            combinedWordWidth += WidthOf(word.gameObject);
        }

        SpaceAllEvenly(targetPos);

        word.transform.rotation = transform.rotation;

        Vector3 dropScale = word.transform.localScale;
        word.transform.localScale = 3f * dropScale;
        LeanTween.scale(word.gameObject, dropScale, 0.3f).setEaseOutBack();
        Debug.Log("/" + word.wordText + "/ placed.");

        if(placedWord != null && !replacing)
        {
            StartCoroutine(ReplaceDislodgedWordAfter(placedWord, word, 0f));
        }

        player.ColorWords();

        return true;
    }

    private IEnumerator ReplaceDislodgedWordAfter(Word dislodgedWord, Word replacementWord, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (dislodgedWord != null && replacementWord.GetLastSpot() != null)
        {

            dislodgedWord.PickUp();
            Vector3 fakeClickPos = replacementWord.GetLastDot().transform.position;
            replacementWord.GetLastSpot().PlaceWordAt(dislodgedWord, fakeClickPos, true);
        }
    }


    public void ClearWord(Word w, bool andReset, bool andRespace)
    {
        w.Yellow();
        if (w.Correct())
        {
            GameObject.Find("GameManager").GetComponent<FollowingFish>().Fewer();
        }
        w.SetMood(0);
        if(w.GetDot() != null)
        {
            w.GetDot().SetObj(null);
        }
        
        words.Remove(w);
        combinedWordWidth -= WidthOf(w.gameObject);
        w.ClearSpot();

        foreach (GameObject d in dots)
        {
            DropDot dd = d.GetComponent<DropDot>();
            if (dd.GetObj() == w.gameObject)
            {
                dd.SetObj(null);
            }
        }
        if (andRespace)
        {
            Vector3 targetPos = new Vector3(w.transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - 0.002f);
            //SpaceAllEvenly(w.transform.localPosition);
            SpaceAllEvenly(targetPos);
        }     

        if (andReset && w.gameObject.GetComponent<IdleWobble>() != null)
        {
            w.gameObject.GetComponent<IdleWobble>().enabled = true;
            w.gameObject.GetComponent<IdleWobble>().ResetPos();
        }
        player.ColorWords();
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

        combinedWordWidth = 0f;
        for(int i=0; i<words.Count; i++)
        {
            combinedWordWidth += WidthOf(words[i].gameObject);
        }

        Debug.Log(words.Count + " < " + targetWords.Count + " ? " + (words.Count < targetWords.Count) + ",   ( " + combinedWordWidth + " + " + minSpace + " * " + words.Count + " + " + WidthOf(w.gameObject) + ") < " + spotWidth + " ) ? "+ (combinedWordWidth + minSpace * words.Count + WidthOf(w.gameObject) < spotWidth));
        
        return ((words.Count < targetWords.Count) && (combinedWordWidth + minSpace * words.Count + WidthOf(w.gameObject) < spotWidth));
    }

    private float WidthOf(GameObject g)
    {
        float totalWidth = 0f;
        //int numChildrenRendering = 0;

        if(g.GetComponent<BoxCollider>() != null)
        {
            totalWidth = g.GetComponent<BoxCollider>().size.x*g.transform.localScale.x;
        } else if (g.GetComponent<Renderer>() != null)
        {
            totalWidth = g.GetComponent<Renderer>().bounds.size.x;
            //numChildrenRendering++;
        }
        /*
        foreach (Transform child in g.transform)
        {
            if(child.GetComponent<Renderer>() != null && child.name != "effect")
            {
                totalWidth += child.gameObject.GetComponent<Renderer>().bounds.size.x;
                numChildrenRendering++;
            }
        }
        */
        //return totalWidth / numChildrenRendering;

        return totalWidth;
    }

    private Word ClosestWordTo(Vector3 clickPos)
    {
        List<GameObject> wordObjects = new();
        foreach (Word w in words)
        {
            wordObjects.Add(w.gameObject);
        }
        GameObject closestObj = ClosestObjectTo(clickPos, wordObjects);
        if (closestObj == null)
        {
            return null;
        } else
        {
            return closestObj.GetComponent<Word>();
        }
        
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
            //Debug.Log(" : item "+i+" is ")
            float neighbourEdgeX = (i == 0) ? spotLeftEdgeX : objects[i - 1].transform.localPosition.x + WidthOf(objects[i - 1]) / 2;
            float newTargetX = neighbourEdgeX + spaceBetween + WidthOf(objects[i]) / 2;

            objects[i].transform.localPosition = new Vector3(newTargetX, targetPos.y, targetPos.z);

            Debug.Log("placed " + objects[i].name + " at " + objects[i].transform.localPosition);
        }
    }

    public void SpaceAllEvenly(Vector3 targetPos)
    {
        
        List<GameObject> widest = new();

        float combinedWidth = 0f;
        for (int i = 0; i < dots.Count; i++)
        {
            GameObject placedObj = dots[i].GetComponent<DropDot>().GetObj();
            //Debug.Log("dot " + i +" has placed: "+placedObj);
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

        //Debug.Log("SpaceAllEvenly placing " + dots.Count + " items with combined width "+combinedWidth+" across "+WidthOf(gameObject)+" with " + spaceBetween + " between");


        for (int i = 0; i < dots.Count; i++)
        {
            //Debug.Log("  :: " + widest[i].name + " width " + WidthOf(widest[i]));
            float neighbourEdgeX = (i == 0) ? spotLeftEdgeX : widest[i - 1].transform.localPosition.x + WidthOf(widest[i - 1]) / 2;
            float newTargetX = neighbourEdgeX + spaceBetween + WidthOf(widest[i]) / 2;

            if (widest[i] != dots[i])
            {
                widest[i].transform.localPosition = new Vector3(newTargetX, transform.localPosition.y, transform.localPosition.z - 0.002f);
                //Debug.Log("SAE moving " + widest[i].name + " to " + widest[i].transform.localPosition);
            } 
            dots[i].transform.localPosition = new Vector3(newTargetX, centerDotPos.y, centerDotPos.z);

        }
    }
}
