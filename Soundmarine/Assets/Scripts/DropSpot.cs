using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropSpot : MonoBehaviour
{
    private int rowIndex;
    private Word word = null;
    
    public void SetIndex(int index)
    {
        rowIndex = index;
    }

    public void SetWord(Word w)
    {
        word = w;
    }
    public void ClearWord()
    {
        word = null;
    }

    public Word GetWord()
    {
        return word;
    }
}
