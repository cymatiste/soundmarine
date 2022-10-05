using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDot : MonoBehaviour
{
    private GameObject droppedObj = null;
    private string targetWord;
    private Light light;


    private void Start()
    {
        light = transform.Find("Point Light").GetComponent<Light>();
        light.intensity = 0f;
    }
    public void SetTargetWord(string tw)
    {
        targetWord = tw;
    }

    public void Highlight()
    {
        //Debug.Log("........." + gameObject.name + " ON");
        light.intensity = 0.55f;
    }
    public void UnHighlight()
    {
        //Debug.Log("........." + gameObject.name + " OFF");
        light.intensity = 0f;
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
        Debug.Log("CORRECT? "+droppedObj.name + " text " + droppedObj.GetComponent<Word>().wordText + " == " + targetWord + " ---> " + (droppedObj.GetComponent<Word>().wordText == targetWord));
        return (droppedObj.GetComponent<Word>().wordText == targetWord);
    }
}
