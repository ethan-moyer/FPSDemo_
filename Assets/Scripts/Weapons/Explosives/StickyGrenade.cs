using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class StickyGrenade : Grenade
{
    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject != player)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            Destroy(rb);
            transform.SetParent(collision.transform, true);
        }
    }
}
