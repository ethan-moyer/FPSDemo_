using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Gun : Weapon
{
    [Header("Gun Attributes")]
    [SerializeField] protected int maxMagAmmo = 0;
    [SerializeField] protected float reloadTime = 1f;
    [SerializeField] protected int numberOfRays = 1;
    [SerializeField] protected VisualEffect muzzleFlash = null;
    protected int currentMagAmmo = 0;

    public override void Init(PlayerCombatController combatController, Transform cam, PlayerInputReader controls, Animator animator, int startingAmmo = -1)
    {
        base.Init(combatController, cam, controls, animator, startingAmmo);
        if (maxMagAmmo <= currentAmmo)
        {
            currentMagAmmo = maxMagAmmo;
            currentAmmo -= maxMagAmmo;
        }
        else
        {
            currentMagAmmo += currentAmmo;
            currentAmmo = 0;
        }
    }

    public override void PrimaryAction()
    {
        if (IsIdle)
        {
            Fire();
        }
    }

    public override void SecondaryAction()
    {
        throw new System.NotImplementedException();
    }

    public override void ThirdAction()
    {
        if (IsIdle)
        {
            StartCoroutine(Reload());
        }
    }

    public override string AmmoToText()
    {
        return $"{currentMagAmmo} {currentAmmo}";
    }

    public void Fire()
    {
        if (timeTillNextFire == 0f && currentMagAmmo > 0)
        {
            currentState = States.Firing;
            timeTillNextFire = 1 / fireRate;
            currentMagAmmo -= 1;
            muzzleFlash.Play();
            animator.SetTrigger("Fire");
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
                        //Hit a Prop.
                        Prop prop = hit.transform.GetComponent<Prop>();
                        if (prop != null)
                        {
                            prop.Hit(hit.point, cam.forward * 10f);
                            PlaceEffect(1, hit.point, hit.normal);
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

    public IEnumerator Reload()
    {
        int neededAmmo = maxMagAmmo - currentMagAmmo;
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
            combatController.UpdateAmmoText(AmmoToText());
        }
    }

    protected void PlaceHitEffect(int index, Vector3 position, Vector3 normal)
    {
        GameObject hitEffect = ObjectPooler.SharedInstance.GetPooledObject(index);
        if (hitEffect != null)
        {
            hitEffect.transform.position = position;
            hitEffect.transform.rotation = Quaternion.Euler(normal);
            hitEffect.SetActive(true);
        }
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