using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Gun : Weapon
{
    [Header("Gun Attributes")]
    [SerializeField] private int magAmmo;
    [SerializeField] private float reloadTime;
    [SerializeField] private GameObject muzzleFlash;
    private int currentMagAmmo;

    private enum States { Idle, Busy, Firing, Reloading };
    private States currentState = States.Busy;

    public override bool CanSwitch => currentState == States.Idle;

    protected override void Awake()
    {
        base.Awake();
    }

    public override IEnumerator Equip()
    {
        gameObject.SetActive(true);
        timeTillNextFire = 0f;
        currentState = States.Busy;
        yield return new WaitForSeconds(0.25f);
        currentState = States.Idle;
    }

    public override IEnumerator Unequip()
    {
        currentState = States.Busy;
        animator.SetTrigger("Unequip");
        yield return new WaitForSeconds(0.25f);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// This method is used for firing the gun. Fires a single ray with a randomized direction based on the cone radius, and the response depends on the layer of the hit object.
    /// </summary>
    public override void Fire()
    {
        if (timeTillNextFire == 0f && currentState == States.Idle && currentMagAmmo > 0)
        {
            currentState = States.Firing;
            muzzleFlash.GetComponent<VisualEffect>().Play();
            animator.SetTrigger("Fire");
            timeTillNextFire = 1 / fireRate;
            currentMagAmmo -= 1;

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
        if (currentState == States.Idle)
        {
            int neededAmmo = magAmmo - currentMagAmmo;
            if (neededAmmo > 0 && currentAmmo > 0)
            {
                currentState = States.Reloading;
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
        }
        else
        {
            yield break;
        }
    }

    /// <summary>
    /// This method is used for placing particle effects from the object pooler onto a hit point.
    /// </summary>
    /// <param name="index">The index of the particle in the object pooler.</param>
    /// <param name="position">The position to place the particle.</param>
    /// <param name="normal">The normal vector of the hit surface.</param>
    private void PlaceParticle(int index, Vector3 position, Vector3 normal)
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
    }
}
