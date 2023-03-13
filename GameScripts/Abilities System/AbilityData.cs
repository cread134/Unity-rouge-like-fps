using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AbilityData : ScriptableObject
{
    //basic ability data
    public int abilityIndex;
    public int cost = 80;

    public float cooldown;
    public float useTime;
    public float activateTime;
    public int maxCharges;
    public bool hasSecondInteraction;
    public GameObject abilityObject;
    public GameObject abilityPickup;
    public Sprite abilityIcon;
    public string abilityName;
    public string toolTip;
}
