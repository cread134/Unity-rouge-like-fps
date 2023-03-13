using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdcancedCameraRecoil : MonoBehaviour
{
    [Header("Reference Points")]
    public Transform recoilPosition;
    public Transform rotationPoint;
   
    [Header("Speed Settings")]
    public float positionalRecoilSpeed = 8f;
    public float rotationalRecoilSpeed = 8f;
    [Space(10)]

    public float positionalReturnSpeed = 18f;
    public float rotationalReturnSpeed = 38f;

    [Header("Amount setting")]
    public Vector3 recoilRotation = new Vector3(10, 5, 7);
    public Vector3 recoilKickBack = new Vector3(0.015f, 0f, -0.2f);


    Vector3 rotationalRecoil;
    Vector3 positionalRecoil;
    Vector3 rot;

    Vector3 returnPos = new Vector3(0, 1.36f, 0);
    Vector3 returnRot = new Vector3(0, 1.36f, 0);

    private void FixedUpdate()
    {
        rotationalRecoil = Vector3.Lerp(rotationalRecoil, returnRot, rotationalReturnSpeed * Time.deltaTime);
        positionalRecoil = Vector3.Lerp(positionalRecoil, returnPos, positionalReturnSpeed * Time.deltaTime);

        recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, positionalRecoil, positionalRecoilSpeed * Time.deltaTime);
        rot = Vector3.Slerp(rot, rotationalRecoil, rotationalRecoilSpeed * Time.fixedDeltaTime);
        rotationPoint.localRotation = Quaternion.Euler(rot);
    }


    public void Fire(Vector3 recoilRot, Vector3 positionalRecoil, float returnSpeed, float useSpeed)
    {
      //  returnRot = lastRotationRecoil;

        recoilRotation = recoilRot;
        recoilKickBack = positionalRecoil;

        rotationalRecoilSpeed = useSpeed;
        rotationalReturnSpeed = returnSpeed;

        
        rotationalRecoil += new Vector3(-recoilRotation.x, Random.Range(-recoilRotation.y, recoilRotation.y), Random.Range(-recoilRotation.z, recoilRotation.z));
        positionalRecoil += new Vector3(Random.Range(-recoilKickBack.x, recoilKickBack.x), Random.Range(-recoilKickBack.y, recoilKickBack.y), recoilKickBack.z);


    }
}
