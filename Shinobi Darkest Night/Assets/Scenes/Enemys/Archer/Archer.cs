using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
using Pathfinding.Util;

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
    [SerializeField] float lowHpShootingSpeed = 1;

    [Header("Archer Movement Parameters")]
    [SerializeField] float archerSpeed;
    public float stundTime;
    Animator EnemyAnim;
    private AIPath canMove;
    private AI_Move AI;
    private Vector3 startPos;
    private GameObject thePlayer;
    public List<GameObject> projectiles;
    [SerializeField] LayerMask layerToHit;
    private bool canShootPlayer;
    private GameObject pointTarget;
    private float walkingTime;
    [SerializeField] private float backRadius;
    private float runBackTimer = 0;
    private int runBackIteration = 0;
    private EnemyHealth health;

    [Header("Archer melee")]
    [SerializeField] float meleeDamage;
    [SerializeField] float meleeAttackTime;
    private float meleeTime;
    private DamageRange damageRange;
    private AIDestinationSetter target;
    private int meleeAttackNum;

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
        if (thePlayer == null)
        {
            thePlayer = PlayerController.Instance.GetPlayer();
        }
        if (AI == null)
        {
            AI = GetComponent<AI_Move>();
        }

        if (target == null)
        {
            target = GetComponent<AIDestinationSetter>();
        }
        if (health == null)
        {
            health = GetComponent<EnemyHealth>();
        }
        gameObject.GetComponent<AIDestinationSetter>().target = thePlayer.transform;
        nextShoot = Time.time + Random.Range(shootingRate_Min, shootingRate_Max);
        canShootPlayer = true;
        meleeAttackNum = 0;
    }


    void FixedUpdate()
    {
        if (AI.IsMoving() && canMove.canMove && !AI.stop)
        {
            EnemyAnim.SetFloat("Moving", 1);
        }
        else
        {
            EnemyAnim.SetFloat("Moving", 0);
        }
            

        if (playerInRange)
        {
            if (hit(transform.position).transform.gameObject.layer != 7 && walkingTime <= 0)
            {
                if (canShootPlayer && !shooting)
                {
                    AI.canMove = true;
                    canShootPlayer = false;
                    pointTarget.transform.position = canShootPoint();
                    gameObject.GetComponent<AIDestinationSetter>().target = pointTarget.transform;
                    walkingTime = 2;
                }
            }
            else if(canShootPlayer && walkingTime <=0)
            {

                if (damageRange.playerInRange || (transform.position - player.transform.position).magnitude <= backRadius && !AI.stop && meleeAttackNum > Random.Range(1, 2))
                {
                    AI.canMove = false;
                    if (Time.time > meleeTime && damageRange.playerInRange)
                    {
                        walkingTime = 0;
                        EnemyAnim.SetTrigger("Melee");
                        meleeAttackNum++;
                        meleeTime = Time.time + meleeAttackTime;
                    }
                    else if ((transform.position - player.transform.position).magnitude <= backRadius && !damageRange.playerInRange && (transform.position - player.transform.position).magnitude > 0.1f && runBackTimer <= 0 && runBackIteration < 4 && meleeAttackNum > Random.Range(1, 2))
                    {
                        EnemyAnim.SetTrigger("Back");
                        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                        AI.canMove = true;
                        canMove.maxSpeed = AI.enemySpeed;
                        canShootPlayer = false;
                        pointTarget.transform.position = RunBack();
                        target.target = pointTarget.transform;
                        walkingTime = 2;
                        runBackTimer = 0.3f;
                    }
                }
                else
                {
                    AI.canMove = false;
                    if (Time.time > nextShoot)
                    {
                        if (health.enemyHealth / health.enemyMaxHealth > 0.75f)
                        {
                            nextShoot = Time.time + Random.Range(shootingRate_Min, shootingRate_Max);
                            EnemyAnim.SetFloat("AnimationSpeed", 1);
                        }
                        else
                        {
                            EnemyAnim.SetFloat("AnimationSpeed", 1.5f);
                            nextShoot = Time.time + Random.Range(shootingRate_Min, shootingRate_Max)/lowHpShootingSpeed;
                        }


                        if (shots > Random.Range(4, 6))
                        {
                            shooting = true;
                            runBackIteration = 0;
                            EnemyAnim.SetTrigger("3shot");
                            shots = 0;
                        }
                        else
                        {
                            shooting = true;
                            runBackIteration = 0;
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
                AI.canMove = true;
            }

        }

        if(fireRange())
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
            walkingTime = 0;
        }
        if (target.target.transform.position.x -transform.position.x > 0 && transform.localScale.x < 0 || target.target.transform.position.x - transform.position.x < 0 && transform.localScale.x > 0)
        {
            Flip(transform);
            Flip(projectileSpawnPoint.parent.transform);
            Flip(transform.GetComponentInChildren<Canvas>().transform);
        }

        if (runBackTimer > 0)
        {
            runBackTimer -= Time.deltaTime;
        }
        if (walkingTime > 0)
        {
            walkingTime -= Time.fixedDeltaTime;
        }
        if (((transform.position - pointTarget.transform.position).magnitude < 0.1f || walkingTime<=0 ) && gameObject.GetComponent<AIDestinationSetter>().target != thePlayer.transform)
        {         
            walkingTime = 0;
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
         while (hit(posToReturn).transform.gameObject.layer != 7 && i <75)
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


    private Vector3 RunBack()
    {
        float R = backRadius/2;
        Vector3 posToReturn = new Vector3(0,0,0);
        List<Vector3> points = new List<Vector3> ();
        int i = 0;
        while (i < 30)
        {
            Vector3 check = new Vector3();
            check = (Random.insideUnitCircle * R) + new Vector2(transform.position.x, transform.position.y);
            if (!CheckIfCanGo(check))
            {
                points.Add(check);
            }
            i++;
        }
        foreach (Vector3 check in points)
        {
            if (posToReturn == Vector3.zero || (check - player.position).magnitude > (posToReturn - player.position).magnitude)
            { 
                posToReturn = check;
            }
        }
        if (posToReturn == Vector3.zero)
        {
            runBackIteration++;
            walkingTime = 0;
            meleeAttackNum = 100;
            return transform.position;
        }
        else
        {
            runBackIteration++;
            Debug.DrawRay(transform.position, -(transform.position - posToReturn).normalized, Color.green, 5);
            meleeAttackNum = 0;
            return posToReturn;
        }


    }

    private bool CheckIfCanGo(Vector3 pos)
    {
        if (Physics2D.CircleCast(transform.position, 0.23f, -(transform.position - pos).normalized, backRadius, layerToHit) || (pos - player.transform.position).magnitude <= backRadius)
        {
            Debug.DrawRay(transform.position, -(transform.position - pos).normalized, Color.red, 5);
        }
        else
        {
            Debug.DrawRay(transform.position, -(transform.position - pos).normalized, Color.blue, 5);
        }

        return (Physics2D.CircleCast(transform.position, 0.23f, -(transform.position - pos).normalized, backRadius, layerToHit) || (pos - player.transform.position).magnitude <= backRadius);
    }

}
