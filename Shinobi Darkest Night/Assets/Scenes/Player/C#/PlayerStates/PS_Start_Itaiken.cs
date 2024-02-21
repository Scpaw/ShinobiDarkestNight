using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS_Start_Itaiken : PlayerState
{
    public override bool canMove { get { return false; } }
    public override bool canAttack { get { return false; } }
    public override bool canExitAnim { get { return false; } }
    public override bool loops { get { return false; } }
    public override bool canTakeDmg { get { return false; } }

    public override void Enter(PlayerStateMachine player)
    {
        directionalAnimationsIndex = this.GetType().FullName;
        player.abilityOn = true;
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

    public override void LMB(PlayerStateMachine player, float value)
    {

    }
    public override void RMB(PlayerStateMachine player, float value)
    {

    }
    public override void Move(PlayerStateMachine player, Vector2 direction)
    {

    }

    public override void ChangeStateAfterAnim(PlayerStateMachine player)
    {
        player.ChangeStates(new PS_Itaiken());
    }

}