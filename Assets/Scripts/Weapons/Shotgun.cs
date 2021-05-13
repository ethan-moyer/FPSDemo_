using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Shotgun : Gun
{
    [SerializeField] private int numberOfRays = 8;

    protected override void Fire()
    {
        if (timeTillNextFire == 0f && currentMagAmmo > 0)
        {
            muzzleFlash.GetComponent<VisualEffect>().Play();
            animator.SetTrigger("Fire");
            currentState = States.Firing;
            timeTillNextFire = 1 / fireRate;
            currentMagAmmo -= 1;
            controller.UpdateAmmoText($"{currentMagAmmo} {currentAmmo}");

            RaycastHit hit;
            controller.gameObject.layer = 2;
            for (int i = 0; i < numberOfRays; ++i)
            {
                Vector3 randDirection = (cam.forward.normalized * maxRange) + (cam.right.normalized * Random.Range(-1f, 1f) * coneRadius) + (cam.up.normalized * Random.Range(-1f, 1f) * coneRadius);
                if (Physics.Raycast(cam.position, randDirection, out hit, maxRange))
                {
                    if (hit.transform.gameObject.layer == 12 && hit.transform.GetComponent<Prop>() != null)
                    {
                        //Hit a prop.
                        hit.transform.GetComponent<Prop>().Hit(hit.point, cam.forward * 10f);
                        PlaceParticle(1, hit.point, hit.normal);
                    }
                    else if (hit.transform.gameObject.layer == 10 || hit.transform.gameObject.layer == 11)
                    {
                        //Hit terrain or a platform.
                        PlaceParticle(0, hit.point, hit.normal);
                    }
                }
            }
            controller.gameObject.layer = 9;
        }
    }
}
