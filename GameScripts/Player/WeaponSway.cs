using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float intensity;
    public float smooth;

    public GameObject playerHead;
    ShootingScript shootScript;

    private Quaternion originRotation;

    private void Start()
    {
        originRotation = transform.localRotation;
        shootScript = playerHead.GetComponent<ShootingScript>();
    }

    private void Update()
    {
        //check that shooting isnt happening
        if (shootScript.isShooting == false)
        {
            UpdateSway();
        }
    }

    private void UpdateSway()
    {
        //getting controls
        float t_x_mouse = Input.GetAxis("Mouse X");
        float t_y_mouse = Input.GetAxis("Mouse Y");

        //calculate target rotation
        Quaternion t_x_adjustment = Quaternion.AngleAxis(intensity * t_x_mouse * -1f, Vector3.up);
        Quaternion t_y_adjustment = Quaternion.AngleAxis(intensity * t_y_mouse, Vector3.right);
        Quaternion targetRotation = originRotation * t_x_adjustment * t_y_adjustment;

        //rotate towards target rotation
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);
    }
}
