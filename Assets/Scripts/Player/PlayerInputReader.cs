using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputReader : MonoBehaviour
{
    private Vector2 walkDir;
    private float lookX;
    private float lookY;
    private bool jumpPressed;

    public Vector2 WalkDir => walkDir;
    public float LookX => lookX;
    public float LookY => lookY;
    public bool JumpButton { get { return jumpPressed; } set { jumpPressed = value; } }

    public void OnWalk(InputAction.CallbackContext ctx)
    {
        walkDir = ctx.ReadValue<Vector2>();
    }

    public void OnLookX(InputAction.CallbackContext ctx)
    {
        lookX = ctx.ReadValue<float>();
    }

    public void OnLookY(InputAction.CallbackContext ctx)
    {
        lookY = ctx.ReadValue<float>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started && !ctx.performed)
            jumpPressed = true;
    }

    private void Update()
    {
        print(jumpPressed);
    }
}
