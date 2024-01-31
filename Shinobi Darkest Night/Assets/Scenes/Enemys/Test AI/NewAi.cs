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

    [Range(0.0f, 1.0f)]
    [SerializeField] private float obsticleAndLevel;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float enemy;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float playerRange;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float movingDirValue;

    //
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        astar = GetComponent<AIPath>();
        astar.maxSpeed = speed;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (checkAgain < Time.time)
        {
            CheckDirection(8);
            checkAgain = Time.time + timeToCheck;
        }
        //astar.

        //Debug.Log(rb.velocity);
        Debug.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y) - rb.velocity, Color.green, Time.deltaTime);
    }

    private void CheckDirection(int numberOfDirections)
    { 
        directions.Clear();
        float currentDir = 360 / numberOfDirections;
        while (currentDir <= 360)
        { 
            Vector3 direction = new Vector2(seeRadius * Mathf.Cos((currentDir * Mathf.Deg2Rad) - (Mathf.Deg2Rad * 90)), seeRadius * Mathf.Sin((currentDir * Mathf.Deg2Rad) - (Mathf.Deg2Rad * 90)));
            Debug.DrawLine(transform.position, (transform.position - direction), Color.red, timeToCheck);
            currentDir += 360 / numberOfDirections;
        }

    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, seeRadius);
        Gizmos.color = Color.blue;
    }

}
