using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected PlayerMovementController player;
    protected CharacterController cc;
    protected Transform cam;
    protected PlayerInputReader controls;

    public PlayerState(PlayerMovementController player, CharacterController cc, Transform cam, PlayerInputReader controls)
    {
        this.player = player;
        this.cc = cc;
        this.cam = cam;
        this.controls = controls;
    }

    public virtual void Update()
    {
    
    }

    public virtual void OnStateEnter()
    {

    }

    public virtual void OnStateExit()
    {

    }
}
