using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PS_Idle : PlayerState
{
    public override bool canMove { get { return true; } }
    public override bool canAttack{ get { return true; } }
    public override bool canExitAnim { get { return true; } }
    public override bool loops { get { return true; } }
    public override bool canTakeDmg { get { return true; } }


    public override void Enter(PlayerStateMachine player)
    {
        directionalAnimationsIndex = this.GetType().FullName;
    }

    public override void Update(PlayerStateMachine player)
    {

    }
    public override void FixedUpdate(PlayerStateMachine player)
    {

    }
    public override void Exit(PlayerStateMachine player)
    {

    }

    public override void Move(PlayerStateMachine player, Vector2 direction)
    {
        if(direction.magnitude > 0)
        {
            player.ChangeStates(player.ps_run);
        }
    }
    public override void LMB(PlayerStateMachine player, float value)
    {

    }
    public override void RMB(PlayerStateMachine player, float value)
    {

    }

    public override void ChangeStateAfterAnim(PlayerStateMachine player)
    {

    }
}
