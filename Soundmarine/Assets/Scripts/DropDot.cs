using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDot : MonoBehaviour
{

    private GameObject droppedObj = null;

    public void SetObj(GameObject g)
    {
        droppedObj = g;
    }
    public GameObject GetObj()
    {
        return droppedObj;
    }
}
