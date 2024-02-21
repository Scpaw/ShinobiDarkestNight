using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS_Fan : PlayerState
{
    public override bool canMove { get { return false; } }
    public override bool canAttack { get { return false; } }
    public override bool canExitAnim { get { return false; } }
    public override bool loops { get { return false; } }
    public override bool canTakeDmg { get { return true; } }
    public override bool ability { get { return false; } }

    public override void Enter(PlayerStateMachine player)
    {
        directionalAnimationsIndex = this.GetType().FullName;
        player.SetLastAttack(true);

        Collider2D[] hit = Physics2D.OverlapCircleAll(player.projectileSpawnPoint.position, player.projectileSpawnPoint.GetComponent<CircleCollider2D>().radius * player.projectileSpawnPoint.parent.localScale.x, LayerMask.GetMask("Enemy"));
        if (hit == null || hit.Length == 0)
        {
            return;
        }
        else
        {
            CameraShake.instance.Shake(EShakeStrenght.extraStrong, EShakeShape.recoil, player.transform.position - player.projectileSpawnPoint.position);
        }
        
        
        foreach (Collider2D enemy in hit)
        {
            if (enemy.gameObject.layer == 6 && enemy.gameObject != null)
            {
                if (enemy.gameObject.GetComponent<EnemyHealth>())
                {
                    enemy.gameObject.GetComponent<EnemyHealth>().enemyAddDamage(player.fanDmg, false, true);
                    enemy.gameObject.GetComponent<EnemyHealth>().ProjectilesOff(1, enemy.gameObject.GetComponent<EnemyHealth>().projectiles.Count, player.shurikenDropDmg);
                }
                if (enemy.gameObject.GetComponent<Rigidbody2D>() != null && enemy.GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Static && enemy.gameObject.GetComponent<EnemyHealth>().canBeAttacked && enemy.gameObject.GetComponent<EnemyHealth>().canDoDmg)
                {
                    enemy.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    if (enemy.gameObject.GetComponent<NewAi>())
                    {
                        enemy.gameObject.GetComponent<NewAi>().Stun(0.9f, player.projectileSpawnPoint.right * player.pushForce);
                    }
                    else
                    {
                        enemy.gameObject.GetComponent<Rigidbody2D>().AddForce(player.projectileSpawnPoint.right * player.pushForce, ForceMode2D.Impulse);
                    }
                    enemy.gameObject.GetComponent<EnemyHealth>().Stun();
                }
            }
        }
    }


    public override void Update(PlayerStateMachine player)
    {

    }
    public override void FixedUpdate(PlayerStateMachine player)
    {

    }
    public override void Exit(PlayerStateMachine player)
    {
        player.SetLastAttack(false);
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
}
