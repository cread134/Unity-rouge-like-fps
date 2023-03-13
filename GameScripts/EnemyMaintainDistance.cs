using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMaintainDistance : MonoBehaviour
{

    public EnemyMover e_mover;

    public EnemyHealer e_Healer;

    public EnemyMover.MotionType toUsePrefered;


    public float distanceThreshold;

    

    // Update is called once per frame
    void FixedUpdate()
    {
        if(e_Healer.playerDistance < distanceThreshold)
        {
            e_mover.prefferedMove = EnemyMover.MotionType.Retreat;
        }
        else
        {
            if (e_Healer.playerDistance > distanceThreshold + 1f)
            {
                e_mover.prefferedMove = toUsePrefered;
            }
            else
            {
                e_mover.prefferedMove = EnemyMover.MotionType.BaseMove;
            }
        }

        if(e_mover.motionType == EnemyMover.MotionType.Retreat || e_mover.motionType == toUsePrefered)
        {
            e_mover.motionType = e_mover.prefferedMove;
        }
    }
}
