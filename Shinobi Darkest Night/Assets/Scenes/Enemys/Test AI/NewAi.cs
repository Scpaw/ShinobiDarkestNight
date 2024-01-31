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
    private Vector2 movingDirection;
    private Vector2 wantToMoveDirection;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float obsticleAndLevel;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float enemy;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float playerValue;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float movingDirValue;

    [SerializeField] private float playerRange;
    [SerializeField] private float changeDirectionSpeed;

    //
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        astar = GetComponent<AIPath>();
        astar.maxSpeed = speed/100;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (checkAgain < Time.time)
        {
            CheckDirection(20);
            checkAgain = Time.time + timeToCheck;
        }

        astar.Move(movingDirection.normalized * speed * Time.deltaTime);

        if (Vector2.Angle(wantToMoveDirection, movingDirection) > 20 || wantToMoveDirection != movingDirection)
        {
            movingDirection = Vector2.Lerp(movingDirection, wantToMoveDirection, changeDirectionSpeed * Time.deltaTime);
        }

        Debug.DrawLine(transform.position, (transform.position + astar.desiredVelocity.normalized * 3), Color.magenta, Time.deltaTime);
    }

    private void CheckDirection(int numberOfDirections)
    { 
        wantToMoveDirection = Vector2.zero;
        directions.Clear();
        directions.Add(astar.desiredVelocity.normalized * movingDirValue);
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
                    Debug.DrawLine(transform.position, (transform.position + direction.normalized * obsticleAndLevel * ((hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude)), Color.cyan, timeToCheck);
                    directions.Add(-direction.normalized * obsticleAndLevel * (hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude);
                }
                else if (hit.transform.tag == "Player")
                {
                    //player
                    Debug.DrawLine(transform.position, (transform.position + direction.normalized * playerValue * ((hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude)), Color.blue, timeToCheck);
                    directions.Add(direction.normalized * playerValue * ((PlayerController.Instance.transform.position - transform.position).magnitude - playerRange));
                }
                else if (hit.transform.tag == "Enemy")
                {
                    //enemy
                    Debug.DrawLine(transform.position, (transform.position + direction.normalized * enemy * ((hit.point - new Vector2( transform.position.x,transform.position.y)).magnitude)), Color.yellow, timeToCheck);
                    directions.Add(-direction.normalized * enemy * (hit.point - new Vector2(transform.position.x, transform.position.y)).magnitude);
                }
                else
                {
                    Debug.DrawLine(transform.position, (transform.position + direction.normalized ), Color.red, timeToCheck);
                }
            }
            else
            {
                Debug.DrawLine(transform.position, (transform.position + direction), Color.white, timeToCheck);
            }
            currentDir += 360 / numberOfDirections;
        }

        foreach (Vector2 direction in directions) 
        {
            wantToMoveDirection += direction;
        }
        Debug.DrawLine(transform.position, (new Vector2(transform.position.x, transform.position.y) + wantToMoveDirection), Color.yellow, timeToCheck);
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, seeRadius);
        Gizmos.color = Color.blue;
    }

}
