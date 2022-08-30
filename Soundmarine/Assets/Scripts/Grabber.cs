using UnityEngine;

public class Grabber : MonoBehaviour
{
    private GameObject selectedObject;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("left click");
            if (selectedObject == null)
            {
                // picking up a new object

                Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
                Ray ray = Camera.main.ScreenPointToRay(position);
                RaycastHit[] hits = Physics.RaycastAll(ray);

                //Debug.Log(hits.Length+" hits:");
                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit hit = hits[i];

                    if (hit.collider != null && hit.collider.CompareTag("drag"))
                    {
                        
                        selectedObject = hit.collider.gameObject;
                        selectedObject.GetComponent<Collider>().enabled = false;
                        selectedObject.GetComponent<Rigidbody>().isKinematic = true;
                        selectedObject.transform.rotation = Quaternion.identity;

                        Debug.Log("got something! " + selectedObject);

                        foreach (Transform child in selectedObject.transform)
                        {
                            Color oldColor = child.GetComponent<Renderer>().material.color;

                            if (child.GetComponent<Renderer>().material.HasProperty("_Color"))
                            {
                                child.GetComponent<Renderer>().material.SetColor("_Color", new Color(oldColor.r, oldColor.g, oldColor.b, 0.5f));
                            }
                        }


                        if (selectedObject.GetComponent<Word>().GetSpot() != null)
                        {
                            selectedObject.GetComponent<Word>().GetSpot().ClearWord();
                            selectedObject.GetComponent<Word>().ClearSpot();
                        }

                        //stop looking! we don't want to activate every grabbable thing in this line of sight, just the first one.
                        break;
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
                //Debug.Log("checking hit vs " + hits[i].collider.gameObject + ",  " + i);
                if (hits[i].collider != null)
                {
                    if (hits[i].collider.CompareTag("drop"))
                    {
                        GameObject dropSpotObj = hits[i].collider.gameObject;
                        //Debug.Log("Dropping on " + dropSpotObj);
                        selectedObject.transform.position = new Vector3(dropSpotObj.transform.position.x, dropSpotObj.transform.position.y, dropSpotObj.transform.position.z - 2f);
                        selectedObject.transform.rotation = dropSpotObj.transform.rotation;

                       
                       if (dropSpotObj.GetComponent<DropSpot>().GetWord() != null)
                       {
                           dropSpotObj.GetComponent<DropSpot>().GetWord().ClearSpot();
                           dropSpotObj.GetComponent<DropSpot>().GetWord().gameObject.GetComponent<Rigidbody>().isKinematic = false;
                       }
                       
                       dropSpotObj.GetComponent<DropSpot>().SetWord(selectedObject.GetComponent<Word>());
                        selectedObject.GetComponent<Word>().SetSpot(dropSpotObj.GetComponent<DropSpot>());

                        droppedOnTarget = true;
                    }
                }
            }

            if (!droppedOnTarget)
            {
                selectedObject.transform.position = new Vector3(selectedObject.transform.position.x, selectedObject.transform.position.y, 0f);
            }
                
            selectedObject.GetComponent<Rigidbody>().isKinematic = droppedOnTarget;
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
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            selectedObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, -.25f);
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
