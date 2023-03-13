using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRScript : MonoBehaviour , IPooledObject
{

    LineRenderer thisLineR;

    public float linetime = 0.1f;

    private void Start()
    {
        thisLineR = this.GetComponent<LineRenderer>();
    }


    public void OnObjectSpawn()
    {
        this.GetComponent<LineRenderer>().enabled = true;
        //bruh
    }


    public void MoveToPoint(Vector3 point)
    {
        this.GetComponent<LineRenderer>().SetPosition(0, this.transform.position);
        this.GetComponent<LineRenderer>().SetPosition(1, point);

        StartCoroutine(StopLineRenderer());
    }



    
    IEnumerator StopLineRenderer()
    {
        yield return new WaitForSeconds(linetime);
        this.GetComponent<LineRenderer>().enabled = false;
    }
}


