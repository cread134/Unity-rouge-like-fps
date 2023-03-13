using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerDefaults : MonoBehaviour
{
    

    public void SetDefaults()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        WeaponWhellManager w_wheel_manager = player.GetComponent<WeaponWhellManager>();
        foreach (GameObject go in w_wheel_manager.wheelSlots)
        {
            if (go.GetComponent<WheelSlotScript>().slotGun != null)
            {
                go.GetComponent<WheelSlotScript>().slotGun.curAmmo = go.GetComponent<WheelSlotScript>().slotGun.maxAmmo;
            }
            
        }

        GameObject head = w_wheel_manager.headObject;

        head.GetComponent<MoneyManager>().curMoney = head.GetComponent<MoneyManager>().startMoney;
        head.GetComponent<ShootingScript>().curGun.curAmmo = head.GetComponent<ShootingScript>().curGun.maxAmmo;
    }
}
