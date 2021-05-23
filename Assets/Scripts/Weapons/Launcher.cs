using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : ReloadableWeapon
{
    [SerializeField] private Rocket projectile;
    [SerializeField] private Vector3 launchOffset;

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

            Rocket newProjectile = Instantiate(projectile, cam.TransformPoint(launchOffset), Quaternion.LookRotation(cam.forward)).GetComponent<Rocket>();
            newProjectile.player = combatController.gameObject;

            RaycastHit hit;
            combatController.gameObject.layer = 2;
            if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance))
            {
                newProjectile.direction = (hit.point - cam.TransformPoint(launchOffset)).normalized;
            }
            else
            {
                newProjectile.direction = cam.forward;
            }
            combatController.gameObject.layer = 9;
        }
    }
}
