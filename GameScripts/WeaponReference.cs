using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponReference : ScriptableObject
{
    public List<GunScript> rarityA = new List<GunScript>();
    public List<GunScript> rarityB = new List<GunScript>();
    public List<GunScript> rarityC = new List<GunScript>();
    public List<GunScript> rarityD = new List<GunScript>();
}
