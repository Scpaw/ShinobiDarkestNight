using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health")]
    public float enemyMaxHealth;
    public float enemyHealth;
    [SerializeField] Slider enemyHealthSlider;
    [SerializeField] GameObject enemyCanvas;
    public bool canBeAttacked;
    public bool canDoDmg;
    private float canBeAttackedTimer;
    public List<GameObject> projectiles;
    public float stundTime;
    public bool isStuned;
    public int canDeflect;
    public float deflectAgain;
    public bool deflectAtRandom;
    public Vector3 startPos = Vector3.zero;
    [SerializeField] bool useParticles;

    public void Awake()
    {
        enemyHealth = enemyMaxHealth;
        canDoDmg = true;
    }
    private void OnEnable()
    {
        deflectAgain = 5;
        enemyHealth = enemyMaxHealth;
        if (enemyHealthSlider != null)
        {
            enemyHealthSlider.maxValue = enemyMaxHealth;
            enemyHealthSlider.value = enemyHealth;
        }
        canBeAttacked = true;
        if (transform.parent != null)
        {
            transform.parent.GetComponent<RoomBrain>().enemiesActive++;
        }


        if (startPos == Vector3.zero)
        {
            startPos = transform.localPosition;
        }
        else
        { 
            transform.localPosition = startPos;
        }
    }

    public void enemyAddDamage(float Damage, bool dropProjectiles, bool useparticle)
    {
        if (canDoDmg)
        {
            if (useparticle && useParticles)
            {
                ParticleManager.instance.UseParticle("Blood", transform.position, transform.rotation.eulerAngles);
            }
            enemyHealth -= Damage;
            if (enemyHealthSlider != null)
            {
                enemyHealthSlider.value = enemyHealth;
            }
            if (dropProjectiles)
            {
                ProjectilesOff(0);
            }
            if (enemyHealth <= 0)
            {
                MakeDead();
            }
            deflectAgain = 5;
        }
    }

    public void AddProjectile(GameObject projectile)
    {
        projectiles.Add(projectile);
        if (!canDoDmg)
        {
            ProjectilesOff(1);
        }
    }

    private void MakeDead()
    {
        StopAllCoroutines();
        ProjectilesOff(0);
        gameObject.SetActive(false);
    }

    public void ProjectilesOff(float projectileAddForce)
    {
        foreach (GameObject projectile in projectiles)
        {
            projectile.transform.parent = null;
            projectile.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            projectile.GetComponent<Rigidbody2D>().AddForce((projectile.transform.position - transform.position) * (4 + projectileAddForce), ForceMode2D.Impulse);
        }
        if (projectiles.Count > 0)
        {
            projectiles.Clear();
        }
    }

    public IEnumerator Stuned(bool meeleAttack)
    {
        if ((canBeAttacked || !meeleAttack) && canDoDmg)
        {
            isStuned = true;
            stundTime = 0.75f;
            while (stundTime > 0)
            {
                stundTime -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            isStuned = false;
            if (meeleAttack)
            {
                canBeAttacked = false;
                canBeAttackedTimer = 10f;
                while (canBeAttackedTimer >0)
                { 
                    canBeAttackedTimer -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                canBeAttacked = true;
            }
        }
    }

    private void OnDisable()
    {
        if (transform.parent != null && enemyHealth <= 0)
        {
            transform.parent.GetComponent<RoomBrain>().SpawnEnemies();
        }
        if (!canBeAttacked)
        {
            StopAllCoroutines();
        }
        if (transform.parent != null)
        {
            transform.parent.GetComponent<RoomBrain>().enemiesActive--;
        }
        if (GetComponent<activateEnemy>())
        { 
            GetComponent<activateEnemy>().Activate();
        }
    }
}
