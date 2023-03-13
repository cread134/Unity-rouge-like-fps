using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickupScript : MonoBehaviour
{

    public LayerMask pickupMask;
    PlayerControls playerCont;

    public float pickupRange;

    public GameObject body;
    WeaponWhellManager weaponWheelManag;
    public GameObject interactObj;

    public GameObject gunInfoHolder;
    public GameObject dpsinfo;
    public GameObject gunNameObject;

    TextMeshProUGUI dpsText;
    TextMeshProUGUI gunName;

    private bool touching;
    private bool touched;


    // Start is called before the first frame update
    void Start()
    {
        GameObject controlsManager = GameObject.FindGameObjectWithTag("ControlsManager");
        playerCont = controlsManager.GetComponent<PlayerControls>();
        weaponWheelManag = body.GetComponent<WeaponWhellManager>();

        interactObj.SetActive(false);

        dpsText = dpsinfo.GetComponent<TextMeshProUGUI>();
        gunName = gunNameObject.GetComponent<TextMeshProUGUI>();
        gunInfoHolder.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(this.transform.position, transform.forward, out hit, pickupRange, pickupMask) && !weaponWheelManag.inWheel)
        {

            interactObj.SetActive(true);
            string name = hit.transform.tag;

            switch (name)
            {
                case "Interactable":
                    I_Interactable objInterface = hit.transform.gameObject.GetComponent<I_Interactable>();
                    objInterface.LookedAt();

                    if (Input.GetKeyDown(playerCont.pickUp) && this.GetComponent<ShootingScript>().reloading == false)
                    {
                        objInterface.Interact();
                    }
                    break;
                case "weapon pickup":
                    WeaponPickupScript w_pickup = hit.transform.gameObject.GetComponent<WeaponPickupScript>();
                    gunInfoHolder.SetActive(true);
                    dpsText.text = "DPS " + w_pickup.dps.ToString();
                    gunName.text = w_pickup.weaponName;

                    if (Input.GetKeyDown(playerCont.pickUp) && this.GetComponent<ShootingScript>().reloading == false)
                    {
                        GunScript g_Script = w_pickup.thisGun;
                        weaponWheelManag.WeaponPickup(g_Script, hit.transform.gameObject);
                    }
                    break;

                case "chest":
                    gunInfoHolder.SetActive(false);
                    if (Input.GetKeyDown(playerCont.pickUp))
                    {            
                        hit.transform.GetComponent<ChestScript>().Open();
                    }
                    break;

                case "endLevel":
                    gunInfoHolder.SetActive(false);
                    EndPedestool endPedestoolScript = hit.transform.gameObject.GetComponent<EndPedestool>();
                    endPedestoolScript.LookedAt();
                    if (Input.GetKeyDown(playerCont.pickUp))
                    {
                        endPedestoolScript.NextLevel();
                    }
                    break;

                case "door":
                    gunInfoHolder.SetActive(false);
                    if (Input.GetKeyDown(playerCont.pickUp))
                    {
                
                        hit.transform.gameObject.GetComponent<DoorScript>().Activate();
                    }

                    break;

                case "abilityPickup":

                    AbilityPickup ablPickup = hit.transform.gameObject.GetComponent<AbilityPickup>();
                    gunInfoHolder.SetActive(true);
                    gunName.text = ablPickup.thisAbilityData.abilityName;
                    dpsText.text = ablPickup.thisAbilityData.toolTip;

                    if (Input.GetKeyDown(playerCont.pickUp) && ablPickup.thisAbilityData != this.GetComponent<AbilityManager>().thisAbility)
                    {
                        ablPickup.Pickup(this.gameObject);
                        Destroy(hit.transform.gameObject);
                        Debug.Log("ability picked up");
                    }
                    break;
            }
 
        }
        else
        {
            interactObj.SetActive(false);
            
            gunInfoHolder.SetActive(false);
        }
    }


}
