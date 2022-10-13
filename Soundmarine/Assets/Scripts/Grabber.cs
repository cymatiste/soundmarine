using UnityEngine;

public class Grabber : MonoBehaviour
{
    private GameObject selectedObject;

    public Animator dropEffect;

    public AudioSource grabSound;
    public AudioSource dropSound;
    public AudioSource softGrabSound;

    private Vector3 dragPoint;

    private void Start()
    {
        if (dropEffect != null)
        {
            dropEffect.enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //dropEffect.enabled = true;
            //Debug.Log("click");
            //dropEffect.Play("Drop", 0);


            if (selectedObject == null)
            {
                // picking up a new object

                Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
                Ray ray = Camera.main.ScreenPointToRay(position);
                RaycastHit[] hits = Physics.RaycastAll(ray);

                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit hit = hits[i];

                    if (hit.collider != null && hit.collider.CompareTag("fish"))
                    {
                        hit.collider.gameObject.GetComponent<Fish>().Wobble();
                    } else if (hit.collider != null && hit.collider.CompareTag("drag"))
                    {
                        selectedObject = hit.collider.gameObject;
                        dragPoint = selectedObject.transform.position - hit.point;
                        Debug.Log("DRAGPOINT: h.p " + hit.point+", stp "+selectedObject.transform.position);
                        DropSpot lds = selectedObject.GetComponent<Word>().GetLastSpot();
                        DropDot ldd = selectedObject.GetComponent<Word>().GetLastDot();
                        DropSpot ds = selectedObject.GetComponent<Word>().GetSpot();
                        DropDot dd = selectedObject.GetComponent<Word>().GetDot();


                        //Debug.Log("Picking up: " + selectedObject);
                        //Debug.Log("          , spot: " + (ds == null ? "NULL" : ds.gameObject.name));
                        //Debug.Log("          , dot: " + (dd == null ? "NULL" : dd.gameObject.name));
                        //Debug.Log("          , prevSpot: " + (lds == null ? "NULL" : lds.gameObject.name));
                        //Debug.Log("          , prevDot: " + (ldd == null ? "NULL" : ldd.gameObject.name));

                        selectedObject.GetComponent<Word>().PickUp();

                        /*
                        foreach (Transform child in selectedObject.transform)
                        {
                            Color oldColor = child.GetComponent<Renderer>().material.color;

                            if (child.GetComponent<Renderer>().material.HasProperty("_Color"))
                            {
                                child.GetComponent<Renderer>().material.SetColor("_Color", new Color(oldColor.r, oldColor.g, oldColor.b, 0.5f));
                            }
                        }
                        */

                        Word selectedWord = selectedObject.GetComponent<Word>();
                        if (selectedWord.GetSpot() != null)
                        {
                            softGrabSound.Play();

                            selectedWord.GetSpot().ClearWord(selectedWord, false, false);
                        } else
                        {
                            grabSound.Play();
                        }

                        //stop looking! we don't want to activate every grabbable thing in this line of sight, just the first one.
                        break;
                    } else
                    {
                        //Debug.Log("not draggable: " + selectedObject);
                    }
                
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {

            if(selectedObject == null)
            {
                return;
            }

            bool droppedOnTarget = false;

            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            Ray ray = Camera.main.ScreenPointToRay(position);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            for (int i=0; i<hits.Length; i++)
            {
                if (hits[i].collider != null)
                {
                    if (hits[i].collider.CompareTag("drop"))
                    {

                        // dropping

                        GameObject dropSpotObj = hits[i].collider.gameObject;

                        Vector3 dropPoint = hits[i].point;

                        //Debug.Log("Dropping "+ selectedObject.GetComponent<Word>().wordText+" on " +dropSpotObj.name+" at "+ hits[i].point);


                        Debug.Log("dropSpotObj: " + dropSpotObj + ", DS: " + dropSpotObj.GetComponent<DropSpot>() + ", hit: " + i+" of "+hits.Length + ", " + dropPoint);
                        

                        bool placed = dropSpotObj.GetComponent<DropSpot>().PlaceWordAt(selectedObject.GetComponent<Word>(), dropPoint);

                        if (placed)
                        {
                            selectedObject.GetComponent<Word>().SetSpot(dropSpotObj.GetComponent<DropSpot>());
                            droppedOnTarget = true;
                            selectedObject.GetComponent<Word>().UnHighlight();
                            selectedObject.GetComponent<AudioSource>().Play();
                            dropSound.Play();
                        }
                    }
                }
            }

            if (!droppedOnTarget)
            {
                Debug.Log("!droppedOnTarget");
                selectedObject.transform.position = new Vector3(selectedObject.transform.position.x, selectedObject.transform.position.y, 0f);
                selectedObject.GetComponent<Word>().PutDown();
                DropSpot lastSpot = selectedObject.GetComponent<Word>().GetLastSpot();
                if(lastSpot != null)
                {
                    lastSpot.SpaceAllEvenly(lastSpot.transform.position);
                }
                
            }

            /*
            foreach (Transform child in selectedObject.transform)
            {
                Color oldColor = child.GetComponent<Renderer>().material.color;

                if (child.GetComponent<Renderer>().material.HasProperty("_Color"))
                {
                    child.GetComponent<Renderer>().material.SetColor("_Color", new Color(oldColor.r, oldColor.g, oldColor.b, 1f));
                }
            }
            */
            selectedObject = null;
            
        }

        if(selectedObject != null)
        {
            //  follow the cursor

            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);

            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            Vector3 localized = selectedObject.transform.InverseTransformPoint(worldPosition);
            selectedObject.transform.position = new Vector3(worldPosition.x + dragPoint.x, worldPosition.y + dragPoint.y, selectedObject.transform.position.z);
        }
   
    }

    private RaycastHit CastRay()
    {
        Vector3 screenMousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);
        RaycastHit hit;
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit);

        return hit;
    }
}
