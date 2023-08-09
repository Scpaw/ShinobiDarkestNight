using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
//using static TreeEditor.TreeEditorHelper;

public class Doll : MonoBehaviour
{
    [Header("Enemy Damage")]
    [SerializeField] float enemyDamage;
    [SerializeField] float enemyDamageRate;
    [SerializeField] float enemyNextDamage;
    [SerializeField] private GameObject thePlayer;

    Animator EnemyAnim;
    private Vector3 startPos;

    private void Awake()
    {
        thePlayer = GameObject.Find("Shinobi");
        EnemyAnim = transform.Find("Grafika").GetComponent<Animator>();
        startPos = transform.position;
    }

    private void OnEnable()
    {
        thePlayer = GameObject.Find("Shinobi");
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
        }
    }

    void Attack()
    {
        PlayerHealth thePlayerHealth = thePlayer.GetComponent<PlayerHealth>();

        if (enemyNextDamage <= Time.time)
        {
            thePlayerHealth.AddDamage(enemyDamage);
            enemyNextDamage = Time.time + enemyDamageRate;
        }
    }

}