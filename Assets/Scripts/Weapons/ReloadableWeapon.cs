using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ReloadableWeapon : Weapon
{
    [Header("Reloadable")]
    [SerializeField] protected int maxMagAmmo = 0;
    [SerializeField] protected float reloadTime = 1f;
    [SerializeField] protected AudioClip fireClip = null;
    [SerializeField] protected AudioClip reloadClip = null;
    protected int currentMagAmmo = 0;

    public override int CurrentAmmo
    {
        get { return currentAmmo; }
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

    public override IEnumerator Unequip()
    {
        if (isZoomed)
            Zoom(false);
        return base.Unequip();
    }

    public override void PrimaryAction()
    {
        if (IsIdle)
        {
            if (timeTillNextFire == 0f && currentMagAmmo > 0)
            {
                if (isZoomed && !stayZoomed)
                {
                    Zoom(false);
                }
                Fire();
            }
        }
    }

    public override void SecondaryAction()
    {
        if ((IsIdle && stayZoomed == false) || ((currentState == States.Idle || currentState == States.Firing) && stayZoomed == true))
        {
            Zoom(true);
        }
    }

    public override void ThirdAction()
    {
        if (IsIdle)
        {
            if (isZoomed)
                Zoom(false);
            StartCoroutine(Reload());
        }
    }

    protected abstract void Fire();

    protected virtual void Zoom(bool shouldZoom)
    {
        if (shouldZoom == true)
        {
            if (isZoomed)
            {
                isZoomed = false;
                ChangeFOV.Invoke(-1);
            }
            else
            {
                isZoomed = true;
                ChangeFOV.Invoke(zoomFOVMultiplier);
            }
        }
        else
        {
            isZoomed = false;
            ChangeFOV.Invoke(-1);
        }
    }

    protected IEnumerator Reload()
    {
        int neededAmmo = maxMagAmmo - currentMagAmmo;
        if (neededAmmo > 0 && currentAmmo > 0)
        {
            currentState = States.Busy;
            TriggerAnimation.Invoke("Reload");

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
            UpdateAmmoText.Invoke(AmmoToText());
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
