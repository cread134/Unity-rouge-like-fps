using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePushScript : MonoBehaviour, IAbility
{
    public Vector3 hitBox;
    public LayerMask hitMask;
    public float knockBackValue;

    void IAbility.InitializeAbility(GameObject playerHead)
    {

    }

    void IAbility.StartAbility()
    {

    }

    void IAbility.UseAbility() // calculates the sawning of the projectile
    {
        Vector3 pointVector = transform.position + transform.forward * hitBox.z / 2;
        Collider[] hitColliders = Physics.OverlapBox(pointVector, hitBox / 2, transform.rotation, hitMask);
        if (hitColliders.Length > 0)
        {
            foreach (var col in hitColliders)
            {
                if (col.transform.CompareTag("enemy"))
                {
                    col.gameObject.GetComponent<EnemyLimb>().masterObject.GetComponent<EnemyHealer>().KnockBack(knockBackValue, transform.forward);
                }
            }
        }

    }


    void IAbility.EndAbility()
    {

    }

    void IAbility.SecondInteration()
    {

    }
}
