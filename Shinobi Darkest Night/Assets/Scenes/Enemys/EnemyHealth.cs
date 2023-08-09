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
        enemyHealth = enemyMaxHealth;
        enemyHealthSlider.maxValue = enemyMaxHealth;
        enemyHealthSlider.value = enemyHealth;
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
        //Destroy(gameObject, 0.1f);
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

    public IEnumerator Stuned()
    {
        isStuned = true;
        stundTime = 0.75f;
        while (stundTime > 0)
        {
            stundTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        isStuned = false;
    }
    private void OnDisable()
    {
        if (transform.parent != null && enemyHealth <= 0)
        {
            transform.parent.GetComponent<RoomBrain>().SpawnEnemies();
        }
    }
}
