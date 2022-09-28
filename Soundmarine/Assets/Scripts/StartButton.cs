using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{

    public AudioSource clickSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        SceneManager.LoadScene("Explanation");
    }

    public void Continue()
    {
        SceneManager.LoadScene("Level01_Rebuild");
    }

}
