using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;

public class Word : MonoBehaviour
{

    public string wordText;
    public AudioSource wordVo;
    private TMPro.TextMeshPro textObj;
    private DropSpot spot = null;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void Init()
    {
        textObj = gameObject.GetComponentInChildren<TMPro.TextMeshPro>();
        if(textObj != null)
        {
            textObj.text = wordText;
        }
        wordVo = gameObject.GetComponent<AudioSource>();
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Speak()
    {
        wordVo.Play();
    }

    public void SetSpot(DropSpot d)
    {
        spot = d;
    }

    public void ClearSpot()
    {
        spot = null;
    }

    public DropSpot GetSpot()
    {
        return spot;
    }
}
