using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class WalkState : PlayerState
{
    private Vector2 inputDirection;

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
        inputDirection = controls.WalkDir;
        Vector3 velocity = player.transform.forward * inputDirection.y + player.transform.right * inputDirection.x;
        velocity *= player.WalkSpeed;
        velocity.y = player.MoveDirection.y;
        player.MoveDirection = Vector3.Lerp(player.MoveDirection, velocity, player.Acceleration * Time.deltaTime);

        if (player.IsGrounded() && player.SlopeAngle != 0f && player.SlopeAngle > cc.slopeLimit)
        {
            player.SwitchState(new SlideState(player, cc, cam, controls));
        }
        else if (controls.JumpButton && player.IsGrounded())
        {
            player.SetY(player.Jump);
        }
        else if (player.IsGrounded() == false)
        {
            player.SwitchState(new AirState(player, cc, cam, controls));
        }
        else
        {
            player.SetY(-4f);
        }
    }

    public void OnJump()
    {
        Debug.Log("Player Jumped!");
    }
}
