using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [Header("Projectile properties")]
    [SerializeField] float projectileDamage;
    [SerializeField] float aliveTime;
    bool enemyHit;
    GameObject enemyTrigger;

    void Update()
    {
        if(enemyHit == true)
        {
            Attack();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == ("Enemy"))
        {
            
            var Triggered = other.gameObject;
            enemyHit = true;
            enemyTrigger = Triggered;
            Attack();
        }

        if (other.gameObject.tag == "Collider")
        {
            Destroy(gameObject, 0);
        }
        else if (other.gameObject.tag == "Enemy")
        {
            Destroy(gameObject, 0);
        }
    }

    void Attack()
    {
        Enemy theEnemyHealth = enemyTrigger.GetComponent<Enemy>();
        
        if (enemyHit == true)
        {
            theEnemyHealth.enemyAddDamage(projectileDamage);
        }
    }

    void Awake()
    {
        enemyHit = false;

        if (gameObject.tag == "Projectile")
        {
            Destroy(gameObject, aliveTime);
        }
    }
}
