using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class GunScript : ScriptableObject
{
    public int weaponIndex;
    public int cost = 50;

    public int curAmmo;

    public int maxAmmo;
    public int ammoPerShot;
    public float firerate;
    public int bulletsPerShot;
    public bool raycast;
    public float raycastRange;
    public float reloadTime;
    public float accuracy;
    public bool automatic;
    public float bypassRange = 15f;
    [Header("projectile settings")]

    public GameObject projectile;
    public int amountToQueue;

    [Space]
    public GameObject weaponModel;
    public int ammoIndex;
    public string gunName;

    public GameObject pickupObject;
    public GameObject impactEffect;
    public int damage;
    public bool damageFalloff;
    public float falloffMultiplier;
    public GameObject[] enemyHitParticles;
    public bool customShooting;
    public bool hasMuzzleFlash;

    public bool sequentialReload;
    public float sequentialTime;
    [Space]
    public float drawTime = 0.2f;

    [Range( 0, 100)]
    public float hitParticlePercentage;
      
    //audio time
    [Header("AudioSettings")]
    public AudioClip shootSound;
    public AudioClip reloadClip;
    public float pitchShift;

    [Header("Recoil control")]

    public bool randomizeGunRot = false;
    public Vector3 recoilRotation = new Vector3(2f,2f,2f);
    public Vector3 recoilRotationCam = new Vector3(10, 5, 7);
    public Vector3 recoilKickBack = new Vector3(0.015f, 0f, 0);
    [Space(10)]
    public float returnSpeed = 38f;
    public float rotationSpeed = 8f;

    [Header("Trail Settings")]
    public Gradient trailColor;
    public float trailWidth = 0.5f;

    [Header("UiSettings")]
    public Sprite w_Icon;
    public Sprite crosshair;

    [Range(0f, 1f)]
    public float crosshairSizeMultiplier;

    [Range(1f, 3f)]
    public float crosshairSizeClamp = 3f;

    public float crossHairRecoil = 0.5f;
    public float crossHairReturnSpeed = .01f;
}
