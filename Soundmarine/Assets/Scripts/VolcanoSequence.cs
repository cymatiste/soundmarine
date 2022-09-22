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
    public float cycleSpeed = 0.5f;

    [Range(0.5f, 1.5f)]
    public float breatheSpeed = 1f;

    public AudioSource inBreath;
    public AudioSource outBreath;

    public GameObject inBtn;
    public GameObject outBtn;

    public ParticleSystem rings;

    public List<Transform> wordTransforms;

    private Coroutine queuedBreath;

    private bool btnPressed = false;
    private bool breathingIn = false;
    
    private float breathVol = 0.1f;

    private Vector3 dreamerStartScale;



     private UnityEngine.Audio.AudioMixerGroup pitchBendGroup;

    // Start is called before the first frame update
    void Start()
    {
        pitchBendGroup = Resources.Load<UnityEngine.Audio.AudioMixerGroup>("PitchBendMixer");
        inBreath.outputAudioMixerGroup = pitchBendGroup;
        outBreath.outputAudioMixerGroup = pitchBendGroup;
        inBreath.volume = 0.1f;
        outBreath.volume = 0.1f;


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
        //dreamArt1.GetComponent<Renderer>().material.color = new Color(1f, 1f, 0.6f, (breathVol - 0.1f) * Mathf.Sin(Time.time*cycleSpeed));
        //dreamArt2.GetComponent<Renderer>().material.color = new Color(1f, 1f, 0.6f, (breathVol - 0.1f) * Mathf.Sin(Time.time*cycleSpeed));
        dreamArt1.GetComponent<Renderer>().material.color = new Color(0,0,0,0);
        dreamArt2.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);

        var emission = rings.emission;
        emission.rateOverTime = 1f*(1f - breathVol);

        inBreath.pitch = breatheSpeed;
        inBreath.outputAudioMixerGroup.audioMixer.SetFloat("pitchBend", 1f / breatheSpeed);

        outBreath.pitch = breatheSpeed;
        outBreath.outputAudioMixerGroup.audioMixer.SetFloat("pitchBend", 1f / breatheSpeed);

        if (btnPressed)
        {
            inBtn.GetComponent<Image>().color = new Color(0f, 0f, 1f * (inBreath.time / inBreath.clip.length));
            outBtn.GetComponent<Image>().color = new Color(0f, 0f, 1f * (outBreath.time / outBreath.clip.length));
            breathVol = Mathf.Min(0.5f, breathVol + 0.001f);
            
        } else
        {
            breathVol = Mathf.Max(0.1f, breathVol - 0.001f);
        }
        Debug.Log("v  " + breathVol);
        inBreath.volume = breathVol;
        outBreath.volume = breathVol;


        if (btnPressed)
        {
            Slower();
        }

        if(breathingIn && inBreath.time == 0f)
        {
            BreatheOut();
        } else if (!breathingIn && outBreath.time == 0f)
        {
            BreatheIn();
        }
        
        float inStretch = 0.2f * (inBreath.time / (inBreath.clip.length / breatheSpeed));
        float outStretch = inBreath.time > 0f ? 0f : 0.2f * (1f - (outBreath.time / (outBreath.clip.length / breatheSpeed)));
        float xScale = dreamerStartScale.x * (0.9f + inStretch + outStretch  );
        dreamer.transform.localScale = new Vector3(xScale, dreamer.transform.localScale.y, dreamer.transform.localScale.z);

        //Debug.Log(inBreath.time+" / "+outBreath.time);
    }

    private void Slower()
    {
        breatheSpeed = Mathf.Max(0.5f, breatheSpeed - 0.0001f);
        Debug.Log(breatheSpeed + "    in "+inBreath.clip.length/breatheSpeed+"     out "+outBreath.clip.length/breatheSpeed);
    }

    public void PressIn()
    {
        btnPressed = true;
        BreatheIn();

        SpawnWord();
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
            breatheSpeed = 1f;
            inBreath.time = 0.0001f;
            inBreath.PlayDelayed(1f);

            StartCoroutine(HideButton(outBtn, 2f));
            rings.Emit(1);
        }
        
    }
    public void BreatheOut()
    {
        if (breathingIn)
        {
            inBreath.Stop();
            breathingIn = false;
            breatheSpeed = 1f;
            outBreath.time = 0.0001f;
            outBreath.PlayDelayed(1f);
            rings.Emit(1);

            StartCoroutine(HideButton(inBtn, 2f));
        }
        
    }
    public void Exit()
    {
        inBtn.SetActive(false);
        outBtn.SetActive(false);
        dreamArt1.SetActive(false);
        dreamArt2.SetActive(false);
    }

    public void ShowButtons()
    {
        inBtn.SetActive(true);
        outBtn.SetActive(true);
        rings.gameObject.SetActive(true);
        rings.Play();
    }

    private IEnumerator HideButton(GameObject btn, float s)
    {
        btn.SetActive(false);
        yield return new WaitForSeconds(s);
        btn.SetActive(true);
    }

    private void SpawnWord()
    {
        Transform t = wordTransforms[0];
        wordTransforms.Remove(t);
        t.GetComponent<IdleWobble>().enabled = false;
        t.gameObject.SetActive(true);
        LeanTween.scale(t.gameObject, 1.5f * t.localScale, 3f).setEase(LeanTweenType.easeInOutBack);
        LeanTween.moveY(t.gameObject, t.localPosition.y + 10f, 30f).setEase(LeanTweenType.easeInCirc).setDelay(3f);
    }
}

