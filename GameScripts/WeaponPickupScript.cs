using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickupScript : MonoBehaviour , I_Interactable
{

    public GunScript thisGun;
    public int dps;
    public string weaponName;
    private WeaponWhellManager weaponWheelManager;

    private void Start()
    {
       
        dps = (int)(1 / thisGun.firerate) * thisGun.damage * thisGun.bulletsPerShot;
        weaponName = thisGun.gunName;
    }

    public void LookedAt()
    {
        weaponWheelManager = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponWhellManager>();
    }

    public void Interact()
    {
        weaponWheelManager.WeaponPickup(thisGun, transform.gameObject);
        
    }
}
