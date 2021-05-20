using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ReloadableWeapon : Weapon
{
    [Header("Reloadable")]
    [SerializeField] protected int maxMagAmmo = 0;
    [SerializeField] protected float reloadTime = 1f;
    protected int currentMagAmmo = 0;

    public override int CurrentAmmo
    {
        get { return currentAmmo + currentMagAmmo; }
    }

    public override void SetAmmo(int newAmmo)
    {
        base.SetAmmo(newAmmo);
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

    public override string AmmoToText()
    {
        return $"{currentMagAmmo} {currentAmmo}";
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

    protected abstract void Fire();

    protected IEnumerator Reload()
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
