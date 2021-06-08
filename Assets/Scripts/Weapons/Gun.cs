using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Gun : ReloadableWeapon
{
    [Header("Gun Attributes")]
    [SerializeField] protected int numberOfRays = 1;

    protected override void Fire()
    {
        currentState = States.Firing;
        timeTillNextFire = 1 / fireRate;
        currentMagAmmo -= 1;
        effect.Play();
        TriggerAnimation.Invoke("Fire");
        combatController.UpdateAmmoText(AmmoToText());

        for (int i = 0; i < numberOfRays; i++)
        {
            Vector3 randDirection = (cam.forward.normalized * maxDistance) + (cam.right.normalized * Random.Range(-1f, 1f) * coneRadius) + (cam.up.normalized * Random.Range(-1f, 1f) * coneRadius);
            ShootRay(randDirection);
        }
        combatController.gameObject.layer = 9;
    }
}