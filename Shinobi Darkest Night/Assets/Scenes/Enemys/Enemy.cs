using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Health")]
    [SerializeField] float enemyMaxHealth;
    [SerializeField] float enemyHealth;
    [SerializeField] Slider enemyHealthSlider;
    [SerializeField] GameObject enemyCanvas;

    [Header("Enemy Damage")]
    [SerializeField] float enemyDamage;
    [SerializeField] float enemyDamageRate;
    [SerializeField] float enemyNextDamage;
    [SerializeField] GameObject thePlayer;

    Animator EnemyAnim;
    private bool isdamaged = false;

    public void Awake()
    {
        enemyHealth = enemyMaxHealth;
        enemyHealthSlider.maxValue = enemyMaxHealth;
        enemyHealthSlider.value = enemyHealth;
    }

    public void enemyAddDamage(float Damage)
    {
        enemyHealth -= Damage;
        enemyHealthSlider.value = enemyHealth;
        isdamaged = true;
        Debug.Log(isdamaged);
        if (enemyHealth <= 0)
        {
            MakeDead();
        }
    }

    void Update()
    {
        EnemyAnim = GetComponent<Animator>();
        GameObject parentGameObject = gameObject;
        DamageRange damageR = parentGameObject.GetComponentInChildren<DamageRange>();

        if(isdamaged == true)
        {
            EnemyAnim.SetBool("isDamaged", true);
        }
        else
        {
            EnemyAnim.SetBool("isDamaged", false);
        }

        if (damageR.playerInRange == true) 
        {
            Attack();
            Debug.Log("Attack");
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

    private void MakeDead()
    {
        Destroy(gameObject, 0);
    }

}
