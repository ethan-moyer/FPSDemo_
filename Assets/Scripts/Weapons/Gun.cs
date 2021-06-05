using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Gun : ReloadableWeapon
{
    [Header("Gun Attributes")]
    [SerializeField] protected float damage = 0f;
    [SerializeField] protected int numberOfRays = 1;

    protected override void Fire()
    {
        currentState = States.Firing;
        timeTillNextFire = 1 / fireRate;
        currentMagAmmo -= 1;
        effect.Play();
        triggerAnimation.Invoke("Fire");
        combatController.UpdateAmmoText(AmmoToText());

        RaycastHit hit;
        combatController.gameObject.layer = 2;
        for (int i = 0; i < numberOfRays; i++)
        {
            Vector3 randDirection = (cam.forward.normalized * maxDistance) + (cam.right.normalized * Random.Range(-1f, 1f) * coneRadius) + (cam.up.normalized * Random.Range(-1f, 1f) * coneRadius);
            if (Physics.Raycast(cam.position, randDirection, out hit, maxDistance))
            {
                if (hit.transform.gameObject.layer == 12)
                {
                    //Hit a Pickup or Prop.
                    PlaceEffect(1, hit.point, hit.normal);
                    Prop prop = hit.transform.GetComponent<Prop>();
                    if (prop != null)
                    {
                        prop.Hit(hit.point, cam.forward * 10f);
                    }
                }
                else if (hit.transform.gameObject.layer == 10)
                {
                    //Hit Terrain.
                    PlaceEffect(0, hit.point, hit.normal);
                }
            }
        }
        combatController.gameObject.layer = 9;
    }
}