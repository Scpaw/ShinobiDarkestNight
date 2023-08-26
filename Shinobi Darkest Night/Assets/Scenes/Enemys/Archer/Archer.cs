using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public class Archer : MonoBehaviour
{
    [Header("Archer Projectile Prefabs")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject projectileRotation;

    [Header("Archer Projectile Parameters")]
    [SerializeField] public int sightRange;
    [SerializeField] float shootingRate_Min = 4f;
    [SerializeField] float shootingRate_Max = 6f;
    [SerializeField] float nextShoot = 0.5f;
    private bool playerInRange;
    private int shots;
    private Transform player;


    [Header("Archer Movement Parameters")]
    [SerializeField] float archerSpeed;
    public float stundTime;
    Animator EnemyAnim;
    private AILerp canMove;
    private Vector3 startPos;
    private GameObject thePlayer;
    public List<GameObject> projectiles;
    [Header("Archer melee")]
    [SerializeField] float meleeDamage;
    [SerializeField] float meleeAttackTime;
    private float meleeTime;
    private DamageRange damageRange;

    public void Awake()
    {
        startPos = transform.position;
        playerInRange = false;
        player = PlayerController.Instance.GetPlayer().transform;
        damageRange = GetComponentInChildren<DamageRange>();
    }
    private void OnEnable()
    {
        if (canMove == null)
        {
            canMove = gameObject.GetComponent<AILerp>();
        }
        if (EnemyAnim == null)
        {
            EnemyAnim = transform.Find("Grafika").GetComponent<Animator>();
        }
        transform.position = startPos;
        playerInRange = false;
        canMove.speed = archerSpeed;
        if (thePlayer == null)
        {
            thePlayer = PlayerController.Instance.GetPlayer();
        }
        gameObject.GetComponent<AIDestinationSetter>().target = thePlayer.transform;
        canMove.speed = 1;
    }

    void FixedUpdate()
    {
        if (canMove.speed > 0 && canMove.enabled)
        {
            EnemyAnim.SetFloat("Moving", 1);
        }
        else
        {
            EnemyAnim.SetFloat("Moving", 0);
        }
            

        if (playerInRange)
        {
            canMove.speed = 0;
            if (damageRange.playerInRange)
            {
                if (Time.time > meleeTime)
                {
                    EnemyAnim.SetTrigger("Melee");
                    meleeTime = Time.time + meleeAttackTime;
                }
            }
            else
            {
                if (Time.time > nextShoot)
                {
                    nextShoot = Time.time + Random.Range(shootingRate_Min, shootingRate_Max);

                    if (shots > Random.Range(4, 6))
                    {
                        EnemyAnim.SetTrigger("3shot");
                        shots = 0;
                    }
                    else
                    {
                        EnemyAnim.SetTrigger("shot");
                        shots++;
                    }

                }
            }
        }
        else
        {
            canMove.speed = archerSpeed;
        }

        if(fireRange())
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }
        if (player.position.x -transform.position.x > 0 && transform.localScale.x < 0 || player.position.x - transform.position.x < 0 && transform.localScale.x > 0)
        {
            Flip(transform);
            Flip(projectileSpawnPoint.parent.transform);
            Flip(transform.GetComponentInChildren<Canvas>().transform);
        }
    }

    bool fireRange()
    {
        return (Camera.main.WorldToScreenPoint(transform.position).x < Screen.width * 0.93 && Camera.main.WorldToScreenPoint(transform.position).x > Screen.width*0.07f && Camera.main.WorldToScreenPoint(transform.position).y < Screen.height *0.93 && Camera.main.WorldToScreenPoint(transform.position).y > Screen.height * 0.07);
    }
    public void Attack()
    {
        if (damageRange.playerInRange)
        {
            player.GetComponent<PlayerHealth>().AddDamage(meleeDamage);
        }
    }
    public void SpawnPoint(bool triple)
    {
        if (triple)
        {
            Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation.transform.rotation);
            Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.Euler(projectileRotation.transform.rotation.eulerAngles.x, projectileRotation.transform.rotation.eulerAngles.y, projectileRotation.transform.rotation.eulerAngles.z -15 ));
            Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.Euler(projectileRotation.transform.rotation.eulerAngles.x, projectileRotation.transform.rotation.eulerAngles.y, projectileRotation.transform.rotation.eulerAngles.z +15 ));
        }
        else
        {
            Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation.transform.rotation);
        }
     
    }

    private void Flip(Transform changeThis)
    {
        changeThis.localScale = new Vector3(-changeThis.localScale.x, changeThis.localScale.y, changeThis.localScale.z);
    }

}
