using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
//using static TreeEditor.TreeEditorHelper;

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
    [SerializeField] private GameObject thePlayer;

    Animator EnemyAnim;
    private Vector3 startPos;
    //projectiles
    public List<GameObject> projectiles;
    public float stundTime;

    public void Awake()
    {
        enemyHealth = enemyMaxHealth;
        enemyHealthSlider.maxValue = enemyMaxHealth;
        enemyHealthSlider.value = enemyHealth;

        EnemyAnim = transform.Find("Grafika").GetComponent<Animator>();
        startPos = transform.position;
    }
    private void OnEnable()
    {
        enemyHealth = enemyMaxHealth;
        enemyHealthSlider.maxValue = enemyMaxHealth;
        enemyHealthSlider.value = enemyHealth;
        EnemyAnim = transform.Find("Grafika").GetComponent<Animator>();
        transform.position = startPos;
    }

    public void enemyAddDamage(float Damage, bool dropProjectiles)
    {

        ParticleManager.instance.UseParticle("Blood", transform);
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
        StopAllCoroutines();
        ProjectilesOff();
        gameObject.SetActive(false);
        //Destroy(gameObject, 0.1f);
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

    public IEnumerator Stuned()
    {
        stundTime = 0.75f;
        while (stundTime > 0)
        {
            stundTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

    }
    private void OnDisable()
    {
        if (transform.parent != null && enemyHealth <=0)
        {
            transform.parent.GetComponent<RoomBrain>().SpawnEnemies();
        }
    }
}
