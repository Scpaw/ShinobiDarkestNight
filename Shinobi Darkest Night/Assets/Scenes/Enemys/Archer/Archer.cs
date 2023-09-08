using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
using Pathfinding.Util;
using Unity.VisualScripting;

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
    public bool shooting;

    [Header("Archer Movement Parameters")]
    [SerializeField] float archerSpeed;
    public float stundTime;
    Animator EnemyAnim;
    private AIPath canMove;
    private Vector3 startPos;
    private GameObject thePlayer;
    public List<GameObject> projectiles;
    [SerializeField] LayerMask layerToHit;
    private bool canShootPlayer;
    private GameObject pointTarget;
    private float walkingTime;

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
        pointTarget = Instantiate(new GameObject(), transform.position, transform.rotation);
    }
    private void OnEnable()
    {
        if (canMove == null)
        {
            canMove = gameObject.GetComponent<AIPath>();
        }
        if (EnemyAnim == null)
        {
            EnemyAnim = transform.Find("Grafika").GetComponent<Animator>();
        }
        transform.position = startPos;
        playerInRange = false;
        canMove.maxSpeed = archerSpeed;
        if (thePlayer == null)
        {
            thePlayer = PlayerController.Instance.GetPlayer();
        }
        gameObject.GetComponent<AIDestinationSetter>().target = thePlayer.transform;
        canMove.maxSpeed = 1;
        nextShoot = Time.time + Random.Range(shootingRate_Min, shootingRate_Max);
        canShootPlayer = true;
    }

    void FixedUpdate()
    {
        if (canMove.maxSpeed > 0 && canMove.enabled)
        {
            EnemyAnim.SetFloat("Moving", 1);
        }
        else
        {
            EnemyAnim.SetFloat("Moving", 0);
        }
            

        if (playerInRange)
        {
            if (hit(transform.position).transform.gameObject.layer != 7)
            {
                if (canShootPlayer)
                {
                    canMove.maxSpeed = archerSpeed;
                    canShootPlayer = false;
                    pointTarget.transform.position = canShootPoint();
                    gameObject.GetComponent<AIDestinationSetter>().target = pointTarget.transform;
                    walkingTime = 2;
                }
            }
            else if(canShootPlayer)
            {
                canMove.maxSpeed = 0;
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
                            shooting = true;
                            EnemyAnim.SetTrigger("3shot");
                            shots = 0;
                        }
                        else
                        {
                            shooting = true;
                            EnemyAnim.SetTrigger("shot");
                            shots++;
                        }

                    }
                }
            }
           
        }
        else
        {
            if (!shooting)
            {
                canMove.maxSpeed = archerSpeed;
            }
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

        if (walkingTime > 0)
        {
            walkingTime -= Time.fixedDeltaTime;
        }
        if ((transform.position == pointTarget.transform.position || walkingTime<0 )&& !canShootPlayer)
        { 
            canShootPlayer = true;
            gameObject.GetComponent<AIDestinationSetter>().target = thePlayer.transform;
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
        shooting = false;
    }

    private void Flip(Transform changeThis)
    {
        changeThis.localScale = new Vector3(-changeThis.localScale.x, changeThis.localScale.y, changeThis.localScale.z);
    }

    private RaycastHit2D hit(Vector3 pos)
    { 
        return Physics2D.CircleCast(pos, 0.2f, (player.position - pos).normalized, 30, layerToHit);
    }
    private Vector2 dir(Vector3 pos)
    { 
        return (player.position - pos).normalized;
    }
    private Vector3 canShootPoint()
    {
        Vector2 posToReturn = transform.position;
        int i = 0;
         while (hit(posToReturn).transform.gameObject.layer != 7 && i <15)
         {
            posToReturn = transform.position;
            if (i % 2 == 1)
            {
                posToReturn = posToReturn + new Vector2( dir(posToReturn).x * i/2,0);
            }
            else
            {
                posToReturn = posToReturn + new Vector2 (0,dir(posToReturn).y * i);
            }
            i++;
         }
        return posToReturn;
    }


}
