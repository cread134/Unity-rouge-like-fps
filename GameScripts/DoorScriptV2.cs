using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScriptV2 : MonoBehaviour , I_Interactable
{
    [HideInInspector]
    public bool open = false;

    public bool isLocked;

    public Animator thisAnim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LookedAt()
    {

    }

    public void Interact()
    {
        if (isLocked == false)
        {
            if (open == true)
            {
                open = false;
                Close();
            }
            else
            {
                open = true;
                Open();
            }
        }
    }

    void Open()
    {
        thisAnim.SetBool("Open", true);
    }

    void Close()
    {
        thisAnim.SetBool("Open", false);
    }

    public void Lock()
    {

    }
}
