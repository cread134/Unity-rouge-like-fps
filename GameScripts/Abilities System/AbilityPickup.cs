using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AbilityPickup : MonoBehaviour , I_Interactable
{
    public AbilityData thisAbilityData;

    public void Pickup(GameObject playerHead)
    {
        playerHead.GetComponent<AbilityManager>().SwitchActiveAbility(thisAbilityData);
    }
    public void LookedAt()
    {
    }

    public void Interact()
    {
        Pickup(GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponWhellManager>().headObject);
        Destroy(this.gameObject);
    }

}
