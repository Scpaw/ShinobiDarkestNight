using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [Header("Projectile properties")]
    [SerializeField] float projectileDamage;
    [SerializeField] float maxDamage;
    [SerializeField] float aliveTime;
    [SerializeField] float moveSpeed;
    private Rigidbody2D prRb;
    private SpriteRenderer projectileSpriteRenderer;
    bool enemyHit;
    GameObject enemyTrigger;
    public GameObject afterProjectile;
    private bool deflected;
    [SerializeField] private Gradient deflectedGradient;
    private float deflectedTime;
    void Awake()
    {
        prRb = gameObject.GetComponent<Rigidbody2D>();

        projectileSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        projectileSpriteRenderer.flipX = false;

        enemyHit = false;

        if (gameObject.tag == "Projectile")
        {
            aliveTime = Time.time + aliveTime;
        }
        prRb.AddForce(transform.right* moveSpeed,ForceMode2D.Impulse);
    }

    void Update()
    {

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

        if (aliveTime <= Time.time)
        {
            Destroy(gameObject);
        }
        if (deflected)
        {
            transform.GetChild(0).GetComponent<TrailRenderer>().colorGradient = deflectedGradient;
        }
    }



    void OnTriggerEnter2D(Collider2D other)
    {
        if (deflectedTime < Time.time)
        {
            var Triggered = other.gameObject;


            if (Triggered.layer == 9)
            {
                Destroy(gameObject, 0);

            }
            else if (Triggered.tag == "Obstacle")
            {
                Destroy(gameObject, 0);

            }
            else if (Triggered.tag == "Enemy" && Triggered.GetComponent<EnemyHealth>() && !enemyHit)
            {
                enemyHit = true;
                enemyTrigger = Triggered;
                if (enemyTrigger.GetComponent<EnemyHealth>().canDeflect > 0)
                {
                    aliveTime = Time.time + 3;
                }
                else
                {
                    Destroy(gameObject, 0);
                }
                Attack();
            }
            else if (Triggered.tag == "Player" && Triggered.GetComponent<PlayerHealth>() && deflected)
            {
                Triggered.GetComponent<PlayerHealth>().AddDamage(projectileDamage);
                ParticleManager.instance.UseParticle("Blood", PlayerController.Instance.GetPlayer().transform.position, Vector3.zero);
                Destroy(gameObject, 0);
            }
        }
    }

    void Attack()
    {
        EnemyHealth theEnemyHealth = enemyTrigger.GetComponent<EnemyHealth>();

        if (enemyHit)
        {
            if (theEnemyHealth.canDeflect > 0)
            {
                theEnemyHealth.canDeflect--;
                theEnemyHealth.deflectAgain = 5;
                if (theEnemyHealth.deflectAtRandom)
                {
                    prRb.velocity = -(new Vector3(Random.Range(-1f,1f), Random.Range(-1f, 1f),0)).normalized * prRb.velocity.magnitude;
                }
                else
                {
                    prRb.velocity = -(transform.position - PlayerController.Instance.GetPlayer().transform.position).normalized * prRb.velocity.magnitude;
                }
                deflectedTime = Time.time + 0.05f;
                enemyHit = false;
                deflected = true;
                if (theEnemyHealth.GetComponent<Ronin_AI>())
                {
                    int i = Random.Range(1, 4);
                    theEnemyHealth.GetComponentInChildren<Animator>().SetInteger("Deflect", i);
                }
            }
            else
            {
                if (projectileDamage + 2.5f * theEnemyHealth.projectiles.Count >= maxDamage)
                {
                    theEnemyHealth.enemyAddDamage(maxDamage, false, true);
                }
                else
                {
                    theEnemyHealth.enemyAddDamage(projectileDamage + 2.5f * theEnemyHealth.projectiles.Count, false, true);
                }

            }
        }
    }
    private void OnDestroy()
    {
        if (!enemyHit || PlayerController.Instance.dango >0)
        {
            Instantiate(afterProjectile, transform.position, transform.rotation);
        }
        else
        {
            if (enemyTrigger.activeInHierarchy)
            {
                float scaleAdjustment = enemyTrigger.transform.localScale.x;
                if (scaleAdjustment > 0)
                {
                    scaleAdjustment = 1;
                }
                else
                {
                    scaleAdjustment = -1;
                }
                GameObject hit = Instantiate(afterProjectile, enemyTrigger.transform.InverseTransformDirection(transform.position), transform.rotation, enemyTrigger.transform);
                hit.transform.localPosition = new Vector3(hit.transform.localPosition.x * scaleAdjustment, hit.transform.localPosition.y, hit.transform.localPosition.z);
                hit.GetComponent<ProjectileToCollect>().onEnemy = true;
                hit.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
                enemyTrigger.GetComponent<EnemyHealth>().AddProjectile(hit);

            }
            else
            {
                Instantiate(afterProjectile, transform.position, transform.rotation);
            }
        }
    }

}
