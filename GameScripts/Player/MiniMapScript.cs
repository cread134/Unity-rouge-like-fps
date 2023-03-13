using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapScript : MonoBehaviour
{
    public Transform target;
    public GameObject playerIcon;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(target.position.x, 30, target.position.z);
        playerIcon.transform.rotation = target.transform.rotation;
    }
}
