using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour , I_Interactable
{
    //chest colors
    public Material rarity1Col;
    public Material rarity2Col;
    public Material rarity3Col;
    public Material rarity4Col;
    public Material rarity5Col;

    public GameObject baseObj;

    public WeaponReference w_reference;
    public AmmoPickupReference ammo_ref;
    public float spawnArc;
    public float spawnForce;
    public float gunSpawnForce;

    public bool usable;

    [Range(1, 5)]
    public int rarity;

    private int ammoMultiplier;
    private int weaponRarity;// between 1 and 4
    bool doSpawnGun;

    public WeaponManager w_manager;

    // Start is called before the first frame update
    void Start()
    {

        rarity = Random.Range(1, 6);
        AssignValues();

        w_manager = GameObject.FindGameObjectWithTag("weaponManager").GetComponent<WeaponManager>();
        
    }

    void AssignValues()
    {
        Random.InitState(System.Environment.TickCount);
        switch (rarity)
        {
            case 1:
                doSpawnGun = false;
                ammoMultiplier = 1;
                weaponRarity = 1;
           //     baseObj.GetComponent<MeshRenderer>().material = rarity1Col;
                break;
            case 2:

                doSpawnGun = true;
                weaponRarity = 1;
                ammoMultiplier = 2;
              //  baseObj.GetComponent<MeshRenderer>().material = rarity2Col;
                break;

            case 3:
                doSpawnGun = true;
                weaponRarity = 2;
                ammoMultiplier = 3;
             //   baseObj.GetComponent<MeshRenderer>().material = rarity3Col;
                break;
            case 4:
                doSpawnGun = true;
                weaponRarity = 3;
                ammoMultiplier = 2;
            //    baseObj.GetComponent<MeshRenderer>().material = rarity4Col;
                break;

            case 5:
                doSpawnGun = true;
                weaponRarity = 4;
                ammoMultiplier = 3;
              //  baseObj.GetComponent<MeshRenderer>().material = rarity5Col;
                break;
        }
    }
    public void Open()
    {
        this.GetComponent<Animator>().SetTrigger("open");

    }
    public void CreateDrop() {
        if (usable == true)
        {
            //spawning weapon drop
            #region
            switch (weaponRarity)
            {
                case 1:
                    if (doSpawnGun == true)
                    {
                        GunScript drop_ref = w_reference.rarityD[Random.Range(0, w_reference.rarityD.Count)];
                        GameObject drop = drop_ref.pickupObject;
                        GameObject gunInstance = Instantiate(drop, this.transform.position, Quaternion.identity);
                        Vector3 direction = (transform.up + transform.forward).normalized;
                        gunInstance.GetComponent<Rigidbody>().AddForce(direction * gunSpawnForce, ForceMode.Impulse);
                    }
                    break;
                case 2:
                    if (doSpawnGun == true)
                    {
                        GunScript drop_ref = w_reference.rarityC[Random.Range(0, w_reference.rarityC.Count)];
                    
               

                        GameObject drop = null;

                        bool chooseObj = (Random.value > 0.5f);
                        if(chooseObj == true)
                        {
                            drop = drop_ref.pickupObject;
                        }
                        else
                        {
                            drop = w_manager.allAbilities[Random.Range(1, w_manager.allAbilities.Length)].abilityPickup;
                        }

                        GameObject gunInstance = Instantiate(drop, this.transform.position, Quaternion.identity);
                        Vector3 direction = (transform.up + transform.forward).normalized;
                        gunInstance.GetComponent<Rigidbody>().AddForce(direction * gunSpawnForce, ForceMode.Impulse);
                    }
                    break;
                case 3:
                    if (doSpawnGun == true)
                    {
                        GunScript drop_ref = w_reference.rarityB[Random.Range(0, w_reference.rarityB.Count)];
                        GameObject drop = null;

                        bool chooseObj = (Random.value > 0.5f);
                        if (chooseObj == true)
                        {
                            drop = drop_ref.pickupObject;
                        }
                        else
                        {
                            drop = w_manager.allAbilities[Random.Range(1, w_manager.allAbilities.Length)].abilityPickup;
                        }

                        GameObject gunInstance = Instantiate(drop, this.transform.position, Quaternion.identity);
                        Vector3 direction = (transform.up + transform.forward).normalized;
                        gunInstance.GetComponent<Rigidbody>().AddForce(direction * gunSpawnForce, ForceMode.Impulse);
                    }
                    break;
                case 4:
                    if (doSpawnGun == true)
                    {
                        GunScript drop_ref = w_reference.rarityA[Random.Range(0, w_reference.rarityA.Count)];
                        GameObject drop = drop_ref.pickupObject;
                        GameObject gunInstance = Instantiate(drop, this.transform.position, Quaternion.identity);
                        Vector3 direction = (transform.up + transform.forward).normalized;
                        gunInstance.GetComponent<Rigidbody>().AddForce(direction * gunSpawnForce, ForceMode.Impulse);
                    }
                    break;
            }

            #endregion

            //spawing ammo drop
            for (int i = 0; i < ammoMultiplier; i++)
            {
                GameObject ammoPickup = ammo_ref.ammoPickups[Random.Range(0, ammo_ref.ammoPickups.Count)];
                Quaternion spawnRot = Quaternion.AngleAxis(spawnArc + i * 30, transform.up);
                GameObject ammoInstance = Instantiate(ammoPickup, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.LookRotation(transform.forward));
                Vector3 direction = (transform.up * 6f + ammoInstance.transform.forward).normalized;


                ammoInstance.GetComponent<Rigidbody>().AddForce(direction * spawnForce, ForceMode.Impulse);
            }

            usable = false;
        }
    } 
    public void LookedAt()
    {
    }

    public void Interact()
    {
        Open();
    }
}
