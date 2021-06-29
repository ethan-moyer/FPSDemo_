using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VirtualAudioSource : MonoBehaviour
{
    [SerializeField] private AudioClip clip = null;
    [SerializeField] private AudioMixerGroup mixerGroup = null;
    [SerializeField] [Range(0f, 10f)] private float volume = 1f;
    [SerializeField] private float range = 100f;
    [SerializeField] private bool playOnStart = false;
    [SerializeField] private bool loop = false;
    private AudioSource realAudioSource = null;
    private float timer = 0f;

    public AudioClip Clip => clip;
    public float Volume => volume;
    public float Range => range;

    private void Start()
    {
        if (playOnStart)
            Play(clip);
    }

    private void UpdateLocation()
    {
        if (clip != null)
        {
            float closestDistance = float.MaxValue;
            Transform closestListener = null;
            foreach (Transform listener in VirtualAudioPlayer.SharedInstance.listeners)
            {
                float distance = Vector3.Distance(transform.position, listener.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestListener = listener;
                }
            }

            if (closestListener != null)
            {
                Vector3 lToS = transform.position - closestListener.position;
                float angle = Vector3.SignedAngle(closestListener.forward, lToS, Vector3.up) * Mathf.Deg2Rad;
                realAudioSource.transform.position = new Vector3(closestDistance * Mathf.Sin(angle), 0f, closestDistance * Mathf.Cos(angle));
            }
            else
                realAudioSource.transform.position = Vector3.zero;
        }
    }

    public void Play()
    {
        Play(clip);
    }

    public void Play(AudioClip clip)
    {
        this.clip = clip;
        realAudioSource = VirtualAudioPlayer.SharedInstance.GetAudioSource();
        realAudioSource.clip = clip;
        realAudioSource.outputAudioMixerGroup = mixerGroup;
        realAudioSource.volume = volume;
        realAudioSource.maxDistance = range;
        realAudioSource.loop = loop;
        UpdateLocation();
        realAudioSource.gameObject.SetActive(true);
        realAudioSource.Play();
    }

    private void Update()
    {
        if (realAudioSource != null && timer >= VirtualAudioPlayer.SharedInstance.pollingRate)
        {
            UpdateLocation();
            timer = 0f;
        }
        timer += Time.deltaTime;
    }
}
