using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNoise : MonoBehaviour
{
    AudioSource audioSource;

    public float desiredPitch = 1f;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound(AudioClip a_clip, float pitchShift)
    {
        //  audioSource.clip = a_clip;
        audioSource.pitch = desiredPitch;
        audioSource.pitch += Random.Range(-pitchShift, pitchShift);
        audioSource.PlayOneShot(a_clip);
    }
}
