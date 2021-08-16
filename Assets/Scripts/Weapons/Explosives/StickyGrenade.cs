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
            rb.isKinematic = true;
            rb.detectCollisions = false;
            transform.SetParent(collision.transform, true);
            if (collision.gameObject.layer == 9)
            {
                PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
                if (pc != null)
                {
                    pc.Die.AddListener(OnPlayerDie);
                }
            }
        }
    }

    private void OnPlayerDie(int playerID, int otherPlayerID)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.detectCollisions = true;
    }
}
