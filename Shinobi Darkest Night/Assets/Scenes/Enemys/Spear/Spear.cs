using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Spear : MonoBehaviour
{
    private AILerp ai;
    private EnemyHealth enemyScript;
    private float enemySpeed;
    [SerializeField] float detectRadius;
    [SerializeField] float attackRadius;
    private Transform player;
    private Vector3 attackPoint;
    private float timebtwAttacks;
    private bool canMove;

    private void Awake()
    {
        enemySpeed = GetComponent<AILerp>().speed;
    }

    private void OnEnable()
    {
        if (ai == null)
        {
            ai = GetComponent<AILerp>();
        }
        if (enemyScript == null)
        {
            enemyScript = GetComponent<EnemyHealth>();
        }
        if (player == null)
        {
            player = PlayerController.Instance.GetPlayer().transform;
        }

        if (transform.GetComponent<AIDestinationSetter>().target == null)
        {
            transform.GetComponent<AIDestinationSetter>().target = player;
        }
        timebtwAttacks = 0;
    }
    void Update()
    {
        if (transform.parent.GetComponent<AiBrain>().playerIn && (player.position - transform.position).magnitude < attackRadius && timebtwAttacks < 0.3f)
        {
            Debug.Log("player in");
            StartCoroutine(SpearDash());
            timebtwAttacks = 12;
        }
        if (timebtwAttacks > 0)
        { 
            timebtwAttacks -= Time.deltaTime;
        }
        if (transform.parent.GetComponent<AiBrain>().playerIn && (player.position - transform.position).magnitude < detectRadius)
        {
           

            if (enemyScript.stundTime > 0 || !canMove)
            {
                ai.canMove = false;
            }
            else
            {
                if (!ai.canMove)
                {
                    Debug.Log("move now");
                    StartCoroutine(ResetPathf());
                }
            }
        }
        else
        {
            ai.canMove = false;
        }
        if ((player.position - transform.position).magnitude < 1.5f)
        {
            ai.enabled = false;
        }
        else
        {
            ai.enabled = true;
        }
    }

    private IEnumerator SpearDash()
    {
        canMove = false;
        Debug.Log("start");
        float i = 0;
        ai.canMove = false;
        attackPoint = player.position + ((player.position - transform.position).normalized * 2);


        while (transform.position != attackPoint)
        {
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, attackPoint.x, i/5), Mathf.Lerp(transform.position.y, attackPoint.y, i/5), 0);
            i += Time.deltaTime;
            Debug.Log("moving");
            yield return new WaitForEndOfFrame();
        }
        canMove = true;
        StartCoroutine(ResetPathf());
    }
    private IEnumerator ResetPathf()
    {
        if (canMove)
        {
            ai.SetNewPath();
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            ai.speed = 0;
            while (enemySpeed > ai.speed)
            {
                if (ai.speed > 0.1f)
                {
                    ai.canMove = true;
                }
                ai.speed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
       
    }
}
