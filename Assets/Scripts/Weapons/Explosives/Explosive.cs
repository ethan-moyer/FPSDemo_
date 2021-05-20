﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    [SerializeField] private float damage = 0f;
    [SerializeField] private int explosionEffect = 2;
    [SerializeField] private float sphereRadius;
    [SerializeField] private LayerMask targets;

    public void Explode()
    {
        GameObject effect = ObjectPooler.SharedInstance.GetPooledObject(explosionEffect);
        if (effect != null)
        {
            effect.transform.position = transform.position;
            effect.SetActive(true);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, sphereRadius, targets);
        foreach(Collider collider in colliders)
        {
            Vector3 point = collider.ClosestPoint(transform.position);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, point - transform.position, out hit, sphereRadius))
            {
                float falloff = 1 - (Vector3.Distance(transform.position, hit.point) / sphereRadius);
                if (hit.transform == collider.transform)
                {
                    if (hit.transform.gameObject.layer == 12)
                    {
                        Prop prop = hit.transform.GetComponent<Prop>();
                        if (prop != null)
                        {
                            Debug.Log("Hit prop");
                            prop.Hit(point, hit.normal * falloff * -100f);
                        }
                    }
                }
            }
        }
        Destroy(this.gameObject);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}
