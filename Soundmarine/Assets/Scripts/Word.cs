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
    private Vector3 wordScale;

    private Color offColor = new Color(1f, 1f, 1f, 1f);
    private Color onColor = new Color(1.5f, 1.5f, 1f, 1f);

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
        wordScale = transform.localScale;
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

    public void Highlight()
    {
        transform.GetChild(0).GetComponent<Renderer>().material.color = onColor;
        transform.localScale = new Vector3(wordScale.x * 1.1f, wordScale.y * 1.1f, wordScale.z * 1.1f);
    }
    public void UnHighlight()
    {
        transform.GetChild(0).GetComponent<Renderer>().material.color = offColor;
        transform.localScale = new Vector3(wordScale.x, wordScale.y, wordScale.z);
    }
}
