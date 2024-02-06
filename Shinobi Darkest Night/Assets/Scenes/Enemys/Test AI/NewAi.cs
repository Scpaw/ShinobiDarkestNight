using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAi : MonoBehaviour
{
    //astar
    private AIPath astar;

    //AI
    [SerializeField] private float speed;
    public bool canMove;
    [SerializeField] private LayerMask whatCanSee;
    [SerializeField] private float seeRadius;
    [SerializeField] private List<Vector2> directions;
    [SerializeField] private float timeToCheck;
    private float checkAgain;
    [SerializeField] private Vector2 movingDirection;
    private Vector2 wantToMoveDirection;
    private Vector3 enemyNear;
    [SerializeField] private bool inZone;
    [SerializeField] private bool right;
    private float lastFlip;
    private Transform player;
    private float velocityControl;
    private float runFromPlayer;

    [Header("Detection Values")]

    [Range(0.0f, 1.0f)]
    [SerializeField] private float obsticleAndLevel;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float enemy;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float playerValue;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float movingDirValue;

    [SerializeField] private float playerRange;
    [SerializeField] private float playerZone;
    [SerializeField] private float changeDirectionSpeed;
    public float detectionRange;

    //attack
    [Header("Attack")]
    private float lastAttack;
    [SerializeField] bool attack;
    public float attackRate;
    public float attackRange = 1.1f;

    //rigidbody
    private Rigidbody2D rb;

    //hit
    private Coroutine stuned;

    //debug
    [SerializeField] private bool debugMode;

    // Start is called before the first frame update
    void Start()
    {
        astar = GetComponent<AIPath>();
        astar.maxSpeed = speed/100;
        rb = GetComponent<Rigidbody2D>();
        player = PlayerController.Instance.transform;
        playerRange += Random.Range(-3f, 1f);
        attack = false;
        canMove = true;
        lastAttack = Time.time + Random.Range(0.7f, 3f);
        velocityControl = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        //test
        if (Input.GetKeyDown(KeyCode.I))
        {
            Stun(0.4f, (transform.position - player.transform.position).normalized * 3);
        }


        if (lastAttack < Time.time && inZone && !attack)
        { 
            Attack();
        }


        if (lastFlip > 0)
        { 
            lastFlip -= Time.deltaTime;
        }

        if (canMove && (player.position - transform.position).magnitude < detectionRange && stuned == null)
        {
            if (!astar.enabled)
            {
                astar.enabled = true;
            }

            if (checkAgain < Time.time)
            {
                CheckDirection(20);
                checkAgain = Time.time + timeToCheck;
            }

            if ( attack || (player.position - transform.position).magnitude > seeRadius)
            {
                movingDirection = Vector2.LerpUnclamped(movingDirection, astar.desiredVelocity.normalized + enemyNear/10, changeDirectionSpeed * Time.deltaTime);
            }
            else
            {
                movingDirection = Vector2.LerpUnclamped(movingDirection, wantToMoveDirection, changeDirectionSpeed * Time.deltaTime);
            }

            if ((player.position - transform.position).magnitude < playerRange - playerZone && !attack)
            {
                runFromPlayer = 1 + 1.1f * (1- ((player.position - transform.position).magnitude / (playerRange - playerZone)));
            }
            else if (runFromPlayer != 1)
            {
                runFromPlayer = 1;
            }

            astar.Move(movingDirection.normalized * speed * velocityControl * runFromPlayer * Time.deltaTime);
        }
        else
        {
            if (movingDirection != Vector2.zero)
            {
                movingDirection = Vector2.zero;
            }
        }

        if (debugMode)
        {
            Debug.DrawLine(transform.position, (transform.position + astar.desiredVelocity.normalized * speed), Color.magenta, Time.deltaTime);
        }
    }

    private void CheckDirection(int numberOfDirections)
    {
        if (((PlayerController.Instance.transform.position - transform.position).magnitude > playerRange + playerZone || (PlayerController.Instance.transform.position - transform.position).magnitude < playerRange - playerZone))
        {
            inZone = false;
            directions.Add(astar.desiredVelocity.normalized * movingDirValue);
        }
        enemyNear = Vector2.zero;
        wantToMoveDirection = Vector2.zero;
        directions.Clear();

        float currentDir = 360 / numberOfDirections;
        while (currentDir <= 360)
        {
            
            Vector3 direction = new Vector2(seeRadius * Mathf.Cos((currentDir * Mathf.Deg2Rad) - (Mathf.Deg2Rad * 90)), seeRadius * Mathf.Sin((currentDir * Mathf.Deg2Rad) - (Mathf.Deg2Rad * 90)));
            Vector3 colliderCorrection = new Vector2(GetComponent<CircleCollider2D>().radius * Mathf.Cos((currentDir * Mathf.Deg2Rad) - (Mathf.Deg2Rad * 90)), GetComponent<CircleCollider2D>().radius * Mathf.Sin((currentDir * Mathf.Deg2Rad) - (Mathf.Deg2Rad * 90)));
            RaycastHit2D hit = Physics2D.Raycast(transform.position + colliderCorrection * 1.1f, direction,seeRadius, whatCanSee);
            if (hit.transform != null)
            {
                if (hit.transform.gameObject.layer == 9 || hit.transform.gameObject.layer == 10)
                {
                    //level and obsticle
                    if (inZone)
                    {
                        if (Mathf.Abs(GetAngle(movingDirection.normalized * speed, direction)) < 15)
                        {
                            if (debugMode)
                            {
                                Debug.DrawLine(transform.position, (transform.position + direction.normalized * seeRadius), Color.blue, timeToCheck);
                            }

                            if ((hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude < 2)
                            {
                                Flip();
                            }
                        }    
                    }
                    else
                    {
                        if (debugMode)
                        {
                            Debug.DrawLine(transform.position, (transform.position + direction.normalized * obsticleAndLevel * (seeRadius - (hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude)), Color.cyan, timeToCheck);
                        }

                        directions.Add(-direction.normalized * obsticleAndLevel * (seeRadius - (hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude));
                    }

                }
                else if (hit.transform.tag == "Player")
                {
                    //player
                    if (!((PlayerController.Instance.transform.position - transform.position).magnitude < playerRange + playerZone && (PlayerController.Instance.transform.position - transform.position).magnitude > playerRange - playerZone))
                    {
                        if (debugMode)
                        {
                            Debug.DrawLine(transform.position, (transform.position + direction.normalized * playerValue * (seeRadius - (hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude)), Color.blue, timeToCheck);
                        }
                        directions.Add(direction.normalized * playerValue * ((PlayerController.Instance.transform.position - transform.position).magnitude - playerRange));
                    }
                    else
                    {
                        inZone = true;
                        //movingDirection = Vector2.zero;
                    }
                }
                else if (hit.transform.tag == "Enemy")
                {
                    //enemy
                    if (inZone)
                    {
                        if (Mathf.Abs(GetAngle(movingDirection.normalized * speed, direction)) < 5)
                        {
                            if (debugMode)
                            {
                                Debug.DrawLine(transform.position, (transform.position + direction.normalized * enemy * ((hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude)), Color.red, timeToCheck);
                            }
                            if ((hit.transform.position - transform.position).magnitude < 1)
                            {
                                Debug.Log("Enemy");
                                Flip();
                            }
                        }
                    }
                    else
                    {
                        if (debugMode)
                        {
                            Debug.DrawLine(transform.position, (transform.position + direction.normalized * enemy * (seeRadius - (hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude)), Color.yellow, timeToCheck);
                        }
                        directions.Add(-direction.normalized * enemy * (seeRadius - (hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude));
                        enemyNear += (-direction.normalized * enemy * (seeRadius - (hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude));
                    }

                }
            }
            currentDir += 360 / numberOfDirections;
        }

        if (inZone)
        {
            //circling the player

            if (right)
            {
                Vector2 perpendicular = RotateVector(Vector2.Perpendicular(PlayerController.Instance.transform.position - transform.position), -15);
                directions.Add(perpendicular);
                if (debugMode)
                {
                    Debug.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y) + perpendicular, Color.green, timeToCheck);
                }
            }
            else
            {
                Vector2 perpendicular = RotateVector(Vector2.Perpendicular(PlayerController.Instance.transform.position - transform.position), 15);
                directions.Add(-perpendicular);
                if (debugMode)
                {
                    Debug.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y) - perpendicular, Color.green, timeToCheck);
                }
            }

        }


        foreach (Vector2 direction in directions) 
        {
            wantToMoveDirection += direction;
        }
        if (debugMode)
        {
            Debug.DrawLine(transform.position, (new Vector2(transform.position.x, transform.position.y) + wantToMoveDirection), Color.yellow, timeToCheck);
        }

    }

    public void Attack()
    {
        attack = true;
        velocityControl = 1.7f;
    }

    public void EndAttack()
    {
        attack = false;
        velocityControl = 1;
        lastAttack = Time.time + Random.Range(attackRate * 1.3f,attackRate * 0.7f);
    }

    private void Flip()
    {
        if (lastFlip <= 0)
        {
            movingDirection = Vector2.zero;
            right = !right;
            lastFlip = 0.1f;
        }
    }

    public void Stun(float time, Vector2 force)
    {
        if (stuned != null)
        { 
            StopCoroutine(stuned);
        }
        StartCoroutine(StunMe(time,force));
    }

    private IEnumerator StunMe(float time, Vector2 force)
    {
        canMove = false;
        astar.enabled = false;
        rb.velocity = Vector2.zero;
        rb.AddForce(force,ForceMode2D.Impulse);
        while (time > 0)
        { 
            time-= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        canMove = true;
    }

    private Vector2 RotateVector(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta * Mathf.Deg2Rad) - v.y * Mathf.Sin(delta * Mathf.Deg2Rad),
            v.x * Mathf.Sin(delta * Mathf.Deg2Rad) + v.y * Mathf.Cos(delta * Mathf.Deg2Rad)
        );
    }

    private float GetAngle(Vector2 vec1, Vector2 vec2)
    {
        return Vector2.Angle(vec1,vec2);
    }

   

    private void OnDrawGizmos()
    {
        if (debugMode)
        {
            Gizmos.DrawWireSphere(transform.position, seeRadius);
            Gizmos.color = Color.blue;
            Gizmos.color = Color.red;
            if (PlayerController.Instance != null)
            {
                Gizmos.DrawWireSphere(PlayerController.Instance.transform.position, playerRange + playerZone);
                Gizmos.DrawWireSphere(PlayerController.Instance.transform.position, playerRange - playerZone);
            }
        }
    }

}
