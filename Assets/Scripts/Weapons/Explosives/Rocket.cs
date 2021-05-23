using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Explosive
{
    public Vector3 direction = Vector3.zero;
    public GameObject player = null;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float rocketRadius = 1f;
    private float timer = 0f;

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        if (Physics.CheckSphere(transform.position, rocketRadius))
            Explode();
        timer += Time.deltaTime;
        if (timer >= 10f)
            Destroy(this.gameObject);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, rocketRadius);
    }
}
