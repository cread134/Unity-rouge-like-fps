using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    public float rotationSpeed = 6;
    public float returnSpeed = 25;

    public Vector3 recoilRotation = new Vector3(2f, 2f, 2f);

    private Vector3 currentRotation;
    private Vector3 rot;

    ShootingScript shootScript;

    private void Start()
    {
        shootScript = transform.parent.gameObject.GetComponent<ShootingScript>();
    }

    private void FixedUpdate()
    {
        if (shootScript.isShooting == true)
        {
            currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
            rot = Vector3.Slerp(rot, currentRotation, rotationSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(rot);
        }
    }

    public void Fire(Vector3 recoil)
    {       
        recoilRotation = recoil;

        if (shootScript.curGun.randomizeGunRot == true)
        {
            currentRotation += new Vector3(Random.Range( -recoilRotation.x, recoilRotation.x), Random.Range(-recoilRotation.y, recoilRotation.y), Random.Range(-recoilRotation.z, recoilRotation.z));
        }
        else
        {
            currentRotation += new Vector3(-recoilRotation.x, recoilRotation.y, Random.Range(-recoilRotation.z, recoilRotation.z));
        }
    }

}
