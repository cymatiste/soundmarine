using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{

    public List<string> words;
    public List<AudioClip> clips;
    public GameObject wordPrefab;


    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i<words.Count; i++)
        {
            GameObject newWord = Instantiate(wordPrefab, new Vector3(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(25f, 35f), UnityEngine.Random.Range(-15f, 25f)), Quaternion.identity);
            newWord.GetComponent<Word>().wordText = words[i];
            newWord.GetComponent<AudioSource>().clip = clips[i];
            newWord.GetComponent<Word>().Init();
            Debug.Log("instantiated " + newWord.GetComponent<Word>().wordText);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
