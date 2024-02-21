using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
    public abstract bool canMove { get; }
    public abstract bool canAttack { get; }
    public abstract bool canExitAnim { get; }
    public abstract bool canTakeDmg { get; }
    public abstract bool loops { get; }
    public abstract bool ability { get; }
    public  string directionalAnimationsIndex { get; set; }

    public abstract void Enter(PlayerStateMachine player);

    public abstract void Update(PlayerStateMachine player);
    public abstract void FixedUpdate(PlayerStateMachine player);

    public abstract void Exit(PlayerStateMachine player);

    public abstract void Move(PlayerStateMachine player, Vector2 direction);

    public abstract void LMB(PlayerStateMachine player, float value);
    public abstract void RMB(PlayerStateMachine player, float value);
    public abstract void ChangeStateAfterAnim(PlayerStateMachine player);
}
