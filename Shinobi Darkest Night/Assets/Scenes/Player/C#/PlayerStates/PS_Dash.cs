using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS_Dash : PlayerState
{
    public override bool canMove { get { return false; } }
    public override bool canAttack { get { return false; } }
    public override bool canExitAnim { get { return false; } }
    public override bool loops { get { return false; } }
    public override bool canTakeDmg { get { return true; } }
    public override bool ability { get { return false; } }

    private float dashDuration;
    private float dashCooldown;
    public override void Enter(PlayerStateMachine player)
    {
        directionalAnimationsIndex = this.GetType().FullName;
        player.rb.velocity = Vector2.zero;
        player.rb.velocity = player.movingDirection.normalized * player.dashSpeed;
        dashDuration = player.dashDuration;
        player.canDash = false;
        dashCooldown = player.dashCooldown;
    }
    public override void Update(PlayerStateMachine player)
    {
        if (dashDuration > 0)
        {
            dashDuration -= Time.deltaTime;
        }
        else
        {
            player.ChangeStates(player.lastState);
        }   
    }
    public override void FixedUpdate(PlayerStateMachine player)
    {

    }
    public override void Exit(PlayerStateMachine player) 
    {
        player.rb.velocity = Vector2.zero;
        player.ResetDash();
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
        Debug.Log(player.lastState);
        player.ChangeStates(player.lastState);
    }


}
