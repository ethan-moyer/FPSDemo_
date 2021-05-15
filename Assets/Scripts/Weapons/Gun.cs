﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Gun : Weapon
{
    [Header("Gun Attributes")]
    [SerializeField] protected int magAmmo;
    [SerializeField] protected float reloadTime;
    [SerializeField] protected GameObject muzzleFlash;
    protected int currentMagAmmo;

    public override void SetUp(PlayerCombatController controller, Transform cam, PlayerInputReader controls, Animator animator)
    {
        base.SetUp(controller, cam, controls, animator);
        if (magAmmo <= currentAmmo)
        {
            currentMagAmmo = magAmmo;
            currentAmmo -= magAmmo;
        }
        else
        {
            currentMagAmmo += currentAmmo;
            currentAmmo = 0;
        }
    }

    public override void PrimaryAction()
    {
        if (currentState == States.Idle)
            Fire();
    }

    public override void SecondaryAction()
    {
        throw new System.NotImplementedException();
    }

    public override void ReloadAction()
    {
        if (currentState == States.Idle)
            StartCoroutine(Reload());
    }

    /// <summary>
    /// This method is used for firing the gun. Fires a single ray with a randomized direction based on the cone radius, and the response depends on the layer of the hit object.
    /// </summary>
    protected virtual void Fire()
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
            controller.gameObject.layer = 9;
        }
    }

    /// <summary>
    /// This method handles reloading the weapon. Pauses control while the animation plays and ammo is transfered into magAmmo if possible.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Reload()
    {
        int neededAmmo = magAmmo - currentMagAmmo;
        if (neededAmmo > 0 && currentAmmo > 0)
        {
            currentState = States.Busy;
            animator.SetTrigger("Reload");
            yield return new WaitForSeconds(reloadTime);

            if (neededAmmo <= currentAmmo)
            {
                currentMagAmmo += neededAmmo;
                currentAmmo -= neededAmmo;
            }
            else
            {
                currentMagAmmo += currentAmmo;
                currentAmmo = 0;
            }

            currentState = States.Idle;
        }
        controller.UpdateAmmoText($"{currentMagAmmo} {currentAmmo}");
    }

    /// <summary>
    /// This method is used for placing particle effects from the object pooler onto a hit point.
    /// </summary>
    /// <param name="index">The index of the particle in the object pooler.</param>
    /// <param name="position">The position to place the particle.</param>
    /// <param name="normal">The normal vector of the hit surface.</param>
    protected void PlaceParticle(int index, Vector3 position, Vector3 normal)
    {
        GameObject hitEffect = ObjectPooler.SharedInstance.GetPooledObject(index);
        hitEffect.transform.position = position;
        hitEffect.transform.rotation = Quaternion.LookRotation(normal);
        hitEffect.SetActive(true);
    }

    private void Update()
    {
        if (timeTillNextFire > 0f)
        {
            timeTillNextFire -= Time.deltaTime;
        }
        else if (currentState == States.Firing)
        {
            timeTillNextFire = 0f;
            currentState = States.Idle;
        }

        animator.SetFloat("Speed", controls.WalkDir.magnitude);
    }

    protected override void UpdateAmmoText()
    {
        controller.UpdateAmmoText($"{currentMagAmmo} {currentAmmo}");
    }
}
