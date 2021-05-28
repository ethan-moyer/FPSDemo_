using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Explosive
{
    public Vector3 direction = Vector3.zero;
    public GameObject player = null;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float rocketRadius = 1f;
    private Rigidbody rb;
    private float timer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        rb.velocity = direction * speed;
        timer += Time.deltaTime;
        if (timer >= 10f)
            Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != player)
        {
            Explode();
        }
    }
}
