using UnityEngine;

public class Grabber : MonoBehaviour
{
    private GameObject selectedObject;

    public Animator dropEffect;

    private void Start()
    {
        dropEffect.enabled = false;
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

                    if (hit.collider != null && hit.collider.CompareTag("drag"))
                    {
                        //Debug.Log("Picking up: " + selectedObject);
                        selectedObject = hit.collider.gameObject;
                        selectedObject.GetComponent<Collider>().enabled = false;
                        if(selectedObject.GetComponent<Rigidbody>()!= null)
                        {
                            selectedObject.GetComponent<Rigidbody>().isKinematic = true;
                        }
                        if (selectedObject.GetComponent<IdleWobble>() != null)
                        {
                            selectedObject.GetComponent<IdleWobble>().enabled = false;
                        }
                        selectedObject.transform.rotation = Quaternion.identity;

                        foreach (Transform child in selectedObject.transform)
                        {
                            Color oldColor = child.GetComponent<Renderer>().material.color;

                            if (child.GetComponent<Renderer>().material.HasProperty("_Color"))
                            {
                                child.GetComponent<Renderer>().material.SetColor("_Color", new Color(oldColor.r, oldColor.g, oldColor.b, 0.5f));
                            }
                        }

                        Word selectedWord = selectedObject.GetComponent<Word>();
                        if (selectedWord.GetSpot() != null)
                        {
                            
                            selectedWord.GetSpot().ClearWord(selectedWord, false);
                            selectedWord.ClearSpot();
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
                        Debug.Log("Dropping "+ selectedObject.GetComponent<Word>().wordText+" on " +dropSpotObj.name+" at "+ hits[i].point);

                        Debug.Log("is this about " + hits[i].point + " ? ");
                        dropSpotObj.GetComponent<DropSpot>().PlaceWordAt(selectedObject.GetComponent<Word>(), hits[i].point);
                        selectedObject.GetComponent<Word>().SetSpot(dropSpotObj.GetComponent<DropSpot>());

                        droppedOnTarget = true;
                        selectedObject.GetComponent<Word>().UnHighlight();
                        selectedObject.GetComponent<AudioSource>().Play();
                    }
                }
            }

            if (!droppedOnTarget)
            {
                selectedObject.transform.position = new Vector3(selectedObject.transform.position.x, selectedObject.transform.position.y, 0f);
                if (selectedObject.GetComponent<IdleWobble>() != null)
                {
                    selectedObject.GetComponent<IdleWobble>().enabled = true;
                    selectedObject.GetComponent<IdleWobble>().ResetPos();
                }
            }
            if (selectedObject.GetComponent<Rigidbody>() != null)
            {
                selectedObject.GetComponent<Rigidbody>().isKinematic = droppedOnTarget;
            }
            selectedObject.GetComponent<Collider>().enabled = true;



            foreach (Transform child in selectedObject.transform)
            {
                Color oldColor = child.GetComponent<Renderer>().material.color;

                if (child.GetComponent<Renderer>().material.HasProperty("_Color"))
                {
                    child.GetComponent<Renderer>().material.SetColor("_Color", new Color(oldColor.r, oldColor.g, oldColor.b, 1f));
                }
            }
            selectedObject = null;
            
        }

        if(selectedObject != null)
        {
            //  follow the cursor

            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            Vector3 localized = selectedObject.transform.InverseTransformPoint(worldPosition);
            selectedObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, selectedObject.transform.position.z);
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
