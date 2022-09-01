using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // assuming:
    // all the rows are the same size

    public List<GameObject> row1;
    public List<GameObject> row2;
    private List<List<GameObject>> rows = new();
    public float loopSeconds = 20f;
    public AudioSource beat1;
    public AudioSource beat2;

    private float loopTimer = 0f;
    private int rowLength;
    private int rowIndex = 0;

    void Start()
    {

        rowLength = row1.Count;

        rows.Add(row1);
        //rows.Add(row2);

        for (int i = 0; i < rowLength; i++)
        {
            foreach (List<GameObject> row in rows)
            {
                row[i].AddComponent<DropSpot>();
                row[i].GetComponent<DropSpot>().SetIndex(i);
            }
        }
    }

    void Update()
    {

        if (loopTimer > loopSeconds)
        {
            rowIndex = 0;
            loopTimer = 0f;

        }

        int targetIndex = rowIndex;
        if ((loopTimer / loopSeconds) * rowLength >= targetIndex)
        {
            //Debug.Log("*** " + rowIndex + ", " + loopTimer);
            AudioSource beat = (rowIndex % 2 == 0) ? beat1 : beat2;
            beat.Play();

            foreach (List<GameObject> row in rows)
            {
                int prevIndex = (rowIndex == 0) ? rowLength - 1 : rowIndex - 1;
                UnHighlight(row[prevIndex]);

                Highlight(row[rowIndex]);
                PlayWordOn(row[rowIndex]);
            }

            rowIndex++;
        }
        loopTimer += Time.deltaTime;
    }

    private void PlayWordOn(GameObject g)
    {
        Word word = g.GetComponent<DropSpot>().GetWord();

        if (word != null && !word.GetComponent<AudioSource>().isPlaying)
        {
            //Debug.Log("plz play:  " + word.wordText);
            word.GetComponent<AudioSource>().Play();
        }
    }

    private void Highlight(GameObject g)
    {
        //Color color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);
        Color color = new Color(0.1f, 1f, 0f, 1f);
        g.GetComponent<Renderer>().material.SetColor("_Color", color);
        //Debug.Log("Highlight " + g);
    }
    private void UnHighlight(GameObject g)
    {
        Color color = new Color(1f, 1f, 1f, 1f);
        g.GetComponent<Renderer>().material.SetColor("_Color", color);
        //Debug.Log("UnHighlight " + g);
    }
}
