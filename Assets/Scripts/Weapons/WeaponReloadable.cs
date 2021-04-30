using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponReloadable : Weapon
{
    private int availableAmmo;

    public override void SetUp(PlayerCombatController controller, Transform cam, PlayerInputManager controls)
    {
        base.SetUp(controller, cam, controls);
        if (currentAmmo > data.magAmmo)
        {
            availableAmmo = data.magAmmo;
            currentAmmo -= data.magAmmo;
        }
    }
}
