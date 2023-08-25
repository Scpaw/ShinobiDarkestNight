using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Okubi_AnimToCode : MonoBehaviour
{
    private float attackTime;
    private bool playerIn;
    private PlayerHealth playerHealth;
    private float dmg;
    private bool attacking;
    private EnemyHealth enemyHealth;
    private float playerInAttackTime;
    private void OnEnable()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        dmg = 10;
        enemyHealth = transform.parent.GetComponent<EnemyHealth>();
    }
    public void HairAttack()
    {
        attackTime = 2;
        GetComponent<BoxCollider2D>().enabled = true;
        attacking = true;
        enemyHealth.ProjectilesOff(10);
    }
    private void Update()
    {
        if (attacking)
        {
            if (attackTime <= 0 && !playerIn)
            {
                StopAttack();
            }
            if (attackTime > 0 &&!playerIn)
            {
                attackTime -= Time.deltaTime;
            }
            if (playerIn)
            {
                if (playerInAttackTime < 1 && playerInAttackTime > 0)
                {
                    dmg = 20;
                }
                else if (playerInAttackTime <= 0)
                {
                    StopAttack();
                }


                playerInAttackTime -= Time.deltaTime;
                
                if (attackTime <= 0)
                {
                    StopAttack();
                }
                else
                {
                    playerHealth.AddDamage(dmg * Time.deltaTime);
                }
            }

            if (enemyHealth.canDeflect <= 0)
            {
                enemyHealth.canDeflect = 1;
            }
        }
        else
        {
            enemyHealth.canDeflect = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerInAttackTime = 2;
            playerIn = true;
            playerHealth = collision.GetComponent<PlayerHealth>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerIn = false;
        }
    }


    private void StopAttack()
    {
        playerIn = false;
        attackTime = 0;
        GetComponent<Animator>().SetTrigger("Back");
        transform.parent.GetComponent<EnemyDamage>().canAttack = true;
        transform.parent.GetComponent<EnemyDamage>().attackAnim = true;
        GetComponent<BoxCollider2D>().enabled = false;
        attacking = false;
        playerInAttackTime = 2;
        dmg = 10;
    }
}
