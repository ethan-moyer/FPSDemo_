using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private bool pooled = false;
    private Rigidbody rb = null;
    private float timer = 0f;
    public Vector3 direction { get; set; }
    public PlayerController player { get; set; }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        rb.velocity = direction * speed;
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            if (pooled)
                gameObject.SetActive(false);
            else
                Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject != player.gameObject)
        {
            Hit(col);
        }
    }

    protected abstract void Hit(Collision col);
}
