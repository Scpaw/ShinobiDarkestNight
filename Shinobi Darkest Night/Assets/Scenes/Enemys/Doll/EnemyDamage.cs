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
    [SerializeField] float enemyDamage;
    [SerializeField] float enemyDamageRate;
    [SerializeField] float enemyNextDamage;
    [SerializeField] private GameObject thePlayer;
    public bool isAttacking;
    public int attacksInt;
    public bool attackAnim;

    Animator EnemyAnim;


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
    }

    void Update()
    {
        GameObject parentGameObject = gameObject;
        DamageRange damageR = parentGameObject.GetComponentInChildren<DamageRange>();
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
