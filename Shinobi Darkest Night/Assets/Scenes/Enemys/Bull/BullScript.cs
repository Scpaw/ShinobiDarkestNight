using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullScript : MonoBehaviour
{
    private AIPath ai;
    private EnemyHealth enemyScript;
    private float enemySpeed;
    [SerializeField] float detectRadius;
    [SerializeField] float attackRadius;
    private Transform player;
    private float timebtwAttacks;
    [SerializeField] LayerMask playerLayer;
    private GameObject hit;
    private float offScreenSpeed;
    private DamageRange damageRange;
    
    //dash
    private Coroutine dashCorutine;
    private bool canMove;
    [SerializeField] private float dashMaxSpeed;
    private float currentSpeed;
    private Rigidbody2D rb;
    private bool dashing;
    private bool attack;
    [SerializeField] float dmg;

    private void Awake()
    {
        enemySpeed = GetComponent<AIPath>().maxSpeed;
        offScreenSpeed = enemySpeed * 2;
    }

    private void OnEnable()
    {
        if (ai == null)
        {
            ai = GetComponent<AIPath>();
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
        if (damageRange == null)
        { 
            damageRange = GetComponentInChildren<DamageRange>();
        }
        timebtwAttacks = 0;
        canMove = true;
        enemyScript.canDoDmg = false;

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
                    timebtwAttacks = Random.Range(3, 4);
                }
            }

        }
        if (timebtwAttacks > 0 && canMove)
        {
            timebtwAttacks -= Time.deltaTime;
        }
        if (transform.parent.GetComponent<AiBrain>().playerIn && (player.position - transform.position).magnitude < detectRadius && (player.position - transform.position).magnitude > attackRadius/2)
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
        if ( dashing|| transform.parent.GetComponent<AiBrain>().playerIn && (player.position - transform.position).magnitude > detectRadius && transform.parent.GetComponent<AiBrain>().playerIn)
        {
            ai.enabled = false;
        }
        else
        {
            ai.enabled = true;
        }
        if (Camera.main.WorldToScreenPoint(transform.position).x > 0 && Camera.main.WorldToScreenPoint(transform.position).x < Screen.width && Camera.main.WorldToScreenPoint(transform.position).y > 0 && Camera.main.WorldToScreenPoint(transform.position).y < Screen.height)
        {
            ai.maxSpeed = enemySpeed;
        }
        else
        {
            ai.maxSpeed = offScreenSpeed;
        }

        if (dashing && damageRange.playerInRange && !attack)
        {
            player.GetComponent<PlayerHealth>().AddDamage((currentSpeed/dashMaxSpeed) * dmg);
            attack = true;
        }

        if (transform.parent.GetComponent<RoomBrain>().enemiesActive == 1 && !enemyScript.canDoDmg)
        { 
            enemyScript.canDoDmg = true;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dashing && dashCorutine != null)
        {
            StopCoroutine(dashCorutine);
            rb.velocity = Vector3.zero;
            dashing = false;
            StartCoroutine(Wait(currentSpeed / dashMaxSpeed));
            dashCorutine = null;
        }
    }

    private IEnumerator Dash()
    {
        //start
        dashing = true;
        attack = false;
        currentSpeed = 0;
        Vector3 dir = (player.position - transform.position).normalized;
        Debug.DrawLine(transform.position,(transform.position + dir *3) , Color.red, 5);
        canMove = false;
        float i = 0.35f;
        while (dashMaxSpeed > currentSpeed)
        {
            rb.AddForce(dir *dashMaxSpeed * Time.deltaTime,ForceMode2D.Impulse);
            yield return new WaitForEndOfFrame();
            currentSpeed = rb.velocity.magnitude;
        }
        //runnning
        while (i > 0)
        { 
            i-=Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //stoping

        while (currentSpeed > 0.1f)
        {
            rb.AddForce(-rb.velocity.normalized *dashMaxSpeed * Time.deltaTime, ForceMode2D.Impulse);
            yield return new WaitForEndOfFrame();
            currentSpeed = rb.velocity.magnitude;
        }
        dashing = false;
        canMove = true;
        dashCorutine = null;
    }

    private IEnumerator Wait(float timeToWait)
    {
        while (timeToWait > 0)
        { 
            timeToWait -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        canMove = true;
    }

    private IEnumerator ResetPathf()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        ai.maxSpeed = 0;
        while (enemySpeed > ai.maxSpeed)
        {
            if (ai.maxSpeed > 0.1f)
            {
                ai.canMove = true;
            }
            ai.maxSpeed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
