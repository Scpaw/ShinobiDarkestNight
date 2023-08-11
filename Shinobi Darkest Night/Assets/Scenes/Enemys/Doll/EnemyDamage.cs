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
    private Vector3 startPos;

    private void Start()
    {
        EnemyAnim = transform.Find("Grafika").GetComponent<Animator>();
        startPos = transform.position;
        thePlayer = PlayerController.Instance.GetPlayer();
    }

    private void OnEnable()
    {
        attacksInt = 0;  
        EnemyAnim = transform.Find("Grafika").GetComponent<Animator>();
        startPos = transform.position;
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
