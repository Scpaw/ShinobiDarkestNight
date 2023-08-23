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
    }

    void Update()
    {
        playerIn = damageR.playerInRange;
        if (damageR.playerInRange == true)
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
        if (attackAnim) 
        {
            EnemyAnim.SetTrigger("Attack");
        }

        PlayerHealth thePlayerHealth = thePlayer.GetComponent<PlayerHealth>();
        if (enemyNextDamage <= Time.time)
        {
            thePlayerHealth.AddDamage(enemyDamage);
            enemyNextDamage = Time.time + enemyDamageRate;
            if (attackAnim)
            {
                attacksInt++;
            }
        }
    }

}
