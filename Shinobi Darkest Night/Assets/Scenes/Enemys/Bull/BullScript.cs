using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullScript : MonoBehaviour
{
    private AILerp ai;
    private EnemyHealth enemyScript;
    private float enemySpeed;
    [SerializeField] float detectRadius;
    [SerializeField] float attackRadius;
    private Transform player;
    private float timebtwAttacks;
    [SerializeField] LayerMask playerLayer;
    private GameObject hit;
    private float offScreenSpeed;
    
    //dash
    private Coroutine dashCorutine;
    private bool canMove;
    private float dashMaxSpeed;
    private float currentSpeed;
    private Rigidbody2D rb;

    private void Awake()
    {
        enemySpeed = GetComponent<AILerp>().speed;
        offScreenSpeed = enemySpeed * 2;
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

        if (GetComponent<Rigidbody2D>())
        { 
            rb = GetComponent<Rigidbody2D>();
        }
        timebtwAttacks = 0;
        canMove = true;
    }
    void Update()
    {
        if (transform.parent.GetComponent<AiBrain>().playerIn && (player.position - transform.position).magnitude < attackRadius && timebtwAttacks < 0.3f && GetComponent<EnemyHealth>().stundTime < 0.1f)
        {
            if (Physics2D.Raycast(transform.position, (player.position - transform.position).normalized, 30, LayerMask.GetMask("Player")))
            {
                hit = Physics2D.Raycast(transform.position, -transform.position + player.position, 100, playerLayer).transform.gameObject;
                if (hit.layer == player.gameObject.layer && dashCorutine == null)
                {
                    dashCorutine =  StartCoroutine(Dash());
                    timebtwAttacks = 6;
                }
            }

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
                    StartCoroutine(ResetPathf());
                }
            }
        }
        else
        {
            ai.canMove = false;
        }
    }

    private void FixedUpdate()
    {
        if ( transform.parent.GetComponent<AiBrain>().playerIn && (player.position - transform.position).magnitude > detectRadius)
        {
            ai.enabled = false;
        }
        else
        {
            ai.enabled = true;
        }
        if (Camera.main.WorldToScreenPoint(transform.position).x > 0 && Camera.main.WorldToScreenPoint(transform.position).x < Screen.width && Camera.main.WorldToScreenPoint(transform.position).y > 0 && Camera.main.WorldToScreenPoint(transform.position).y < Screen.height)
        {
            ai.speed = enemySpeed;
        }
        else
        {
            ai.speed = offScreenSpeed;
        }
    }

    private IEnumerator Dash()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        Debug.DrawLine(transform.position,(transform.position + dir *3) , Color.red, 5);
        canMove = false;
        float i = 1;
        while (i > 0)
        {
            i -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        canMove = true;
        dashCorutine = null;
    }


    private IEnumerator ResetPathf()
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
