using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class WalkState : PlayerState
{
    private Vector2 inputDirection;
    private float footstepsTimer = 0f;

    public WalkState(PlayerMovementController player, CharacterController cc, Transform cam, PlayerInputReader controls) : base(player, cc, cam, controls)
    {
    }

    public override void OnStateEnter()
    {
    }

    public override void OnStateExit()
    {
    }

    public override void Update()
    {
        Vector3 velocity = player.transform.forward * controls.WalkDir.y + player.transform.right * controls.WalkDir.x;
        velocity *= player.WalkSpeed;
        velocity.y = player.MoveDirection.y;
        if (controls.WalkDir == Vector2.zero)
        {
            player.MoveDirection = Vector3.Lerp(player.MoveDirection, velocity, player.Decceleration * Time.deltaTime);
        }
        else
        {
            player.MoveDirection = Vector3.Lerp(player.MoveDirection, velocity, player.Acceleration * Time.deltaTime);
        }

        if (player.IsGrounded() && player.SlopeAngle != 0f && player.SlopeAngle > cc.slopeLimit)
        {
            player.SwitchState(new SlideState(player, cc, cam, controls));
        }
        else if (controls.JumpDown && player.IsGrounded())
        {
            player.SetY(player.Jump);
            player.PlayClip(player.LandingLight);
        }
        else if (player.IsGrounded() == false)
        {
            player.SwitchState(new AirState(player, cc, cam, controls));
        }
        else
        {
            player.SetY(-4f);
        }

        if (footstepsTimer >= Mathf.PI / 10f && controls.WalkDir.magnitude > 0f)
        {
            player.PlayClip(player.Footsteps);
            footstepsTimer = 0f;
        }
        else
        {
            footstepsTimer += Time.deltaTime;
        }
    }

    public void OnJump()
    {
        Debug.Log("Player Jumped!");
    }
}
