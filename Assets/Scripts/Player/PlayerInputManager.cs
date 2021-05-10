﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputManager : MonoBehaviour
{
    public Vector2 WalkDir { get; set; }
    public Vector2 LookDir { get; set; }
    public bool JumpDown { get; set; }
    public bool WeaponSwitchDown { get; set; }
    public bool WeaponFireDown { get; set; }
    public bool WeaponFireHeld { get; set; }
    public bool WeaponReloadDown { get; set; }

    public void OnWalk(CallbackContext ctx)
    {
        WalkDir = ctx.ReadValue<Vector2>();
    }
    
    public void OnLookX(CallbackContext ctx)
    {
        LookDir = new Vector2(ctx.ReadValue<float>(), LookDir.y);
    }

    public void OnLookY(CallbackContext ctx)
    {
        LookDir = new Vector2(LookDir.x, ctx.ReadValue<float>());
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

    public void OnReload(CallbackContext ctx)
    {
        if (ctx.started)
            WeaponReloadDown = true;
    }

    private void LateUpdate()
    {
        JumpDown = false;
        WeaponSwitchDown = false;
        WeaponFireDown = false;
        WeaponReloadDown = false;
    }
}
