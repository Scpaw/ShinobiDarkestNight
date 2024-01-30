using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health")]
    public float enemyMaxHealth;
    public float enemyHealth;
    [SerializeField] Slider enemyHealthSlider;
    private Text enemyHealthText;
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
    private Coroutine stunCorutine;
    public float canAttackAgain;
    
    //hp drop
    [SerializeField] GameObject hpPoint;
    private float canDropHp;
    private PlayerHealth playerHP;

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
            enemyHealthText = enemyHealthSlider.GetComponentInChildren<Text>();
            enemyHealthText.text = ((int)enemyHealth).ToString();
        }
        canBeAttacked = true;
        if (transform.parent.transform.GetComponent<RoomBrain>() != null)
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

        if (playerHP == null)
        { 
            playerHP = PlayerController.Instance.GetComponent<PlayerHealth>();
        }    
    }

    private void Update()
    {
        if (canAttackAgain > 0)
        {
            canAttackAgain -= Time.deltaTime;
        }

        if (canDropHp > 0)
        { 
            canDropHp -= Time.deltaTime;
        }
    }

    public void enemyAddDamage(float Damage, bool dropProjectiles, bool useparticle)
    {
        if (canDoDmg)
        {

            PlayerController.Instance.RegenAttacks(Damage/2);

            canAttackAgain = 0.9f;
            if (useparticle && useParticles)
            {
                ParticleManager.instance.UseParticle("Blood", transform.position, transform.rotation.eulerAngles);
            }
            enemyHealth -= Damage;
            if (enemyHealthSlider != null)
            {
                enemyHealthSlider.value = enemyHealth;
                enemyHealthText.text = ((int) enemyHealth).ToString();
            }
            if (dropProjectiles)
            {
                ProjectilesOff(0);
            }
            if (enemyHealth <= 0)
            {
                MakeDead();
            }
            else
            {
                HitDropHp(Damage);
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

    private void DropHp()
    {
        int drop = (int)Random.Range(1, (enemyMaxHealth/35) * (1 - playerHP.GetPlayerHP()));
        while (drop > 0)
        {
            Instantiate(hpPoint, transform.position, Quaternion.Euler(Vector3.zero), null);
            drop--;
        }
    }

    private void HitDropHp(float dmg)
    {
        if (dmg > 60)
        {
            dmg = 60;
        }
        int drop = Mathf.RoundToInt(1-(enemyHealth / enemyMaxHealth) + (dmg / enemyMaxHealth) + Random.Range(-0.07f, 0.07f) - 0.35f + 0.22f* (1- playerHP.GetPlayerHP()));

        if (drop < 1 || Random.value > 0.65f || canDropHp > 0)
        {
            return;
        }

        canDropHp = 0.1f;
        while (drop > 0)
        {
            GameObject HPPoint = Instantiate(hpPoint, transform.position, Quaternion.Euler(Vector3.zero), null);
            drop--;
        }
    }

    private void MakeDead()
    {
        StopAllCoroutines();
        ProjectilesOff(0);
        DropHp();
        CameraShake.instance.Shake(EShakeStrenght.extraStrong, EShakeShape.recoil);
        gameObject.SetActive(false);
    }

    public void ProjectilesOff(float projectileAddForce,int projectilesToRemove = 9999, float dmgFromProjectile = 0)
    {
        if (projectilesToRemove == 9999)
        {
            projectilesToRemove = projectiles.Count;
        }

        while (projectilesToRemove > 0 && projectiles.Count > 0) 
        {
            if (projectiles.Count >= projectilesToRemove)
            {
                if (dmgFromProjectile > 0)
                {
                    enemyAddDamage(dmgFromProjectile, false, false);
                }
                projectiles[projectilesToRemove - 1].transform.parent = null;
                projectiles[projectilesToRemove - 1].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                projectiles[projectilesToRemove - 1].GetComponent<Rigidbody2D>().AddForce((projectiles[projectilesToRemove - 1].transform.position - transform.position) * (4 + projectileAddForce), ForceMode2D.Impulse);
                projectiles.Remove(projectiles[projectilesToRemove - 1]);
            }
            projectilesToRemove--;        
        }
    }

    public void Stun()
    {
        if (canBeAttacked && canDoDmg && gameObject.activeInHierarchy)
        {
            if (stunCorutine != null)
            {
                StopCoroutine(stunCorutine);
            }

            stunCorutine = StartCoroutine(Stuned());
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

        if (!canBeAttacked)
        {
            StopAllCoroutines();
        }
        if (transform.parent.GetComponent<RoomBrain>())
        {
            if (transform.parent.GetComponent<RoomBrain>() && enemyHealth <= 0)
            {
                transform.parent.GetComponent<RoomBrain>().SpawnEnemies();
            }
            transform.parent.GetComponent<RoomBrain>().enemiesActive--;
        }
        else if (transform.parent.GetComponent<activateEnemy>())
        {
            
            transform.parent.GetComponent<activateEnemy>().Activate();
        }
    }
}
