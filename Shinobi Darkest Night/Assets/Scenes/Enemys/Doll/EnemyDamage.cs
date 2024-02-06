using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
//using static TreeEditor.TreeEditorHelper;

public class EnemyDamage : MonoBehaviour
{
    [Header("Enemy Damage")]
    public float enemyDamage;
    [SerializeField] float enemyDamageRate;
    [SerializeField] float enemyNextDamage;
    private GameObject thePlayer;
    public bool isAttacking;
    public int attacksInt;
    public bool attackAnim;
    private DamageRange damageR;
    Animator EnemyAnim;
    public bool playerIn;
    public bool canAttack;
    [SerializeField] bool doDmgWhileAttack = true;
    [SerializeField] bool EnemyAI = false;



    private void OnEnable()
    {
        attacksInt = 0;
        if (EnemyAnim == null)
        {
            EnemyAnim = transform.Find("Grafika").GetComponent<Animator>();
        }
        if (thePlayer == null)
        {
            thePlayer = PlayerController.Instance.GetPlayer();
        }
        if (damageR == null)
        {
            damageR = GetComponentInChildren<DamageRange>();
        }
        canAttack = true;

        if (GetComponent<NewAi>())
        {
            GetComponent<NewAi>().attackRate = enemyDamageRate;
        }
    }

    void Update()
    {
        playerIn = damageR.playerInRange;
        if (damageR.playerInRange && canAttack && GetComponent<EnemyHealth>().canAttackAgain <=0)
        {
            Attack();
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }
    }

    void Attack()
    {
        PlayerHealth thePlayerHealth = thePlayer.GetComponent<PlayerHealth>();
        if (enemyNextDamage <= Time.time)
        {
            if (EnemyAI)
            { 
                GetComponent<NewAi>().EndAttack();
            }

            if (doDmgWhileAttack)
            {
                thePlayerHealth.AddDamage(enemyDamage);
            }
            enemyNextDamage = Time.time + enemyDamageRate;

            if (attackAnim)
            {
                attacksInt++;
                if (attackAnim)
                {
                    EnemyAnim.SetTrigger("Attack");
                }
            }
        }
    }

    public void Dmg()
    {
        PlayerHealth thePlayerHealth = thePlayer.GetComponent<PlayerHealth>();
        thePlayerHealth.AddDamage(enemyDamage);
    }

}
