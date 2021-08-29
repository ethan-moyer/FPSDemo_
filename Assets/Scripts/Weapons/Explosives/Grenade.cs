using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Explosive))]
public class Grenade : MonoBehaviour
{
    [SerializeField] private float explodeTime = 3f;
    private bool timerStarted = false;
    private Explosive explosive = null;
    private Rigidbody rb = null;
    private VirtualAudioSource audioSource = null;
    public PlayerController player { get; set; }

    private void Awake()
    {
        explosive = GetComponent<Explosive>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<VirtualAudioSource>();
    }

    public void SetUp(PlayerController player, Vector3 velocity)
    {
        this.player = player;
        explosive.player = player;
        rb.velocity = velocity;
        Physics.IgnoreCollision(GetComponent<Collider>(), player.GetComponent<Collider>());
    }
    
    private IEnumerator ExplodeTimer()
    {
        timerStarted = true;
        yield return new WaitForSeconds(explodeTime);
        explosive.Explode();
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        audioSource.Play();
        if (timerStarted == false)
            StartCoroutine(ExplodeTimer());
    }
}
