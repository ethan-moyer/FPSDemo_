using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plasma : Projectile
{
    [SerializeField] private float HPDamage = 0f;
    [SerializeField] private float SPMultiplier = 0f;

    protected override void Hit(Collision collision)
    {
        if (collision.gameObject.layer == 9)
        {
            PlayerController hitPlayer = collision.gameObject.GetComponent<PlayerController>();
            if (hitPlayer != null)
            {
                hitPlayer.DamageHit(HPDamage, SPMultiplier, collision.GetContact(0).point, player);
            }
        }
        else if (collision.gameObject.layer == 10 || collision.gameObject.layer == 12) //Hit terrain or a prop
        {
            ModularWeapon.PlaceEffect(4, collision.contacts[0].point, collision.contacts[0].normal);
        }

        gameObject.SetActive(false);
    }
}
