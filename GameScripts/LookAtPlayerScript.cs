using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        transform.LookAt(player.transform.position);
    }
}
