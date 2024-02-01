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
    [SerializeField] private LayerMask whatCanSee;
    [SerializeField] private float seeRadius;
    [SerializeField] private List<Vector2> directions;
    [SerializeField] private float timeToCheck;
    private float checkAgain;
    [SerializeField] private Vector2 movingDirection;
    private Vector2 wantToMoveDirection;
    [SerializeField] private bool inZone;
    [SerializeField] private bool right;
    private float lastFlip;
    private Transform player;

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
    [SerializeField] private float detectionRange;


    //
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        astar = GetComponent<AIPath>();
        astar.maxSpeed = speed/100;
        rb = GetComponent<Rigidbody2D>();
        player = PlayerController.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (checkAgain < Time.time)
        {
            CheckDirection(20);
            checkAgain = Time.time + timeToCheck;
        }

        if (lastFlip > 0)
        { 
            lastFlip -= Time.deltaTime;
        }

        astar.Move(movingDirection.normalized * speed * Time.deltaTime);



        if ((player.position - transform.position).magnitude > detectionRange)
        {
            movingDirection = Vector2.LerpUnclamped(movingDirection, astar.desiredVelocity.normalized * speed, changeDirectionSpeed * Time.deltaTime);
        }
        else
        { 
            movingDirection = Vector2.LerpUnclamped(movingDirection, wantToMoveDirection, changeDirectionSpeed * Time.deltaTime);
        }

        Debug.DrawLine(transform.position, (transform.position + astar.desiredVelocity.normalized * speed), Color.magenta, Time.deltaTime);
    }

    private void CheckDirection(int numberOfDirections)
    {
        if (((PlayerController.Instance.transform.position - transform.position).magnitude > playerRange + playerZone || (PlayerController.Instance.transform.position - transform.position).magnitude < playerRange - playerZone))
        {
            inZone = false;
            directions.Add(astar.desiredVelocity.normalized * movingDirValue);
        }

        wantToMoveDirection = Vector2.zero;
        directions.Clear();

        float currentDir = 360 / numberOfDirections;
        while (currentDir <= 360)
        {
            
            Vector3 direction = new Vector2(seeRadius * Mathf.Cos((currentDir * Mathf.Deg2Rad) - (Mathf.Deg2Rad * 90)), seeRadius * Mathf.Sin((currentDir * Mathf.Deg2Rad) - (Mathf.Deg2Rad * 90)));
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction,seeRadius, whatCanSee);
            if (hit.transform != null)
            {
                if (hit.transform.gameObject.layer == 9 || hit.transform.gameObject.layer == 10)
                {
                    //level and obsticle
                    if (inZone)
                    {
                        if (Mathf.Abs(GetAngle(movingDirection.normalized * speed, direction)) < 15)
                        {
                            Debug.DrawLine(transform.position, (transform.position + direction.normalized * seeRadius), Color.blue, timeToCheck);
                            Debug.Log((hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude);
                            if ((hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude < 5)
                            {
                                Flip();
                            }
                        }    
                    }
                    else
                    {
                        Debug.DrawLine(transform.position, (transform.position + direction.normalized * obsticleAndLevel * ((hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude)), Color.cyan, timeToCheck);
                        directions.Add(-direction.normalized * obsticleAndLevel * (hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude);
                    }

                }
                else if (hit.transform.tag == "Player")
                {
                    //player
                    if (!((PlayerController.Instance.transform.position - transform.position).magnitude < playerRange + playerZone && (PlayerController.Instance.transform.position - transform.position).magnitude > playerRange - playerZone))
                    {
                        Debug.DrawLine(transform.position, (transform.position + direction.normalized * playerValue * ((hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude)), Color.blue, timeToCheck);
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
                            Debug.DrawLine(transform.position, (transform.position + direction.normalized * enemy * ((hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude)), Color.red, timeToCheck);
                            if ((hit.transform.position - transform.position).magnitude < 2)
                            {
                                Flip();
                            }
                        }
                    }
                    else
                    {
                        Debug.DrawLine(transform.position, (transform.position + direction.normalized * enemy * ((hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude)), Color.yellow, timeToCheck);
                        directions.Add(-direction.normalized * enemy * (hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude);
                    }

                }
                else
                {
                    //Debug.DrawLine(transform.position, (transform.position + direction.normalized ), Color.red, timeToCheck);
                }
            }
            else
            {
                //Debug.DrawLine(transform.position, (transform.position + direction), Color.white, timeToCheck);
            }
            currentDir += 360 / numberOfDirections;
        }

        if (inZone)
        {
            //circling the player
            //Vector2 perpendicular = Vector2.Perpendicular(PlayerController.Instance.transform.position - transform.position);
            
            if (right)
            {
                Vector2 perpendicular = RotateVector(Vector2.Perpendicular(PlayerController.Instance.transform.position - transform.position), -15);
                directions.Add(perpendicular);
                Debug.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y) + perpendicular, Color.green, timeToCheck);
            }
            else
            {
                Vector2 perpendicular = RotateVector(Vector2.Perpendicular(PlayerController.Instance.transform.position - transform.position), 15);
                directions.Add(-perpendicular);
                Debug.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y) - perpendicular, Color.green, timeToCheck);
            }

        }


        foreach (Vector2 direction in directions) 
        {
            wantToMoveDirection += direction;
        }
        Debug.DrawLine(transform.position, (new Vector2(transform.position.x, transform.position.y) + wantToMoveDirection), Color.yellow, timeToCheck);
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
