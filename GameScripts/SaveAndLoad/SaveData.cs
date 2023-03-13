using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class SaveData
{
    public PlayerData myPlayerData { get; set; }
    public WorldData myWorldData { get; set; }

    public SaveData()
    {

    }
}



[Serializable]
public class PlayerData
{
    public int playerHealth { get; set; }
    public int maxplayerHealth { get; set; }

    public int weaponSlot1Index { get; set; }
    public int weaponSlot2Index { get; set; }
    public int weaponSlot3Index { get; set; }
    public int weaponSlot4Index { get; set; }

    public int lightAmmo;
    public int mediumAmmo;
    public int projectileAmmo;
    public int shotgunAmmo;

    public int gun1Ammo;
    public int gun2Ammo;
    public int gun3Ammo;
    public int gun4Ammo;
    public int gun5Ammo;
    public int gun6Ammo;
    public int gun7Ammo;
    public int gun8Ammo;

    public int money;

    public int abilityIndex;

    public PlayerData
        (
        int health,
        int maxPlayerHealth,

        int weaponSlot1id,
        int weaponSlot2id,
        int weaponSlot3id,
        int weaponSlot4id,


        int l_Ammo,
        int m_Ammo,
        int p_Ammo,
        int s_Ammo,

        int g_1ammo,
        int g_2ammo,
        int g_3ammo,
        int g_4ammo,
        int g_5ammo,
        int g_6ammo,
        int g_7ammo,
        int g_8ammo,

        int thisMoney,

        int thisAblIndex
     
        )
    {
        this.playerHealth = health;

        this.weaponSlot1Index = weaponSlot1id;
        this.weaponSlot2Index = weaponSlot2id;
        this.weaponSlot3Index = weaponSlot3id;
        this.weaponSlot4Index = weaponSlot4id;


        this.lightAmmo = l_Ammo;
        this.mediumAmmo = m_Ammo;
        this.projectileAmmo = p_Ammo;
        this.shotgunAmmo = s_Ammo;

        this.gun1Ammo = g_1ammo;
        this.gun2Ammo = g_2ammo;
        this.gun3Ammo = g_3ammo;
        this.gun4Ammo = g_4ammo;
        this.gun5Ammo = g_5ammo;
        this.gun6Ammo = g_6ammo;
        this.gun7Ammo = g_7ammo;
        this.gun8Ammo = g_8ammo;

        this.money = thisMoney;
        this.abilityIndex = thisAblIndex;
    }
}

[Serializable]
public class WorldData
{
    public int worldSeed;

    public int currentLevel;

    public WorldData (int seed, int level)
    {
        this.worldSeed = seed;
        this.currentLevel = level;
    }
}