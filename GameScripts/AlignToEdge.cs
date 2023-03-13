using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignToEdge : MonoBehaviour
{
    public Transform from;
    public Transform to;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
        
            Align(from, to);

        }

        Debug.DrawLine(from.position, from.position + from.forward * 1, Color.green, 100f);

    }

    public void Align(Transform fromTransform, Transform toTransform)
    {
        //establishing values
        Transform fromParent = fromTransform.parent.transform.parent;
        Transform pivot = fromTransform.parent;


        //align pivot to point for rotation
        float pivotDistance = Vector3.Distance(fromTransform.position, pivot.position);
        Vector3 toPosition = fromTransform.position - fromTransform.forward * pivotDistance;

        Debug.DrawLine(fromParent.position, toPosition, Color.cyan, 100f);

        Vector3 savedPos = fromParent.position;
        fromParent.position = toPosition;
        pivot.position = savedPos;

        //align to point across axis
        Vector3 pointTogo = toTransform.position + toTransform.forward * pivotDistance * 2;
        fromParent.position = pointTogo;

        //for finding angle
        float distanceFrom_To = Vector3.Distance(fromTransform.position, toTransform.position);//a (recaulculates position due to pibotChanging
        float distanceFrom_Parent = Vector3.Distance(fromTransform.position, fromParent.position);//b
        float distanceTo_Parent = Vector3.Distance(fromParent.position, toTransform.position); //c
   
        //debug
        #region 
        //debug
        Debug.DrawLine(fromTransform.position, toTransform.position, Color.black, 100f);
        Debug.DrawLine(fromTransform.position, fromParent.position, Color.yellow, 100f);
        Debug.DrawLine(toTransform.position, fromParent.position, Color.red, 100f); //important line

        Debug.DrawLine(fromTransform.position, fromTransform.position + fromTransform.forward * 1, Color.green, 100f);

        #endregion

        //rotate

        Vector3 dir = (toTransform.position - fromParent.position).normalized;
        Vector3 dir2 = (fromTransform.position - fromParent.position).normalized;
        float radNum = Vector3.Angle(dir, dir2);
        Debug.Log(radNum);
        //testdirToRotate
        fromParent.rotation = Quaternion.AngleAxis(1, Vector3.up); // find which is the right way to turn

        if (radNum != 0 || radNum != 180)
        {
            fromParent.rotation = Quaternion.AngleAxis(1, Vector3.up); // find which is the right way to turn

            if (Vector3.Distance(fromTransform.position, toTransform.position) > distanceFrom_To)
            {
                radNum *= -1f;
            }

            fromParent.rotation = Quaternion.AngleAxis(-1, Vector3.up); // find which is the right way to turn
        }
        //dorotate
        fromParent.rotation = Quaternion.AngleAxis(radNum, Vector3.up); //we must determine whether to add or subtract

        // connect through offset
        float xOffset = fromTransform.position.x - toTransform.position.x; //finds offset
        float zOffset = fromTransform.position.z - toTransform.position.z;

        fromParent.position = new Vector3(fromParent.position.x - xOffset, fromParent.position.y, fromParent.position.z - zOffset);
    }


}
