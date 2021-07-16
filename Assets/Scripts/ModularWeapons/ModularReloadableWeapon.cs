using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularReloadableWeapon : ModularWeapon
{
    [SerializeField] private int maxMagAmmo = 0;
    [SerializeField] private float reloadTime = 1f;
    private int currentMagAmmo;

    public override int TotalAmmo
    {
        get { return currentMagAmmo + currentAmmo; }
    }

    public override void SetAmmo(int ammo)
    {
        if (ammo == -1)
            currentAmmo = maxAmmo;
        else
            currentAmmo = Mathf.Min(ammo, maxAmmo);

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

    public override void UpdateAmmoText()
    {
        UpdatingAmmoText.Invoke($"{currentMagAmmo} {currentAmmo}");
    }

    public override void PrimaryAction()
    {
        if (CurrentState == States.Idle && currentMagAmmo > 0)
        {
            if (stayZoomed == false)
                SetZoomLevel(0);

            attack.Attack(player, this, cam);
            attackTimer = 1 / attackRate;
            CurrentState = States.Firing;
            currentMagAmmo -= 1;
            UpdateAmmoText();
        }
    }

    public override void SecondaryAction()
    {
        if (CurrentState == States.Idle)
        {
            SetZoomLevel(currentZoomLevel + 1);
        }
    }

    public override void TertiaryAction()
    {
        if (CurrentState == States.Idle)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        int neededAmmo = maxMagAmmo - currentMagAmmo;
        if (neededAmmo > 0 && currentAmmo > 0)
        {
            CurrentState = States.Busy;
            SetZoomLevel(0);
            TriggeringAnimation.Invoke("Reload");

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
            CurrentState = States.Idle;
            UpdateAmmoText();
        }
    }
}
