using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : ReloadableWeapon
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private float launchOffset;

    protected override void Fire()
    {
        if (timeTillNextFire == 0f && currentMagAmmo > 0)
        {
            currentState = States.Firing;
            timeTillNextFire = 1 / fireRate;
            currentMagAmmo -= 1;
            effect.Play();
            animator.SetTrigger("Fire");
            combatController.UpdateAmmoText(AmmoToText());

            Rocket newProjectile = Instantiate(projectile, cam.position + cam.forward.normalized * launchOffset, projectile.transform.rotation).GetComponent<Rocket>();
            newProjectile.direction = cam.forward;
        }
    }
}
