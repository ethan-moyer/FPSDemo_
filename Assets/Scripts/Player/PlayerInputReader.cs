using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputReader : MonoBehaviour
{
    public Vector2 WalkDir { get; private set; }
    public Vector2 LookDir { get; private set; }
    public bool JumpDown { get; private set; }
    public bool WeaponSwitchDown { get; private set; }
    public bool WeaponFireDown { get; private set; }
    public bool WeaponZoomDown { get; private set; }
    public bool WeaponFireHeld { get; private set; }
    public bool WeaponReloadDown { get; private set; }
    public bool WeaponInteractDown { get; private set; }
    public bool ThrowGrenadeDown { get; private set; }
    public bool SwitchGrenadeDown { get; private set; }

    public void OnWalk(CallbackContext ctx)
    {
        WalkDir = ctx.ReadValue<Vector2>();
    }
    
    public void OnLook(CallbackContext ctx)
    {
        LookDir = ctx.ReadValue<Vector2>();
    }

    public void OnJump(CallbackContext ctx)
    {
        if (ctx.started)
            JumpDown = true;
    }

    public void OnWeaponSwitch(CallbackContext ctx)
    {
        if (ctx.started)
            WeaponSwitchDown = true;
    }

    public void OnFire(CallbackContext ctx)
    {
        if (ctx.started)
        {
            WeaponFireDown = true;
            WeaponFireHeld = true;
        }
        else if (ctx.canceled)
        {
            WeaponFireHeld = false;
        }
    }

    public void OnWeaponZoom(CallbackContext ctx)
    {
        if (ctx.started)
            WeaponZoomDown = true;
    }

    public void OnReload(CallbackContext ctx)
    {
        if (ctx.started)
            WeaponReloadDown = true;
    }

    public void OnInteract(CallbackContext ctx)
    {
        if (ctx.performed)
            WeaponInteractDown = true;
    }

    public void OnThrowGrenade(CallbackContext ctx)
    {
        if (ctx.started)
            ThrowGrenadeDown = true;
    }

    public void OnSwitchGrenade(CallbackContext ctx)
    {
        if (ctx.started)
            SwitchGrenadeDown = true;
    }

    private void LateUpdate()
    {
        JumpDown = false;
        WeaponSwitchDown = false;
        WeaponFireDown = false;
        WeaponZoomDown = false;
        WeaponReloadDown = false;
        WeaponInteractDown = false;
        ThrowGrenadeDown = false;
        SwitchGrenadeDown = false;
    }
}
