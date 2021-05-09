using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputManager : MonoBehaviour
{
    /*[SerializeField] private bool singlePlayer = false;
    private Controls input;
    private InputUser devices;
    public Vector2 WalkDir { get { return input.Player.Walk.ReadValue<Vector2>(); } }
    public Vector2 LookDir { get { return new Vector2(input.Player.LookX.ReadValue<float>(), input.Player.LookY.ReadValue<float>()); } }
    public bool JumpDown { get { return GetButtonDown(input.Player.Jump); } }
    public bool WeaponSwitchDown { get { return GetButtonDown(input.Player.WeaponSwitch); } }
    public bool WeaponFireDown { get { return GetButtonDown(input.Player.Fire); } }
    public bool WeaponFireHeld { get { return GetButton(input.Player.Fire); } }

    private void Awake()
    {
        input = new Controls();
        input.Enable();
        if (!singlePlayer)
        {
            devices = InputUser.CreateUserWithoutPairedDevices();
            devices.AssociateActionsWithUser(input);
        }
    }

    public void AssignDevice(InputDevice device)
    {
        devices = InputUser.PerformPairingWithDevice(device, devices);
        devices.AssociateActionsWithUser(input);
    }

    public void SwapSchemes()
    {

    }

    public bool GetButton(InputAction action)
    {
        return action.ReadValue<float>() > 0f;
    }

    public bool GetButtonDown(InputAction action)
    {
        return action.triggered && action.ReadValue<float>() > 0f;
    }

    public bool GetButtonUp(InputAction action)
    {
        return action.triggered && action.ReadValue<float>() == 0f;
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }*/

    public Vector2 WalkDir { get; set; }
    public Vector2 LookDir { get; set; }
    public bool JumpDown { get; set; }
    public bool WeaponSwitchDown { get; set; }
    public bool WeaponFireDown { get; set; }
    public bool WeaponFireHeld { get; set; }

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

    private void LateUpdate()
    {
        JumpDown = false;
        WeaponSwitchDown = false;
        WeaponFireDown = false;
    }
}
