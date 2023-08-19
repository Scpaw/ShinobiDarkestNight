using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
//using static TreeEditor.TreeEditorHelper;

public class ScareCrow : MonoBehaviour
{
    [Header("Enemy Damage")]
    [SerializeField] float enemyDamage;
    [SerializeField] float enemyDamageRate;
    [SerializeField] float enemyNextDamage;
    [SerializeField] private GameObject thePlayer;
    [SerializeField] private bool randomSimulated;
    Animator EnemyAnim;



    private void OnEnable()
    {
        if (randomSimulated)
        {
            if (Random.value > 0.5f)
            {
                GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            }
            else
            {
                GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            }
        }

        if (thePlayer == null)
        {
            thePlayer = PlayerController.Instance.GetPlayer();
        }
        if (EnemyAnim == null)
        {
            EnemyAnim = transform.Find("Grafika").GetComponent<Animator>();
        }
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
