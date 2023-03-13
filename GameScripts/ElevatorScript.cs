using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorScript : MonoBehaviour , I_Interactable
{
    public float travelTime;

    public float stopTime;

    private bool moving = false;

    public Transform point1;
    public Transform point2;

    private Transform pointMoveA;
    private Transform pointMoveB;
    public Transform movePlatform;
    float timer;

    bool doMove;


    // Update is called once per frame
    void FixedUpdate()
    {
        if (doMove)
        {
            timer += Time.deltaTime / travelTime;
            if (timer < 1)
            {
                movePlatform.position = Vector3.Lerp(pointMoveA.position, pointMoveB.position, timer);
            }
        }
    }

    public void LookedAt()
    {

    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(point1.position, point2.position);
    }

    public void Interact()
    {
        if (moving != true)
        {
            StartCoroutine(MoveElevator());
        }
    }

    IEnumerator MoveElevator()
    {
        moving = true;
        doMove = true;
        timer = 0f;

      
        pointMoveA = point1;
        pointMoveB = point2;
        yield return new WaitForSeconds(travelTime);

        doMove = false;

        yield return new WaitForSeconds(stopTime);
        timer = 0f;
        doMove = true;
        pointMoveA = point2;
        pointMoveB = point1;
      
        yield return new WaitForSeconds(travelTime);
 
        doMove = false;
        moving = false;
    }


  
}
