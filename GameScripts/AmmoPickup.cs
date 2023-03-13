using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{

    public int ammount;
    public int index;


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.transform.GetComponent<PlayerHealth>().head.GetComponent<AmmunitionManager>().PickupAmmo(ammount, index);
            Destroy(this.gameObject);
        }
    }


 
}
