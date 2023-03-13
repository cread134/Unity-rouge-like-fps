using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class SaveManager : MonoBehaviour
{
    GameObject player;
    public GameObject weaponmanager;
    WeaponManager w_manager;

    public GameObject seedManager;
    public GameObject levelLoader;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Application.persistentDataPath);
        w_manager = weaponmanager.GetComponent<WeaponManager>();
    }


    public void Save()
    {
        Debug.Log("saved");

        try
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Open(Application.persistentDataPath + "/" + "SaveTest.dat", FileMode.Create);

            SaveData data = new SaveData();

            SavePlayer(data);
            SaveWorld(data);

            bf.Serialize(file, data);

            file.Close();
        }
        catch (System.Exception)
        {
            //this willhandling corrupted saves

        }
    }

    private void SavePlayer(SaveData data)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        PlayerHealth p_health = player.GetComponent<PlayerHealth>();
        WeaponWhellManager w_wheel_manager = player.GetComponent<WeaponWhellManager>();

        GameObject head = w_wheel_manager.headObject;
        AmmunitionManager ammoManager = head.GetComponent<AmmunitionManager>();

        ShootingScript shootScript = head.GetComponent<ShootingScript>();
        


        #region


        int g_1Ammo;
        if (w_wheel_manager.wheelSlots[0].GetComponent<WheelSlotScript>().slotGun != null)
        {
            g_1Ammo = w_wheel_manager.wheelSlots[0].GetComponent<WheelSlotScript>().slotGun.curAmmo;
        }
        else
        {
            g_1Ammo = 0;
        }

        int g_2Ammo;
        if (w_wheel_manager.wheelSlots[1].GetComponent<WheelSlotScript>().slotGun != null)
        {
            g_2Ammo = w_wheel_manager.wheelSlots[1].GetComponent<WheelSlotScript>().slotGun.curAmmo;
        }
        else
        {
            g_2Ammo = 0;
        }

        int g_3Ammo;
        if (w_wheel_manager.wheelSlots[2].GetComponent<WheelSlotScript>().slotGun != null)
        {
            g_3Ammo = w_wheel_manager.wheelSlots[2].GetComponent<WheelSlotScript>().slotGun.curAmmo;
        }
        else
        {
            g_3Ammo = 0;
        }

        int g_4Ammo;
        if (w_wheel_manager.wheelSlots[3].GetComponent<WheelSlotScript>().slotGun != null)
        {
            g_4Ammo = w_wheel_manager.wheelSlots[3].GetComponent<WheelSlotScript>().slotGun.curAmmo;
        }
        else
        {
            g_4Ammo = 0;
        }

        int g_5Ammo;
        if (w_wheel_manager.wheelSlots[4].GetComponent<WheelSlotScript>().slotGun != null)
        {
            g_5Ammo = w_wheel_manager.wheelSlots[4].GetComponent<WheelSlotScript>().slotGun.curAmmo;
        }
        else
        {
            g_5Ammo = 0;
        }

        int g_6Ammo;
        if (w_wheel_manager.wheelSlots[5].GetComponent<WheelSlotScript>().slotGun != null)
        {
            g_6Ammo = w_wheel_manager.wheelSlots[5].GetComponent<WheelSlotScript>().slotGun.curAmmo;
        }
        else
        {
            g_6Ammo = 0;
        }

        int g_7Ammo;
        if (w_wheel_manager.wheelSlots[6].GetComponent<WheelSlotScript>().slotGun != null)
        {
            g_7Ammo = w_wheel_manager.wheelSlots[6].GetComponent<WheelSlotScript>().slotGun.curAmmo;
        }
        else
        {
            g_7Ammo = 0;
        }

        int g_8Ammo;
        if (w_wheel_manager.wheelSlots[7].GetComponent<WheelSlotScript>().slotGun != null)
        {
            g_8Ammo = w_wheel_manager.wheelSlots[7].GetComponent<WheelSlotScript>().slotGun.curAmmo;
        }
        else
        {
            g_8Ammo = 0;
        }

        #endregion

        data.myPlayerData = new PlayerData
            (
                p_health.curhealth,
                p_health.maxHealth,

                w_wheel_manager.wheelSlots[0].GetComponent<WheelSlotScript>().gunID,
                w_wheel_manager.wheelSlots[1].GetComponent<WheelSlotScript>().gunID,
                w_wheel_manager.wheelSlots[2].GetComponent<WheelSlotScript>().gunID,
                w_wheel_manager.wheelSlots[3].GetComponent<WheelSlotScript>().gunID,


                ammoManager.quantity[0],
                ammoManager.quantity[1],
                ammoManager.quantity[2],
                ammoManager.quantity[3],

                g_1Ammo,
                g_2Ammo,
                g_3Ammo,
                g_4Ammo,
                g_5Ammo,
                g_6Ammo,
                g_7Ammo,
                g_8Ammo,

                player.GetComponent<MoneyManager>().curMoney,

                head.GetComponent<AbilityManager>().thisAbility.abilityIndex

            );

    }



    private void SaveWorld(SaveData data)
    {   
        data.myWorldData = new WorldData(seedManager.GetComponent<SetLevelSeed>().seed, levelLoader.GetComponent<LevelLoader1>().currentLevel);

        Debug.Log(data.myWorldData.worldSeed);
    }

    public void Load()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Open(Application.persistentDataPath + "/" + "SaveTest.dat", FileMode.Open);

            SaveData data = (SaveData)bf.Deserialize(file);


            file.Close();

            
            LoadWorld(data);
            StartCoroutine(PlayerLoadStaggering(data));
        }
        catch (System.Exception)
        {
            //this willhandling corrupted saves

        }
    }

    private void LoadPlayer(SaveData data)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        PlayerHealth p_health = player.GetComponent<PlayerHealth>();
        WeaponWhellManager w_wheel_manager = player.GetComponent<WeaponWhellManager>();

        GameObject head = w_wheel_manager.headObject;
        AmmunitionManager ammoManager = head.GetComponent<AmmunitionManager>();

        ShootingScript shootScript = head.GetComponent<ShootingScript>();

        //updating player health
        p_health.curhealth = data.myPlayerData.playerHealth;

        p_health.maxHealth = data.myPlayerData.maxplayerHealth;
        p_health.CreateHealthIcons(p_health.maxHealth);
        p_health.UpdateHealth();

        //updating money
        player.GetComponent<MoneyManager>().curMoney = data.myPlayerData.money;

        //updating weapons
        #region
        w_wheel_manager.wheelSlots[0].GetComponent<WheelSlotScript>().slotGun = w_manager.allGuns[data.myPlayerData.weaponSlot1Index];
        w_wheel_manager.wheelSlots[0].GetComponent<WheelSlotScript>().UpdateGunData();

        w_wheel_manager.wheelSlots[1].GetComponent<WheelSlotScript>().slotGun = w_manager.allGuns[data.myPlayerData.weaponSlot2Index];
        w_wheel_manager.wheelSlots[1].GetComponent<WheelSlotScript>().UpdateGunData();

        w_wheel_manager.wheelSlots[2].GetComponent<WheelSlotScript>().slotGun = w_manager.allGuns[data.myPlayerData.weaponSlot3Index];
        w_wheel_manager.wheelSlots[2].GetComponent<WheelSlotScript>().UpdateGunData();

        w_wheel_manager.wheelSlots[3].GetComponent<WheelSlotScript>().slotGun = w_manager.allGuns[data.myPlayerData.weaponSlot4Index];
        w_wheel_manager.wheelSlots[3].GetComponent<WheelSlotScript>().UpdateGunData();

        w_wheel_manager.NewWeaponSelected(w_wheel_manager.wheelSlots[0]);

        ammoManager.quantity[0] = data.myPlayerData.lightAmmo;
        ammoManager.quantity[1] = data.myPlayerData.mediumAmmo;
        ammoManager.quantity[2] = data.myPlayerData.projectileAmmo;
        ammoManager.quantity[3] = data.myPlayerData.shotgunAmmo;


        if (w_wheel_manager.wheelSlots[0].GetComponent<WheelSlotScript>().slotGun != null) { w_wheel_manager.wheelSlots[0].GetComponent<WheelSlotScript>().slotGun.curAmmo = data.myPlayerData.gun1Ammo; }
        if (w_wheel_manager.wheelSlots[1].GetComponent<WheelSlotScript>().slotGun != null) { w_wheel_manager.wheelSlots[1].GetComponent<WheelSlotScript>().slotGun.curAmmo = data.myPlayerData.gun2Ammo; }
        if (w_wheel_manager.wheelSlots[2].GetComponent<WheelSlotScript>().slotGun != null) { w_wheel_manager.wheelSlots[2].GetComponent<WheelSlotScript>().slotGun.curAmmo = data.myPlayerData.gun3Ammo; }
        if (w_wheel_manager.wheelSlots[3].GetComponent<WheelSlotScript>().slotGun != null) { w_wheel_manager.wheelSlots[3].GetComponent<WheelSlotScript>().slotGun.curAmmo = data.myPlayerData.gun4Ammo; }
     
        shootScript.UpdateGunInfo();
        #endregion

        //update ability
        head.GetComponent<AbilityManager>().SwitchActiveAbility(w_manager.allAbilities[data.myPlayerData.abilityIndex]);

    }

    private void LoadWorld(SaveData data)
    {
        levelLoader.GetComponent<LevelLoader1>().currentLevel = data.myWorldData.currentLevel;
        seedManager.GetComponent<SetLevelSeed>().seed = data.myWorldData.worldSeed;

        levelLoader.GetComponent<LevelLoader1>().levelSeed = data.myWorldData.worldSeed;        
    }

    IEnumerator PlayerLoadStaggering(SaveData data)
    {
        yield return new WaitForSeconds(0.5f);
        LoadPlayer(data);
    }  // loads player after loading word gen values
}
