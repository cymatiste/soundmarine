using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDot : MonoBehaviour
{
    private GameObject droppedObj = null;
    private string targetWord;

    public void SetTargetWord(string tw)
    {
        targetWord = tw;
    }

    public void SetObj(GameObject g)
    {
        droppedObj = g;
    }
    public GameObject GetObj()
    {
        return droppedObj;
    }
    public bool Correct()
    {
        return droppedObj.GetComponent<Word>().wordText == targetWord;
    }
}
