using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolcanoSequence : MonoBehaviour
{
    public GameObject dreamArt1;
    public GameObject dreamArt2;

    [Range(0.0f, 1f)]
    public float cycleSpeed = 0.5f;

    [Range(0.5f, 1.5f)]
    public float breatheSpeed = 1f;

    public AudioSource inBreath;
    public AudioSource outBreath;

    public GameObject inBtn;
    public GameObject outBtn;

    private bool breathingIn = false;
    private bool breathingOut = false;

    private UnityEngine.Audio.AudioMixerGroup pitchBendGroup;

    // Start is called before the first frame update
    void Start()
    {
        pitchBendGroup = Resources.Load<UnityEngine.Audio.AudioMixerGroup>("PitchBendMixer");
        inBreath.outputAudioMixerGroup = pitchBendGroup;
        outBreath.outputAudioMixerGroup = pitchBendGroup;

        BreatheIn();
    }

    // Update is called once per frame
    void Update()
    {
        dreamArt1.GetComponent<Renderer>().material.color = new Color(1f, 1f, 0.6f, 0.4f + 0.5f * Mathf.Sin(Time.time*cycleSpeed));
        dreamArt2.GetComponent<Renderer>().material.color = new Color(1f, 1f, 0.6f, 0.4f + 0.5f * Mathf.Cos(Time.time*cycleSpeed));

        inBreath.pitch = breatheSpeed;
        inBreath.outputAudioMixerGroup.audioMixer.SetFloat("pitchBend", 1f / breatheSpeed);

        outBreath.pitch = breatheSpeed;
        outBreath.outputAudioMixerGroup.audioMixer.SetFloat("pitchBend", 1f / breatheSpeed);

        inBtn.GetComponent<Image>().color = new Color(0f, 0f, 1f*(inBreath.time/inBreath.clip.length));
        outBtn.GetComponent<Image>().color = new Color(0f, 0f, 1f * (outBreath.time/outBreath.clip.length));

        Debug.Log(inBreath.time);
    }

    public void BreatheIn()
    {
        Debug.Log("in");
        float delay = 0f;
        if (outBreath.isPlaying && outBreath.time < outBreath.clip.length - 0.5f)
        {
            outBreath.time = outBreath.clip.length - 0.5f;
            delay = 0.5f;
        }
        inBreath.PlayDelayed(delay);
        breathingIn = true;
    }
    public void BreatheOut()
    {
        Debug.Log("out");
        float delay = 0f;
        if (inBreath.isPlaying && inBreath.time < inBreath.clip.length - 0.5f)
        {
            inBreath.time = inBreath.clip.length - 0.5f;
            delay = 0.5f;
        }
        outBreath.PlayDelayed(delay);
        breathingOut = true;
    }

    public void Exit()
    {
        inBtn.SetActive(false);
        outBtn.SetActive(false);
    }

}
