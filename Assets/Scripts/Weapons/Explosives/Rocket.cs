using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Explosive
{
    public Vector3 direction = Vector3.zero;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float rocketRadius = 1f;

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
        if (Physics.CheckSphere(transform.position, rocketRadius))
        {
            Explode();
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, rocketRadius);
    }
}
