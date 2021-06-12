using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualAudioSource : MonoBehaviour
{
    [SerializeField] private AudioClip clip = null;
    [SerializeField] [Range(0f, 10f)] private float volume = 1f;
    [SerializeField] private float range = 100f;

    public AudioClip Clip => clip;
    public float Volume => volume;
    public float Range => range;

    public void Play(AudioClip clip)
    {
        this.clip = clip;
        Play();
    }

    public void Play()
    {
        if (clip != null)
            VirtualAudioPlayer.SharedInstance.Play(this);
    }
}
