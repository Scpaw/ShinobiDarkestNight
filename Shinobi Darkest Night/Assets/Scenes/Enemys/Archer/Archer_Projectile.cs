using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_Projectile : MonoBehaviour
{

    [Header("Projectile properties")]
    [SerializeField] float projectileDamage;
    [SerializeField] float aliveTime;
    [SerializeField] float moveSpeed;

    private Rigidbody2D rb;
    private SpriteRenderer projectileSpriteRenderer;
    bool playerHit;
    GameObject playerTrigger;
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        playerTrigger = GameObject.Find("Shinobi");
        projectileSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        projectileSpriteRenderer.flipX = false;

        playerHit = false;

        if (gameObject.tag == "Projectile")
        {
            Destroy(gameObject, aliveTime);
        }
    }

    void Update()
    {
        ProjectileMovement();
    }

    void ProjectileMovement()
    {
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == ("Player"))
        {

            var Triggered = other.gameObject;
            playerHit = true;
            playerTrigger = Triggered;
            Attack();
        }

        if (other.gameObject.tag == "Collider")
        {
            Destroy(gameObject, 0);
        }
        else if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject, 0);
        }
    }

    void Attack()
    {
        PlayerHealth theEnemyHealth = playerTrigger.GetComponent<PlayerHealth>();

        if (playerHit == true)
        {
            theEnemyHealth.AddDamage(projectileDamage);

        }
    }
}
