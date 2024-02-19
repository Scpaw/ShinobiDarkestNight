using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS_Katana : PlayerState
{
    public override bool canMove { get { return false; } }
    public override bool canAttack { get { return true; } }
    public override bool canExitAnim { get { return false; } }
    public override bool loops { get { return false; } }
    public override bool canTakeDmg { get { return true; } }

    private MeeleAttack attackInfo;
    private float time;
    private bool attacked;
    public override void Enter(PlayerStateMachine player)
    {
        player.SetLastAttack(true);
        attackInfo = player.katanaAttacks[player.attackIndex];
        player.facingDirection = player.projectileSpawnPoint.position - player.transform.position;
        time = attackInfo.movingDuration * attackInfo.animation.GetFacingClip(player.facingDirection).length;
        attacked = false;
        player.rb.velocity = Vector3.zero;
    }
    public override void Update(PlayerStateMachine player)
    {
        if (!attacked)
        {
            if (time > 0 && !Physics2D.OverlapCircle(player.projectileSpawnPoint.position, 1.6f, LayerMask.GetMask("Collider")))
            {
                if (player.rb.velocity.magnitude < player.katanaSpeed)
                {
                    player.rb.AddForce(player.facingDirection * player.katanaSpeed);
                }
                time -= Time.deltaTime;
            }
            else
            {
                Collider2D[] hit = Physics2D.OverlapCircleAll(player.projectileSpawnPoint.position, attackInfo.range, LayerMask.GetMask("Enemy"));
                if (hit != null && hit.Length != 0)
                {
                    if (player.rb.velocity.magnitude > 0.1f && time > 0)
                    {
                        CameraShake.instance.Shake(EShakeStrenght.extraStrong, EShakeShape.recoil, player.transform.position - player.projectileSpawnPoint.position);
                    }
                    else
                    {
                        CameraShake.instance.Shake(EShakeStrenght.strong, EShakeShape.recoil, player.transform.position - player.projectileSpawnPoint.position);
                    }
                    foreach (Collider2D enemy in hit)
                    {
                        if (enemy.gameObject.layer == 6 && enemy.gameObject != null)
                        {
                            if (enemy.gameObject.GetComponent<EnemyHealth>() && enemy.gameObject.GetComponent<NewAi>())
                            {
                                enemy.gameObject.GetComponent<EnemyHealth>().ProjectilesOff(0, attackInfo.shurikenDrop, 4);
                                enemy.gameObject.GetComponent<EnemyHealth>().enemyAddDamage(attackInfo.dmg, false, true);
                                if (enemy.gameObject.GetComponent<NewAi>())
                                {
                                    enemy.gameObject.GetComponent<NewAi>().Stun(0.4f, player.projectileSpawnPoint.right * 5.3f);
                                }
                                else
                                {
                                    enemy.gameObject.GetComponent<Rigidbody2D>().AddForce(player.projectileSpawnPoint.right * player.pushForce, ForceMode2D.Impulse);
                                }
                            }
                            else if (enemy.gameObject.GetComponent<EnemyHealth>())
                            {
                                enemy.gameObject.GetComponent<EnemyHealth>().enemyAddDamage(attackInfo.dmg, false, true);
                            }
                        }
                    }
                }
                player.rb.velocity = Vector3.zero;
                attacked = true;
            }
        }
    }
    public override void FixedUpdate(PlayerStateMachine player)
    {

    }
    public override void Exit(PlayerStateMachine player)
    {
        player.rb.velocity = Vector3.zero;
        player.SetLastAttack(false);
    }
    public override void LMB(PlayerStateMachine player, float value)
    {

        if (value > 0 && player.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.66)
        {
            if (player.attackIndex + 1 >= player.katanaAttacks.Count)
            {
                player.attackIndex = 0;
            }
            else
            {
                player.attackIndex += 1;
            }
            player.facingDirection = player.projectileSpawnPoint.position - player.transform.position;
            player.ChangeStates(player.ps_katana);
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
        player.ChangeStates(player.ps_idle);
    }
}
