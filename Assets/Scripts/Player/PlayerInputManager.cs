using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    private Controls input;

    public Vector2 WalkDir { get { return input.Player.Walk.ReadValue<Vector2>(); } }
    public Vector2 LookDir { get { return new Vector2(input.Player.LookX.ReadValue<float>(), input.Player.LookY.ReadValue<float>()); } }
    public bool JumpDown { get { return GetButtonDown(input.Player.Jump); } }
    public bool WeaponSwitchDown { get { return GetButtonDown(input.Player.WeaponSwitch); } }
    public bool WeaponFireDown { get { return GetButtonDown(input.Player.Fire); } }
    public bool WeaponFireHeld { get { return GetButton(input.Player.Fire); } }

    private void Awake()
    {
        input = new Controls();
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
    }
}
