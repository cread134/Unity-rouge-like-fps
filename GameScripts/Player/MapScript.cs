using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour
{
    public bool mapOpen;
    public float sensitivity;
    public float distanceFromCenter;
    public Transform target;
    public List<Transform> mapObjects = new List<Transform>();
    public PlayerControls P_cont;
    public ShootingScript s_script;
    public WeaponWhellManager w_wheelScript;
    public MeleeScript meeleeScript;
    public bool orbitCamera = false;

    [Space]
    [Header("camera settings")]
    public Camera playerCam;
    public Camera mapCam;

    [SerializeField]
    Transform focus = default;

    [SerializeField, Range(1f, 20f)]
    float distance = 5f;
    // Start is called before the first frame update
    void Start()
    {
        mapCam.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(P_cont.mapKey) && s_script.reloading == false && w_wheelScript.inWheel == false && meeleeScript.inMelee == false && PauseMenu.gamePaused == false)
        {
            MapInteract();
        }

        if (orbitCamera == true)
        {
            OrbitCamera();
        }
    }

    void OrbitCamera()
    {

    }

    void LateUpdate()
    {
        Vector3 focusPoint = focus.position;
        Vector3 lookDirection = transform.forward;
        transform.localPosition = focusPoint - lookDirection * distance;
    }

    public void MapInteract()
    {
        if(mapOpen == true)
        {
            CloseMap();
        }
        else
        {
            OpenMap();
        }
    }

    public void OpenMap()
    {
        mapOpen = true;
        playerCam.enabled = false;
        mapCam.enabled = true;
    }
    public void CloseMap()
    {
        mapOpen = false;
        playerCam.enabled = true;
        mapCam.enabled = false;
    }
}
