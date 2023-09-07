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
    public Vector3 startPos;
    public List<GameObject> projectiles;
    public float stundTime;
    public bool isStuned;
    public int canDeflect;
    public float deflectAgain;
    public bool deflectAtRandom;

    public void Awake()
    {
        enemyHealth = enemyMaxHealth;
        enemyHealthSlider.maxValue = enemyMaxHealth;
        enemyHealthSlider.value = enemyHealth;
        canDoDmg = true;
    }
    private void OnEnable()
    {
        deflectAgain = 5;
        enemyHealth = enemyMaxHealth;
        enemyHealthSlider.maxValue = enemyMaxHealth;
        enemyHealthSlider.value = enemyHealth;
        canBeAttacked = true;
        if (startPos == Vector3.zero)
        {
            startPos = transform.position;
        }
        else
        {
            transform.position = startPos;
        }
        transform.parent.GetComponent<RoomBrain>().enemiesActive++;
    }

    public void enemyAddDamage(float Damage, bool dropProjectiles, bool useparticle)
    {
        if (canDoDmg)
        {
            if (useparticle)
            {
                ParticleManager.instance.UseParticle("Blood", transform.position, transform.rotation.eulerAngles);
            }
            enemyHealth -= Damage;
            enemyHealthSlider.value = enemyHealth;
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
        transform.parent.GetComponent<RoomBrain>().enemiesActive--;
    }
}
