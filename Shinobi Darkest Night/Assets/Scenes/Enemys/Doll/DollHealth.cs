using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DollHealth : MonoBehaviour
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

    //projectiles
    public List<GameObject> projectiles;

    public void Awake()
    {
        enemyHealth = enemyMaxHealth;
        enemyHealthSlider.maxValue = enemyMaxHealth;
        enemyHealthSlider.value = enemyHealth;
    }

    public void enemyAddDamage(float Damage, bool dropProjectiles)
    {
        enemyHealth -= Damage;
        enemyHealthSlider.value = enemyHealth;
        if (enemyHealth <= 0)
        {
            MakeDead();
        }
        if (dropProjectiles)
        {
            ProjectilesOff();
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

    public void AddProjectile(GameObject projectile)
    {
        projectiles.Add(projectile);
    }

    private void MakeDead()
    {
        ProjectilesOff();
        Destroy(gameObject, 0.1f);
    }

    void ProjectilesOff()
    {
        foreach (GameObject projectile in projectiles)
        {
            projectile.transform.parent = null;
            projectile.GetComponent<Rigidbody2D>().AddForce((projectile.transform.position - transform.position) * 3, ForceMode2D.Impulse);
        }
        if (projectiles.Count > 0)
        {
            projectiles.Clear();
        }
    }

    private void OnDestroy()
    {
        ProjectilesOff();
    }
}
