using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS_Heal : PlayerState
{
    public override bool canMove { get { return false; } }
    public override bool canAttack { get { return false; } }
    public override bool canExitAnim { get { return true; } }
    public override bool loops { get { return true; } }
    public override bool canTakeDmg { get { return false; } }
    public override bool ability { get { return true; } }

    private float timeToHeal;
    public override void Enter(PlayerStateMachine player)
    {
        directionalAnimationsIndex = this.GetType().FullName;
    }

    public override void Update(PlayerStateMachine player)
    {
        if (timeToHeal > 0)
        { 
            timeToHeal -= Time.deltaTime;
        }
    }
    public override void FixedUpdate(PlayerStateMachine player)
    {

    }
    public override void Exit(PlayerStateMachine player)
    {

    }
    public override void LMB(PlayerStateMachine player, float value)
    {
        if (value > 0 && timeToHeal <=0) 
        {
            player.ChangeAnimation(player.facingDirection);
            Heal(player);
        }
    }
    public override void RMB(PlayerStateMachine player, float value)
    {

    }

    public override void Move(PlayerStateMachine player, Vector2 direction)
    {

    }

    public override void ChangeStateAfterAnim(PlayerStateMachine player)
    {
        player.ChangeToIdle();
    }

    private void Heal(PlayerStateMachine player)
    {
        player.hp.AddHealth(10);

        timeToHeal = 0.3f;
    }

}
