using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS_Desumiru : PlayerState
{
    public override bool canMove { get { return true; } }
    public override bool canAttack { get { return false; } }
    public override bool canExitAnim { get { return true; } }
    public override bool loops { get { return true; } }
    public override bool canTakeDmg { get { return true; } }
    public override bool ability { get { return true; } }

    public override void Enter(PlayerStateMachine player)
    {
        directionalAnimationsIndex = this.GetType().FullName;
    }

    public override void Update(PlayerStateMachine player)
    {

    }
    public override void FixedUpdate(PlayerStateMachine player)
    {
        //Vector2 moveForce = player.movingDirection * (player.moveSpeed* 0.1f) * Time.fixedDeltaTime;
        //Debug.Log(moveForce);
        //player.rb.AddForce(moveForce);


        //Prevent the player from blocking on collisions when moving
        if (player.movingDirection != Vector2.zero)
        {
            bool success = TryMove(player.movingDirection, player);

            if (!success)
            {
                success = TryMove(new Vector2(player.movingDirection.x, 0), player);

                if (!success)
                {
                    success = TryMove(new Vector2(0, player.movingDirection.y), player);
                }
            }
        }
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
        player.ChangeStates(player.ps_idle);
    }

    bool TryMove(Vector2 direction, PlayerStateMachine player)
    {
        if (direction != Vector2.zero)
        {
            int count = player.rb.Cast(
                direction,
                player.movementFilter,
                player.castCollisions,
                player.moveSpeed * Time.fixedDeltaTime + player.collisionOffset
                );

            if (count == 0)
            {
                player.rb.MovePosition(player.rb.position + direction * player.moveSpeed/4 * Time.fixedDeltaTime);

                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

}

