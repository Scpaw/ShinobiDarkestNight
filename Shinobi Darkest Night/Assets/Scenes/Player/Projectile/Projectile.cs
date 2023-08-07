using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [Header("Projectile properties")]
    [SerializeField] float projectileDamage;
    [SerializeField] float aliveTime;
    [SerializeField] float moveSpeed;
    private Rigidbody2D prRb;
    private SpriteRenderer projectileSpriteRenderer;
    bool enemyHit;
    GameObject enemyTrigger;
    public GameObject afterProjectile;
    void Awake()
    {
        prRb = gameObject.GetComponent<Rigidbody2D>();

        projectileSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        projectileSpriteRenderer.flipX = false;

        enemyHit = false;

        if (gameObject.tag == "Projectile")
        {
            Destroy(gameObject, aliveTime);
        }
    }

    void Update()
    {
        ProjectileMovement();

        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        if (mousePosition.z > 0)
        {
            projectileSpriteRenderer.flipX = true;
        }
        else if(mousePosition.z < 0)
        {
            projectileSpriteRenderer.flipX = false;
        }
 
    }

    void ProjectileMovement()
    {
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
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
            theEnemyHealth.enemyAddDamage(projectileDamage, false,true);

        }
    }
    private void OnDestroy()
    {
        if (!enemyHit)
        {
            Instantiate(afterProjectile, transform.position, transform.rotation);
        }
        else
        {
            if (enemyTrigger.activeInHierarchy)
            {
                GameObject hit = Instantiate(afterProjectile, enemyTrigger.transform.InverseTransformDirection(transform.position), transform.rotation, enemyTrigger.transform);
                hit.GetComponent<ProjectileToCollect>().onEnemy = true;
                hit.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
                enemyTrigger.GetComponent<Enemy>().AddProjectile(hit);
            }
            else
            {
                Instantiate(afterProjectile, transform.position, transform.rotation);
            }
        }
    }
}
