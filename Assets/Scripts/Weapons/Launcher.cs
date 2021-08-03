using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : ReloadableWeapon
{
    [SerializeField] private Projectile projectile = null;
    [SerializeField] private Vector3 launchOffset = Vector3.zero;

    protected override void Fire()
    {
        currentState = States.Firing;
        timeTillNextFire = 1 / fireRate;
        currentMagAmmo -= 1;
        effect.Play();
        TriggerAnimation.Invoke("Fire");
        combatController.UpdateAmmoText(AmmoToText());

        Rocket newProjectile = Instantiate(projectile, cam.TransformPoint(launchOffset), Quaternion.LookRotation(cam.forward)).GetComponent<Rocket>();
        newProjectile.player = combatController.GetComponent<PlayerController>();

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
