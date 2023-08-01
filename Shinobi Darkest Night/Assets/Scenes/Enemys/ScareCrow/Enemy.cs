using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TreeEditor.TreeEditorHelper;

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

    //projectiles
    public List<GameObject> projectiles;

    public void Awake()
    {
        enemyHealth = enemyMaxHealth;
        enemyHealthSlider.maxValue = enemyMaxHealth;
        enemyHealthSlider.value = enemyHealth;
        EnemyAnim = transform.Find("Grafika").GetComponent<Animator>();
    }

    public void enemyAddDamage(float Damage, bool dropProjectiles)
    {
        enemyHealth -= Damage;
        enemyHealthSlider.value = enemyHealth;
        isdamaged = true;
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
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Static)
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.RandomRange(-2, 2), Random.RandomRange(-2, 2)), ForceMode2D.Impulse);
            }
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
            Debug.Log((projectile.transform.position - transform.position) * 3);
        }
        projectiles.Clear();
    }

    private void OnDestroy()
    {
        ProjectilesOff();
    }
}
