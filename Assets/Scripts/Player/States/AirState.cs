using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AirState : PlayerState
{
    private Vector2 inputDirection;

    public AirState(PlayerMovementController player, CharacterController cc, Transform cam, PlayerInputReader controls) : base(player, cc, cam, controls)
    {
    }

    public override void OnStateEnter()
    {
    }

    public override void OnStateExit()
    {
        if (player.MoveDirection.y <= -7f)
            player.PlayClip(player.LandingHeavy);
        //else
            //player.PlayClip(player.LandingLight);
        player.SetY(0f);
    }

    public override void Update()
    {
        inputDirection = controls.WalkDir;
        Vector3 velocity = player.transform.forward * inputDirection.y + player.transform.right * inputDirection.x;
        velocity *= player.WalkSpeed;
        velocity.y = player.MoveDirection.y;

        player.MoveDirection = Vector3.Lerp(player.MoveDirection, velocity, player.Acceleration * 0.1f * Time.deltaTime);
        player.MoveDirection += Vector3.up * player.Gravity * Time.deltaTime;

        if (player.IsGrounded() == true && velocity.y <= 0f)
        {
            player.SwitchState(new WalkState(player, cc, cam, controls));
        }
    }
}
