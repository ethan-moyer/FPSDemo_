using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideState : PlayerState
{
    private Vector2 inputDirection;
    public SlideState(PlayerMovementController player, CharacterController cc, Transform cam, PlayerInputReader controls) : base(player, cc, cam, controls)
    {
    }

    public override void OnStateEnter()
    {
        player.MoveDirection = player.SlopeTangent * 5f;
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
    }

    public override void Update()
    {
        inputDirection = controls.WalkDir;
        Vector3 velocity = player.transform.forward * inputDirection.y + player.transform.right * inputDirection.x;
        player.MoveDirection = Vector3.Lerp(player.MoveDirection, velocity, player.Acceleration * 0.2f * Time.deltaTime);
        player.MoveDirection += player.SlopeTangent * 5 * Time.deltaTime;

        if (player.IsGrounded() == false)
        {
            player.SwitchState(new AirState(player, cc, cam, controls));
        }
        else if (player.SlopeAngle == 0f || player.SlopeAngle <= cc.slopeLimit)
        {
            player.SwitchState(new WalkState(player, cc, cam, controls));
        }
    }
}
