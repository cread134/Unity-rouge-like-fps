using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public LayerMask doorCheck;

    private bool closed = true;
    private bool locked = false;
    Animator thisAnim;

    public Material lockedmaterial;
    public Material unlockedMaterial;

    public GameObject renderDoor;

    private bool hasBeenopened = false;

    public GameObject questionIcon;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("CheckForDoorOverlap", Random.Range(0f, 1f));
        thisAnim = this.GetComponent<Animator>();
        thisAnim.SetBool("Closed", true);
        renderDoor.GetComponent<MeshRenderer>().material = unlockedMaterial;

        questionIcon.transform.rotation = Quaternion.LookRotation(Vector3.down);
    }

    private void Update()
    {
        if (locked == true)
        {
            closed = true;
        }


    }

    void CheckForDoorOverlap()//checks there aren't two door spawned in one point
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1, doorCheck);
        foreach (Collider col in hitColliders)
        {
            if(col.transform != this.transform)
            {
               Destroy(this.gameObject);
            }
        }
            
      
    }

    public void Lock()
    {
        Debug.Log("locking");
        if (locked == true)
        {
            locked = false;
            renderDoor.GetComponent<MeshRenderer>().material = unlockedMaterial;
            Open();
        }
        else
        {
            if (locked == false)
            {
                locked = true;
                renderDoor.GetComponent<MeshRenderer>().material = lockedmaterial;
                Close();
            }
        }
    }

    public void Activate()
    {

        if (closed == true && locked == false)
        {
            Open();
        }
        else
        {
            if (closed == false)
            {
                Close();
            }
        }
    }

    public void SetIcon()
    {
       //bruh this is scuffed for somereason unity bugged my code out all weird

    }

    void Open()
    {
        closed = false;
        hasBeenopened = true;

        if(questionIcon.activeSelf == true)
        {

            questionIcon.SetActive(false);
        }

        thisAnim.SetBool("Closed", false);
    }

    void Close()
    {
        closed = true;
        thisAnim.SetBool("Closed", true);
    }
}
