using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactScript : MonoBehaviour
{

    public AudioSource audioSource;

    public AudioClip[] clip;
    // Start is called before the first frame update
    void Awake()
    {
        audioSource.PlayOneShot(clip[Random.Range(0, clip.Length)]);
        Destroy(this.gameObject, 1f);
    }

}
