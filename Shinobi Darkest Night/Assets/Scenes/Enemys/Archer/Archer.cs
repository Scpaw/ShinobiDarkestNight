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
    [SerializeField] int fireRange;
    [SerializeField] public int sightRange;
    [SerializeField] float shootingRate_Min = 4f;
    [SerializeField] float shootingRate_Max = 6f;
    [SerializeField] float nextShoot = 0.5f;
    private bool playerInRange;

    [Header("Archer Movement Parameters")]
    [SerializeField] int archerSpeed;
    public float stundTime;
    Animator EnemyAnim;
    private AILerp canMove;
    private Vector3 startPos;

    private GameObject thePlayer;
    public List<GameObject> projectiles;
    private float distance;

    public void Awake()
    {

        canMove = gameObject.GetComponent<AILerp>();

        EnemyAnim = transform.Find("Grafika").GetComponent<Animator>();
        startPos = transform.position;

        playerInRange = false;
        canMove.speed = 1;
    }
    private void Start()
    {
        thePlayer = PlayerController.Instance.GetPlayer();
        gameObject.GetComponent<AIDestinationSetter>().target = thePlayer.transform;
    }
    private void OnEnable()
    {
        canMove = gameObject.GetComponent<AILerp>();

        EnemyAnim = transform.Find("Grafika").GetComponent<Animator>();
        startPos = transform.position;

        playerInRange = false;
        canMove.speed = archerSpeed;
    }

    void FixedUpdate()
    {
        distance = Vector2.Distance(transform.position, thePlayer.transform.position);

        if (playerInRange == true && distance < fireRange)
        {
            canMove.speed = 0;

            if (Time.time > nextShoot)
            {
                nextShoot = Time.time + Random.Range(shootingRate_Min, shootingRate_Max);
                SpawnPoint();
            }
        }
        else
        {
            canMove.speed = archerSpeed;
        }

        if(distance < sightRange)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }
        
    }

    public void SpawnPoint()
    {
       GameObject newProjectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation.transform.rotation);
    }

}
