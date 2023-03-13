using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDetector : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.transform.SetParent(transform.parent, true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {

            other.transform.SetParent(null, true);

        }
    }
}