using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConnectionSript : MonoBehaviour
{
    public bool collidedTo = false;

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("didCollide");
        collidedTo = true;
    }
}
