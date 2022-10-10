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
    private DropSpot prevSpot = null;
    private DropDot dot = null;
    private DropDot prevDot = null;
    private Vector3 wordScale;

    private Color offColor = new Color(1f, 1f, 1f, 1f);
    private Color onColor = new Color(1.5f, 1.5f, 1f, 1f);

    private int HAPPIEST = 1;
    private int UNHAPPY = 0;
    private int mood = 0;
    private bool waving = false;
      
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void Init()
    {
        mood = UNHAPPY;
        textObj = gameObject.GetComponentInChildren<TMPro.TextMeshPro>();
        if(textObj != null)
        {
            textObj.text = wordText;
        }
        wordVo = gameObject.GetComponent<AudioSource>();
        wordScale = transform.localScale;
    }

    public void SetMood(int newMood)
    {
        
        if (newMood == HAPPIEST)
        {
            wordVo.pitch = 1f;
        } else
        {
            wordVo.pitch = 0.5f;
        }
        mood = newMood;
        
    }

    public bool Correct()
    {
        return ( mood == HAPPIEST );
    }

    public void Green()
    {
        GreenChildrenOf(transform);
    }

    private void GreenChildrenOf(Transform parentT)
    {
        foreach (Transform t in parentT)
        {
            MeshRenderer mr = t.gameObject.GetComponent<MeshRenderer>();
            if (t.gameObject.name != "effect" && mr != null)
            {
                mr.material.color = new Color(0.33f, 0.8f, 0.2f, 1);
            }
            GreenChildrenOf(t);
        }
    }

    public void Blue()
    {
        BlueChildrenOf(transform);
    }

    private void BlueChildrenOf(Transform parentT)
    {
        foreach (Transform t in parentT)
        {
            MeshRenderer mr = t.gameObject.GetComponent<MeshRenderer>();
            if (t.gameObject.name != "effect" && mr != null)
            {
                mr.material.color = new Color(0.1f, 0.67f, 1f, 1);
            }
            BlueChildrenOf(t);
        }
    }

    public void Violet()
    {
        VioletChildrenOf(transform);
    }

    private void VioletChildrenOf(Transform parentT)
    {
        foreach (Transform t in parentT)
        {
            MeshRenderer mr = t.gameObject.GetComponent<MeshRenderer>();
            if (t.gameObject.name != "effect" && mr != null)
            {
                mr.material.color = new Color(1f, 0.1f, 1f, 1);
            }
            VioletChildrenOf(t);
        }
    }

    public void Red()
    {
        RedChildrenOf(transform);
    }

    private void RedChildrenOf(Transform parentT)
    {
        foreach (Transform t in parentT)
        {
            MeshRenderer mr = t.gameObject.GetComponent<MeshRenderer>();
            if (t.gameObject.name != "effect" && mr != null)
            {
                mr.material.color = new Color(0.9f, 0.1f, 0.1f, 1);
            }
            RedChildrenOf(t);
        }
    }

    public void Yellow()
    {
        YellowChildrenOf(transform);   
    }

    private void YellowChildrenOf(Transform parentT)
    {
        foreach (Transform t in parentT)
        {
            MeshRenderer mr = t.gameObject.GetComponent<MeshRenderer>();
            if (t.gameObject.name != "effect" && mr != null)
            {
                mr.material.color = new Color(1, 0.67f, 0, 1);
            }
            YellowChildrenOf(t);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (waving)
        {
            
            foreach(Transform t in transform)
            {
                t.localPosition = new Vector3(t.localPosition.x, 0.05f * Mathf.Sin(Time.time*6f + t.position.x*12f), t.localPosition.z);
            }
        }
    }

    public void PickUp()
    {
        gameObject.GetComponent<IdleWobble>().enabled = false;
        gameObject.transform.rotation = Quaternion.identity;
    }

    public void PutDown()
    {
        gameObject.GetComponent<IdleWobble>().enabled = true;
        gameObject.GetComponent<IdleWobble>().ResetPos();
    }

    public void Speak()
    {
        wordVo.Play();
    }

    public void SetSpot(DropSpot ds)
    {
        spot = ds;
    }

    public void SetDot(DropDot d)
    {
        dot = d;
    }

    public DropDot GetDot()
    {
        return dot;
    }
    public void ClearSpot()
    {
        Debug.Log("/" + wordText + "/ clearing spot, spot " + (spot == null ? "NULL" : spot.gameObject.name)+", dot "+ (dot == null ? "NULL" : dot.gameObject.name));

        prevSpot = spot;
        prevDot = dot;
        spot = null;
        dot = null;

        Debug.Log("//"+wordText + "// cleared, prevSpot now " + (prevSpot == null ? "NULL" : prevSpot.gameObject.name)+", prevDot now " + (prevDot == null ? "NULL" : prevDot.gameObject.name));
    }

    public DropSpot GetSpot()
    {
        return spot;
    }
    public DropSpot GetLastSpot()
    {
        return prevSpot;
    }
    public DropDot GetLastDot()
    {
        return prevDot;
    }
    public void Highlight()
    {
        //transform.GetChild(0).GetComponent<Renderer>().material.color = onColor;
        
        if (Correct())
        {
            
            transform.localScale = 1.1f * wordScale;
            Transform effect = transform.Find("effect");
            if (effect != null)
            {
                effect.GetComponent<SpriteRenderer>().enabled = true;
                effect.GetComponent<Animator>().Play("Liquid", 0, 0f);

            }
        } else
        {
            transform.localScale = 0.9f * wordScale;
        }
    }
    public void UnHighlight()
    {
        //transform.GetChild(0).GetComponent<Renderer>().material.color = offColor;
        transform.localScale = new Vector3(wordScale.x, wordScale.y, wordScale.z);
    }

    public void Wave()
    {
        waving = true;
    }
}
