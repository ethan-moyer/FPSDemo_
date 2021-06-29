using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VirtualAudioSource))]
public class Prop : MonoBehaviour
{
    [SerializeField] private AudioClipThreshold[] impactClips = null;
    private Rigidbody rb;
    private VirtualAudioSource audioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<VirtualAudioSource>();
    }

    public void Hit(Vector3 point, Vector3 force)
    {
        rb.AddForceAtPosition(force, point, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        float magnitude = rb.velocity.magnitude;
        print(magnitude);
        foreach (AudioClipThreshold group in impactClips)
        {
            if (magnitude <= group.threshold)
            {
                audioSource.Play(group.clips[Random.Range(0, group.clips.Length)]);
                break;
            }
        }
    }
}
