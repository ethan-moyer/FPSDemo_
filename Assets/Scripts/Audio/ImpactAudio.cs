using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VirtualAudioSource))]
public class ImpactAudio : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips = null;
    private VirtualAudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<VirtualAudioSource>();    
    }

    private void OnEnable()
    {
        audioSource.Play(clips[Random.Range(0, clips.Length)]);
    }
}
