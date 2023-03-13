using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomConnectorScript : MonoBehaviour
{
    //adds extra connections on collision

    bool hasConnected = false;

    public bool doOverride;

    public LayerMask checkLayerMask;

    [HideInInspector]
    public GameObject thisparent;

    RoomClass r_class;

    private void Start()
    {
        thisparent = transform.parent.gameObject;
        r_class = thisparent.GetComponent<RoomClass>();

        //checks for the connections
        Collider[] checkCols = Physics.OverlapSphere(this.transform.position, 1f, checkLayerMask);
        foreach (Collider col in checkCols)
        {
            if (col.transform.CompareTag("Connector") && col.transform != this.transform)
            {
                Collided();
                col.transform.gameObject.GetComponent<RoomConnectorScript>().Collided();
                return;
            }
        }
    }

    public void Collided()
    {
        //Debug.Log("collided");

        if (r_class != null && hasConnected == false && r_class.connectedPoints.Contains(this.transform) == false)
        {
            r_class.connectedPoints.Add(this.transform);
            hasConnected = true;
        }
    }
}
