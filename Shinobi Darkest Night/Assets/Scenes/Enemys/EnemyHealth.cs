using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health")]
    [SerializeField] float enemyMaxHealth;
    [SerializeField] float enemyHealth;
    [SerializeField] Slider enemyHealthSlider;
    [SerializeField] GameObject enemyCanvas;
    public bool canBeAttacked;
    private float canBeAttackedTimer;

    public List<GameObject> projectiles;
    public float stundTime;
    public bool isStuned;

    public void Awake()
    {
        enemyHealth = enemyMaxHealth;
        enemyHealthSlider.maxValue = enemyMaxHealth;
        enemyHealthSlider.value = enemyHealth;
    }
    private void OnEnable()
    {
        canBeAttacked = false;
        enemyHealth = enemyMaxHealth;
        enemyHealthSlider.maxValue = enemyMaxHealth;
        enemyHealthSlider.value = enemyHealth;
        canBeAttacked = true;
    }

    public void enemyAddDamage(float Damage, bool dropProjectiles, bool useparticle)
    {
        if (useparticle)
        {
            ParticleManager.instance.UseParticle("Blood", transform.position, transform.rotation.eulerAngles);
        }
        enemyHealth -= Damage;
        enemyHealthSlider.value = enemyHealth;
        if (dropProjectiles)
        {
            ProjectilesOff();
        }
        if (enemyHealth <= 0)
        {
            MakeDead();
        }
    }

    public void AddProjectile(GameObject projectile)
    {
        projectiles.Add(projectile);
    }

    private void MakeDead()
    {
        StopAllCoroutines();
        ProjectilesOff();
        gameObject.SetActive(false);
    }

    void ProjectilesOff()
    {
        foreach (GameObject projectile in projectiles)
        {
            projectile.transform.parent = null;
            projectile.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            projectile.GetComponent<Rigidbody2D>().AddForce((projectile.transform.position - transform.position) * 3, ForceMode2D.Impulse);
        }
        if (projectiles.Count > 0)
        {
            projectiles.Clear();
        }
    }

    public IEnumerator Stuned(bool meeleAttack)
    {
        if (canBeAttacked || !meeleAttack)
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
    }
}
