using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualAudioPlayer : MonoBehaviour
{
    public List<Transform> listeners;
    public float pollingRate = 1 / 60;
    [SerializeField] private GameObject audioSourcePrefab = null;
    [SerializeField] private int queueSize = 255;
    public static VirtualAudioPlayer SharedInstance;
    private Queue<AudioSource> audioSources;

    private void Awake()
    {
        if (SharedInstance == null)
        {
            SharedInstance = this;
            audioSources = new Queue<AudioSource>();
            for (int i = 0; i < queueSize; i++)
            {
                AudioSource newAudioSource = Instantiate(audioSourcePrefab, transform).GetComponent<AudioSource>();
                newAudioSource.gameObject.SetActive(false);
                audioSources.Enqueue(newAudioSource);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public AudioSource GetAudioSource()
    {
        AudioSource newSource;
        if (audioSources.Peek().gameObject.activeInHierarchy)
        {
            newSource = Instantiate(audioSourcePrefab, transform).GetComponent<AudioSource>();
            audioSources.Enqueue(newSource);
        }
        else
        {
            newSource = audioSources.Dequeue();
            audioSources.Enqueue(newSource);
        }
        return newSource;
    }

    public void Play(VirtualAudioSource virtualAudioSource)
    {
        float closestDistance = float.MaxValue;
        Transform closestListener = null;
        foreach (Transform listener in listeners)
        {
            float distance = Vector3.Distance(listener.position, virtualAudioSource.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestListener = listener;
            }
        }

        if (closestListener != null)
        {
            AudioSource audioSource = audioSources.Dequeue();
            audioSource.clip = virtualAudioSource.Clip;
            audioSource.volume = virtualAudioSource.Volume;
            audioSource.maxDistance = virtualAudioSource.Range;
            audioSource.transform.position = virtualAudioSource.transform.position - closestListener.position;
            audioSource.gameObject.SetActive(true);
            audioSource.Play();
            audioSources.Enqueue(audioSource);
        }
    }
}
