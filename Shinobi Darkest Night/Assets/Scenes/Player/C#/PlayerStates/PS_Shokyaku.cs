using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS_Shokyaku : PlayerState
{
    public override bool canMove { get { return true; } }
    public override bool canAttack { get { return false; } }
    public override bool canExitAnim { get { return true; } }
    public override bool loops { get { return true; } }
    public override bool canTakeDmg { get { return false; } }
    public override bool ability { get { return true; } }

    private bool LMBPress;

    public override void Enter(PlayerStateMachine player)
    {
        directionalAnimationsIndex = "PS_Idle";
    }

    public override void Update(PlayerStateMachine player)
    {
        if (LMBPress)
        {
            if (directionalAnimationsIndex != "PS_Shokyaku")
            {
                directionalAnimationsIndex = "PS_Shokyaku";
            }

            player.facingDirection = player.projectileSpawnPoint.position - player.transform.position;

            ParticleManager.instance.UseParticle("Fire", player.projectileSpawnPoint.position, player.projectileSpawnPoint.rotation.eulerAngles);
            Collider2D[] hits = Physics2D.OverlapAreaAll(new Vector2(player.projectileSpawnPoint.GetChild(0).position.x, player.projectileSpawnPoint.GetChild(0).position.y), new Vector2(player.projectileSpawnPoint.GetChild(1).position.x, player.projectileSpawnPoint.GetChild(1).position.y), player.enemyLayer);
            foreach (Collider2D enemy in hits)
            {
                if (enemy.GetComponent<EnemyHealth>())
                {
                    enemy.GetComponent<EnemyHealth>().enemyAddDamage(40 * Time.deltaTime, false, false);
                    if (enemy.GetComponent<NewAi>())
                    {
                        enemy.GetComponent<NewAi>().Stun(Time.deltaTime, enemy.transform.position - player.transform.position);
                    }
                    else
                    {

                        if (!enemy.GetComponent<EnemyHealth>().isStuned)
                        {
                            enemy.GetComponent<EnemyHealth>().Stun();
                        }
                    }
                }
            }
        }
    }
    public override void FixedUpdate(PlayerStateMachine player)
    {
        if (!LMBPress)
        {
            if (player.movingDirection.magnitude <= 0.01f)
            {
                directionalAnimationsIndex = "PS_Idle";
            }
            else
            {
                directionalAnimationsIndex = "PS_Run";
            }


            Vector2 moveForce = player.movingDirection * player.moveSpeed * Time.fixedDeltaTime;
            player.rb.AddForce(moveForce);


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
    }
    public override void Exit(PlayerStateMachine player)
    {

    }
    public override void LMB(PlayerStateMachine player, float value)
    {
        LMBPress = value > 0;
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
                player.rb.MovePosition(player.rb.position + direction * player.moveSpeed * Time.fixedDeltaTime);

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
