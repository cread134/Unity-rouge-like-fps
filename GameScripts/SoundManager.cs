using UnityEngine.Audio;
using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public Sound[] sounds;

    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null) { instance = this; } else { return; }
   

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loopSound;
        }
    }

   public void Play(string name, int index) // use index when playing rapid sounds, e.g gunfire(this is for optimization) if not wanted set to 0
    {
        if (index == 0)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
            { 
                Debug.LogWarning("Sound" + name + "notFound");
                return;
            }

            s.source.Play();
        }
        else
        {
            Sound s = sounds[index];
            if (s == null)
            {
                Debug.LogWarning("Sound at index" + index + "notFound");
                return;
            }
            s.source.Play();
        }
    }

    public void SetAudioClip(string clipDestination, AudioClip sourceClip) // for sounds that are in large quantaties that change a lot (e.g gun sounds)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.clip = sourceClip;
        s.source.clip = s.clip; // sets the new audio clip
    }

}
