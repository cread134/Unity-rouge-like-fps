using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    public Transform pointToSpawn;

    public PortalScript linkedPortal;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Teleport(GameObject player)
    {
        player.transform.position = linkedPortal.pointToSpawn.position;
        Quaternion lookRot = Quaternion.LookRotation(transform.forward);
        player.GetComponent<WeaponWhellManager>().headObject.GetComponent<MouseLook>().targetCharacterDirection = linkedPortal.pointToSpawn.transform.rotation.eulerAngles * -1;
       
        //  player.transform.rotation = Quaternion.LookRotation(transform.forward);
        // player.GetComponent<WeaponWhellManager>().headObject.GetComponent<MouseLook>().targetCharacterDirection.y = lookRot.eulerAngles.y;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Teleport(other.transform.gameObject);
        }
    }
}
