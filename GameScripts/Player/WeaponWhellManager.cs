using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;


public class WeaponWhellManager : MonoBehaviour
{

    public GameObject weaponWheel;
    PlayerControls playerCont;

    [HideInInspector]
    public bool inWheel;

    public GameObject headObject;
    private MouseLook m_Look;
    public Vector2 slotSize = new Vector2(0.5f, 0.5f);

    public List<GameObject> wheelSlots = new List<GameObject>();

    [HideInInspector]
    public GameObject selectedSlot;

    [HideInInspector]
    public bool removeMode;
    private GunScript gunToSwitch;
    public GameObject removeModeNotifier;

    private GameObject pickup_Item;

    //the meeleee script
    MeleeScript m_Script;
    public float timeLerpMultiplier;
    public bool overrideTime = false;

    public PostProcessVolume blurVolume;
    // Start is called before the first frame update
    void Start()
    {
        //getting the controls
        GameObject controlsManager = GameObject.FindGameObjectWithTag("ControlsManager");
        playerCont = controlsManager.GetComponent<PlayerControls>();
        m_Look = headObject.GetComponent<MouseLook>();
        m_Script = headObject.GetComponent<MeleeScript>();
        selectedSlot = wheelSlots[0];
        m_Look.lockCursor = true;

        removeModeNotifier.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(targetedSlot);
        if (Input.GetKeyDown(playerCont.accessWeaponWheel) && removeMode != true && PauseMenu.gamePaused == false)
        {
            inWheel = true;
        }
        else
        {
            if (Input.GetKeyUp(playerCont.accessWeaponWheel))
            {
                inWheel = false;
                Cursor.visible = true;
            }
        }

        if(removeMode == true)
        {
            inWheel = true;

            //ui updates
            removeModeNotifier.SetActive(true);

            if (Input.GetKeyDown(playerCont.accessWeaponWheel))
            {
                removeMode = false;
                inWheel = false;
            }
        }
        else
        {
            removeModeNotifier.SetActive(false);
        }

        if(inWheel == true)
        {
            m_Look.lockCursor = false;
            m_Look.canLook = false;
            Cursor.visible = true;
            weaponWheel.SetActive(true);
            headObject.GetComponent<ShootingScript>().canShoot = false;

            if(overrideTime == false && PauseMenu.gamePaused == false)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, 0.3f, Time.deltaTime * timeLerpMultiplier);
            }

           // blurVolume.enabled = true;
        }
        else
        {
          //  blurVolume.enabled = false;

            if (overrideTime == false && PauseMenu.gamePaused == false)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, Time.deltaTime * timeLerpMultiplier);
            }

            m_Look.canLook = true;

            //making sure we dont set the ability to shoot during melee attacks
            if (headObject.GetComponent<MeleeScript>().inMelee == false)
            {
                headObject.GetComponent<ShootingScript>().canShoot = true;
            }


            foreach (GameObject g in wheelSlots)
            {
               g.GetComponent<RectTransform>().localScale = g.GetComponent<WheelSlotScript>().scale;
            }
            weaponWheel.SetActive(false);
            m_Look.lockCursor = true;

        }

        //changing weapons from keys
        #region
        if (Input.GetKeyDown(playerCont.weapon1) && inWheel == false)
        {
            wheelSlots[0].SetActive(true);
            if(wheelSlots[0].GetComponent<WheelSlotScript>().slotGun != null)
            {
                NewWeaponSelected(wheelSlots[0]);
            }      
        }
        if (Input.GetKeyDown(playerCont.weapon2) && inWheel == false)
        {
            wheelSlots[1].SetActive(true);
            if (wheelSlots[1].GetComponent<WheelSlotScript>().slotGun != null)
            {
                NewWeaponSelected(wheelSlots[1]);
            }

        }
        if (Input.GetKeyDown(playerCont.weapon3) && inWheel == false)
        {
            wheelSlots[2].SetActive(true);
            if (wheelSlots[2].GetComponent<WheelSlotScript>().slotGun != null)
            {
                NewWeaponSelected(wheelSlots[2]);
            }
        }
        if (Input.GetKeyDown(playerCont.weapon4) && inWheel == false)
        {
            wheelSlots[3].SetActive(true);
            if (wheelSlots[3].GetComponent<WheelSlotScript>().slotGun != null)
            {
                NewWeaponSelected(wheelSlots[3]);
            }
        }
        if (Input.GetKeyDown(playerCont.weapon5) && inWheel == false)
        {
            wheelSlots[4].SetActive(true);
            if (wheelSlots[4].GetComponent<WheelSlotScript>().slotGun != null)
            {
                NewWeaponSelected(wheelSlots[4]);
            }
        }
        if (Input.GetKeyDown(playerCont.weapon6) && inWheel == false)
        {
            wheelSlots[5].SetActive(true);
            if (wheelSlots[5].GetComponent<WheelSlotScript>().slotGun != null)
            {
                NewWeaponSelected(wheelSlots[5]);
            }
        }
        if (Input.GetKeyDown(playerCont.weapon7) && inWheel == false)
        {
            wheelSlots[6].SetActive(true);
            if (wheelSlots[6].GetComponent<WheelSlotScript>().slotGun != null)
            {
                NewWeaponSelected(wheelSlots[6]);
            }
        }
        if (Input.GetKeyDown(playerCont.weapon8) && inWheel == false)
        {
            wheelSlots[7].SetActive(true);
            if (wheelSlots[7].GetComponent<WheelSlotScript>().slotGun != null)
            {
                NewWeaponSelected(wheelSlots[7]);
            }
        }
        #endregion
    }

    public void NewWeaponSelected(GameObject slot)
    {
        selectedSlot = slot;
        switch (removeMode)
        {
            case true:
                
                WheelSlotScript w_slotScr = selectedSlot.GetComponent<WheelSlotScript>();
                Instantiate(w_slotScr.slotGun.pickupObject, this.transform.position, Quaternion.identity);
                w_slotScr.slotGun = gunToSwitch;
                w_slotScr.UpdateGunData();
                ShootingScript shoot2Script = headObject.GetComponent<ShootingScript>();
                shoot2Script.LoadNewGun(w_slotScr.slotGun);

                if (pickup_Item != null)
                {
                    Destroy(pickup_Item);
                }
                inWheel = false;
                removeMode = false;
              //  this.GetComponent<Animator>().SetTrigger("Redraw");
                break;

           case false:

                
                ShootingScript shootScript = headObject.GetComponent<ShootingScript>();
                if (slot.GetComponent<WheelSlotScript>().slotGun != shootScript.curGun)
                {
                    inWheel = false;
                    shootScript.LoadNewGun(slot.GetComponent<WheelSlotScript>().slotGun);
                   // this.GetComponent<Animator>().SetTrigger("Redraw");
                    shootScript.reloading = false;
                    shootScript.sequentialinprocess = false;
                    AbilityManager abManager = headObject.GetComponent<AbilityManager>();
                    abManager.canUseAbility = true;
                }

                break;
        }
    }    

    public void WeaponPickup(GunScript targetGun, GameObject pickupItem)
    {
        Debug.Log("pickup");
        pickup_Item = pickupItem;
        for (int i = 0; i < 4; i++)
            {
            WheelSlotScript slot_instance = wheelSlots[i].GetComponent<WheelSlotScript>();
            if(slot_instance.slotGun == targetGun)
            {
                break;
            }
            if (slot_instance.slotGun == null){
                slot_instance.slotGun = targetGun;
                slot_instance.UpdateGunData();
                if (pickupItem != null)
                {
                    Destroy(pickupItem);
                }
                break;
            }
            if(i == 3)
            {
                removeMode = true;
                gunToSwitch = targetGun;
            }
        }
    }
}
