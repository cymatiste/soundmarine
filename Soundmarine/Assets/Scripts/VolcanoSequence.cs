using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolcanoSequence : MonoBehaviour
{
    public GameObject dreamArt1;
    public GameObject dreamArt2;
    public GameObject dreamer;

    [Range(0.0f, 1f)]
    public float cycleSpeed = 0.025f;

    [Range(0.5f, 1.5f)]
    public float breatheSpeed = 1f;

    public AudioSource inBreath;
    public AudioSource outBreath;

    public GameObject inBtn;
    public GameObject outBtn;

    public ParticleSystem rings;

    public List<Transform> wordTransforms;
    public List<GameObject> wordsRevealed;

    private bool btnPressed = false;
    private bool breathingIn = false;

    // phases:
    // 0: panning in
    // 1: breathing
    // 2: exiting
    private int scenePhase = 0;
    
    private float breathVol = 0.2f;

    private Vector3 dreamerStartScale;

    private float spawnTime = 0f;
    private float spawnDelay = 1f;
    private float slowStartTime = 0f;
    private float inTime = 0f;
    private float outTime = 0f;



    private UnityEngine.Audio.AudioMixerGroup pitchBendGroup;

    // Start is called before the first frame update
    void Start()
    {
        slowStartTime = Time.time;

        pitchBendGroup = Resources.Load<UnityEngine.Audio.AudioMixerGroup>("PitchBendMixer");
        inBreath.outputAudioMixerGroup = pitchBendGroup;
        outBreath.outputAudioMixerGroup = pitchBendGroup;
        inBreath.volume = 0.2f;
        outBreath.volume = 0.2f;


        dreamerStartScale = dreamer.transform.localScale;

        inBtn.SetActive(false);
        outBtn.SetActive(false);

        rings.gameObject.SetActive(false);

        foreach (Transform t in wordTransforms)
        {
            t.gameObject.SetActive(false);
        }

        BreatheIn();
    }

    // Update is called once per frame
    void Update()
    {
        if (scenePhase == 1)
        {
            dreamArt1.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, (breathVol - 0.25f) * Mathf.Sin(Time.time * cycleSpeed));
            dreamArt2.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, (breathVol - 0.25f) * Mathf.Cos(Time.time * cycleSpeed*1.5f));
        }
        else if (scenePhase == 2)
        {
            dreamArt1.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, dreamArt1.GetComponent<Renderer>().material.color.a * 0.9f);
            dreamArt2.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, dreamArt2.GetComponent<Renderer>().material.color.a * 0.9f);
        }
        else
        {
            dreamArt1.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0f);
            dreamArt2.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0f);
        }

        ParticleSystem.EmissionModule emission = rings.emission;
        ParticleSystem.MainModule main = rings.main;
      
        emission.rateOverTime = 3f*(0.51f - breathVol);
        //main.simulationSpeed = 1f * (1f - breathVol);

        inBreath.pitch = breatheSpeed;
        inBreath.outputAudioMixerGroup.audioMixer.SetFloat("pitchBend", 1f / breatheSpeed);

        outBreath.pitch = breatheSpeed;
        outBreath.outputAudioMixerGroup.audioMixer.SetFloat("pitchBend", 1f / breatheSpeed);

        if (btnPressed)
        {
            inBtn.GetComponent<Image>().color = new Color(0f, 0f, 1.2f * (inBreath.time / inBreath.clip.length));
            outBtn.GetComponent<Image>().color = new Color(0f, 0f, 1.2f * (outBreath.time / outBreath.clip.length));
            breathVol = Mathf.Min(0.5f, breathVol + 0.001f);
            
        } else
        {
            inBtn.GetComponent<Image>().color = new Color(0f, 0f, 0f);
            outBtn.GetComponent<Image>().color = new Color(0f, 0f, 0f);
            breathVol = Mathf.Max(0.2f, breathVol - 0.001f);
        }
        //Debug.Log("v  " + breathVol);
        inBreath.volume = breathVol;
        outBreath.volume = breathVol;


        if (btnPressed)
        {
            Slower();
        } else
        {
            Faster();
        }

        if(breathingIn && inBreath.time == 0f && scenePhase == 1)
        {
            BreatheOut();
        } else if (!breathingIn && outBreath.time == 0f && scenePhase == 1)
        {
            BreatheIn();
        }
        
        float inStretch = 0.2f * (inBreath.time / (inBreath.clip.length));
        float outStretch = 0.2f * (outBreath.time / (outBreath.clip.length));
        float xScaleTarget = breathingIn ? dreamerStartScale.x * (0.9f + inStretch) : dreamerStartScale.x * (1.1f - outStretch);
        float xScale = xScaleTarget;
        //Debug.Log(dreamer.transform.localScale.x + " VS " + xScaleTarget);
        if ((Mathf.Abs(dreamer.transform.localScale.x - xScaleTarget) > 0.001f))
        {
            xScale = dreamer.transform.localScale.x + (xScaleTarget - dreamer.transform.localScale.x) / 10f;
            Debug.Log("TWEENING.");
        }
        //Debug.Log("inStretch: " + inStretch + ",   outStretch: " + outStretch + ",   xScale: " + (xScale / dreamerStartScale.x));
        dreamer.transform.localScale = new Vector3(xScale, dreamer.transform.localScale.y, dreamer.transform.localScale.z);

        

        if (scenePhase==1 && breatheSpeed < 0.55f)
        {
            if (Time.time - slowStartTime > 10f && Time.time - spawnTime > spawnDelay) 
            {
                SpawnWord();
                spawnTime = Time.time;
                spawnDelay *= 0.95f;
            }
        } else
        {
            slowStartTime = Time.time;
        }

        //Debug.Log(inBreath.time+" / "+outBreath.time);
    }

    private void Slower()
    {
        breatheSpeed = Mathf.Max(0.5f, breatheSpeed - 0.001f);
        //Debug.Log(breatheSpeed + "    in "+inBreath.clip.length/breatheSpeed+"     out "+outBreath.clip.length/breatheSpeed);
    }
    private void Faster()
    {
        breatheSpeed = Mathf.Min(1.5f, breatheSpeed + 0.001f);
        //Debug.Log(breatheSpeed + "    in " + inBreath.clip.length / breatheSpeed + "     out " + outBreath.clip.length / breatheSpeed);
    }

    public void PressIn()
    {
        btnPressed = true;
        BreatheIn();
    }
    public void PressOut()
    {
        btnPressed = true;
        BreatheOut();
    }
    public void ReleaseIn()
    {
        btnPressed = false;
    }
    public void ReleaseOut()
    {
        btnPressed = false;
       
    }



    public void BreatheIn()
    {
        if (!breathingIn)
        {
            outBreath.Stop();
            breathingIn = true;
            inTime = Time.time;
            inBreath.time = 0.0001f;
            inBreath.PlayDelayed(btnPressed ? 0f : 0.5f);
            if (Time.time - outTime < 2f)
            {
                breatheSpeed = 1.5f;
            }
            rings.Emit(1);

            if (btnPressed)
            {
                StartCoroutine(HideButton(outBtn, 1f));
            }
        }
        
    }
    public void BreatheOut()
    {
        if (breathingIn)
        {
            float lastInBreathDuration = Time.time - inTime;
            outTime = Time.time;
            inBreath.Stop();
            breathingIn = false;
            outBreath.time = 0.0001f;
            outBreath.PlayDelayed(btnPressed ? 0f : 0.5f);
            if(Time.time - inTime < 2f)
            {
                breatheSpeed = 1.5f;
            }
            rings.Emit(1);
            if (btnPressed)
            {
                StartCoroutine(HideButton(inBtn, 1f));
            }
            
        }
        
    }
    public void Exit()
    {
        inBtn.SetActive(false);
        outBtn.SetActive(false);
        dreamArt1.SetActive(false);
        dreamArt2.SetActive(false);
    }

    public void ShowButtons(bool toShow)
    {
        if (toShow)
        {
            scenePhase = 1;
            inBtn.SetActive(true);
            outBtn.SetActive(true);
            rings.gameObject.SetActive(true);
            rings.Play();
        } else
        {
            scenePhase = 2;
            inBtn.SetActive(false);
            outBtn.SetActive(false);
            rings.Stop(true,ParticleSystemStopBehavior.StopEmitting);
            dreamArt1.GetComponent<Renderer>().material.color = new Color(0,0,0,0);
            dreamArt2.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
            StartCoroutine(MoveOnAfter(10f));
        }
        
    }

    private IEnumerator MoveOnAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach(GameObject g in wordsRevealed)
        {
            g.SetActive(false);
        }
        gameObject.GetComponent<GameManager>().PanToShuttle();
    }

    private IEnumerator HideButton(GameObject btn, float s)
    {
        btn.SetActive(false);
        yield return new WaitForSeconds(s);
        if (scenePhase==1)
        {
            btn.SetActive(true);
        }
    }

    private void SpawnWord()
    {
        if(wordTransforms.Count == 0)
        {
            Debug.Log("ALL DONE");
            ShowButtons(false);
            return;
        }
        Transform t = wordTransforms[0];
        wordTransforms.Remove(t);
        wordsRevealed.Add(t.gameObject);
        t.GetComponent<IdleWobble>().enabled = false;
        t.gameObject.SetActive(true);
        t.GetComponent<AudioSource>().Play();
        LeanTween.scale(t.gameObject, 3f * t.localScale, 3f).setEase(LeanTweenType.easeInOutBack);
        LeanTween.moveY(t.gameObject, t.localPosition.y + 10f, 30f).setEase(LeanTweenType.easeInCirc).setDelay(3f).setOnComplete(RemoveLastWord);
    }

    private void RemoveLastWord()
    {
        GameObject g = wordsRevealed[0];
        g.SetActive(false);
        Debug.Log("deactivating " + g.name);
        wordsRevealed.Remove(g);
    }
}

