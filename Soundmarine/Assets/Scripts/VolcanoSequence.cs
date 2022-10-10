using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolcanoSequence : MonoBehaviour
{
    public GameObject dreamArt1;
    public GameObject dreamArt2;
    public GameObject dreamer;
    public GameObject dancer;

    [Range(0.0f, 1f)]
    public float cycleSpeed = 0.025f;

    [Range(0.5f, 1.5f)]
    public float breatheSpeed = 1f;

    public AudioSource inBreath;
    public AudioSource outBreath;
    public AudioSource bubbles;
    public AudioSource creatureVoice;

    public GameObject inBtn;
    public GameObject outBtn;
    public GameObject instructions;

    public ParticleSystem rings;

    public GameObject inCount;
    public GameObject outCount;

    public List<Transform> wordTransforms;
    public List<GameObject> wordsRevealed;

    // 0: none, 1: in, 2: out
    private int btnPressed = 0;
    private int IN = 1;
    private int OUT = 2;
    private int NONE = 0;
    private bool breathingIn = false;
    private bool holdingIn = false;
    private bool holdingOut = false;
    private bool instructionsDone = false;
    private float minChestR = 8.5f;
    private float maxChestR = -3.89f;
    private float minArmsR = 0f;
    private float maxArmsR = 6.2f;
    private float minHeadR = 45f;
    private float maxHeadR = -35f;
    public GameObject dreamerChest;
    public GameObject dreamerHead;
    public GameObject dreamerLeftArm;
    public GameObject dreamerRightArm;

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

    private float breathPercent = 0f;

    private UnityEngine.Audio.AudioMixerGroup pitchBendGroup;

    // Start is called before the first frame update
    void Start()
    {
        Color textCol = instructions.GetComponent<TMPro.TextMeshProUGUI>().color;
        instructions.GetComponent<TMPro.TextMeshProUGUI>().color = new Color(textCol.r, textCol.g, textCol.b, 0f);

        inCount.GetComponent<TMPro.TextMeshProUGUI>().text = "";
        outCount.GetComponent<TMPro.TextMeshProUGUI>().text = "";

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

        Debug.Log("volcano sequence started, scenePhase is "+scenePhase);
    }

    private IEnumerator HideInstructionsAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        instructions.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (scenePhase == 1)
        {
            Color textCol = instructions.GetComponent<TMPro.TextMeshProUGUI>().color;
            if (instructions.activeSelf)
            {
                if(textCol.a <= 1f && textCol.a > 0)
                {
                    instructions.GetComponent<TMPro.TextMeshProUGUI>().color = new Color(textCol.r, textCol.g, textCol.b, Mathf.Min(1f,textCol.a + 0.01f*(instructionsDone ? -1f : 1f) ) );
                    //Debug.Log("instructions a " + textCol.a);
                }                 
            } 

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

        if (btnPressed > 0)
        {
            if(scenePhase == 1 && btnPressed == IN && breathingIn)
            {
                inBtn.GetComponent<Image>().color = new Color(0f, 0f, 1.2f * (inBreath.time / inBreath.clip.length));
                outCount.GetComponent<TMPro.TextMeshProUGUI>().text = "";
                inCount.GetComponent<TMPro.TextMeshProUGUI>().text = ""+Mathf.Floor(Time.time - inTime);

            } else if (scenePhase == 1 && btnPressed == OUT && !breathingIn)
            {
                outBtn.GetComponent<Image>().color = new Color(0f, 0f, 1.2f * (outBreath.time / outBreath.clip.length));
                inCount.GetComponent<TMPro.TextMeshProUGUI>().text = "";
                outCount.GetComponent<TMPro.TextMeshProUGUI>().text = "" + Mathf.Floor(Time.time - outTime);
            } else
            {
                inCount.GetComponent<TMPro.TextMeshProUGUI>().text = "";
                outCount.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            }

            breathVol = Mathf.Min(0.5f, breathVol + 0.001f);

        } else
        {
            inCount.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            outCount.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            inBtn.GetComponent<Image>().color = new Color(0f, 0f, 0f);
            outBtn.GetComponent<Image>().color = new Color(0f, 0f, 0f);
            breathVol = Mathf.Max(0.2f, breathVol - 0.001f);
        }
        //Debug.Log("v  " + breathVol);
        inBreath.volume = breathVol;
        outBreath.volume = breathVol;


        if (btnPressed > 0)
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


        float targetPercent = (scenePhase == 0 || scenePhase == 2)
            ? 0.5f
            : (breathingIn) 
                ? (inBreath.time / inBreath.clip.length) 
                : 1 - (outBreath.time / outBreath.clip.length);

        breathPercent = breathPercent + (targetPercent - breathPercent) / 60f;

        Vector3 chestR = new Vector3(0, 0, minChestR + (maxChestR - minChestR) * breathPercent);
        Vector3 headR = new Vector3(0, 0, minHeadR + (maxHeadR - minHeadR) * breathPercent);
        Vector3 armsR = new Vector3(0, 0, minArmsR + (maxArmsR - minArmsR) * breathPercent);

        dreamerChest.transform.rotation = Quaternion.Euler(chestR);
        dreamerHead.transform.rotation = Quaternion.Euler(headR);
        dreamerLeftArm.transform.rotation = Quaternion.Euler(armsR);
        dreamerRightArm.transform.rotation = Quaternion.Euler(armsR);


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
        btnPressed = IN;
        BreatheIn();
        Debug.Log("PRESS IN");
        if (!instructionsDone)
        {
            instructionsDone = true;
        }
    }
    public void PressOut()
    {
        btnPressed = OUT;
        BreatheOut();
        Debug.Log("PRESS OUT");
        if (!instructionsDone)
        {
            instructionsDone = true;
        }
    }
    public void ReleaseIn()
    {
        btnPressed = NONE;
        Debug.Log("RELEASE IN");
    }
    public void ReleaseOut()
    {
        btnPressed = NONE;
        Debug.Log("RELEASE OUT");

    }



    public void BreatheIn()
    {
        if (!breathingIn)
        {
            outBreath.Stop();
            breathingIn = true;
            inTime = Time.time;
            inBreath.time = 0.0001f;
            inBreath.PlayDelayed(btnPressed == IN ? 0f : 0.5f);
            if (Time.time - outTime < 2f)
            {
                breatheSpeed = 1.5f;
            }
            rings.Emit(1);

            if (btnPressed == OUT)
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
            outBreath.PlayDelayed(btnPressed == OUT? 0f : 0.5f);
            if(Time.time - inTime < 2f)
            {
                breatheSpeed = 1.5f;
            }
            rings.Emit(1);
            if (btnPressed == IN)
            {
                StartCoroutine(HideButton(inBtn, 1f));
            }
            
        }
        
    }
    public void Exit()
    {
        inBtn.SetActive(false);
        outBtn.SetActive(false);
        inCount.SetActive(false);
        outCount.SetActive(false);
        dreamArt1.SetActive(false);
        dreamArt2.SetActive(false);
    }

    public void ShowButtons(bool toShow)
    {
        Debug.Log("should we show the buttons? " + toShow);
        if (toShow)
        {
            scenePhase = 1;
            inBtn.SetActive(true);
            outBtn.SetActive(true);
            inCount.SetActive(true);
            outCount.SetActive(true);
            rings.gameObject.SetActive(true);
            rings.Play();
            Color textCol = instructions.GetComponent<TMPro.TextMeshProUGUI>().color;
            instructions.GetComponent<TMPro.TextMeshProUGUI>().color = new Color(textCol.r, textCol.g, textCol.b, 0.001f);
            //StartCoroutine(HideInstructionsAfter(5f));
        } else
        {
            scenePhase = 2;
            BreatheIn();
            inBtn.SetActive(false);
            outBtn.SetActive(false);
            rings.Stop(true,ParticleSystemStopBehavior.StopEmitting);
            dreamArt1.GetComponent<Renderer>().material.color = new Color(0,0,0,0);
            dreamArt2.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
            creatureVoice.Play();
            StartCoroutine(MoveOnAfter(10f));
        }
        Debug.Log("did we do it? scenePhase is now: " + scenePhase);
    }

    private IEnumerator MoveOnAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach(GameObject g in wordsRevealed)
        {
            g.SetActive(false);
        }
        gameObject.GetComponent<GameManager>().PanToShuttle();
        dreamer.SetActive(false);
        dancer.SetActive(true);
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
        //t.GetComponent<AudioSource>().Play();
        LeanTween.scale(t.gameObject, 2f * t.localScale, 2f).setEase(LeanTweenType.easeInOutBack);
        LeanTween.scale(t.gameObject, 1f * t.localScale, 2f).setDelay(2f).setEase(LeanTweenType.easeOutSine);
        LeanTween.moveY(t.gameObject, t.localPosition.y + 10f, 30f).setEase(LeanTweenType.easeInCirc).setDelay(3f).setOnComplete(RemoveLastWord);
        t.gameObject.GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1.2f);
        t.gameObject.GetComponent<AudioSource>().Play();
    }

    private void RemoveLastWord()
    {
        GameObject g = wordsRevealed[0];
        g.SetActive(false);
        //Debug.Log("deactivating " + g.name);
        wordsRevealed.Remove(g);
    }
}

