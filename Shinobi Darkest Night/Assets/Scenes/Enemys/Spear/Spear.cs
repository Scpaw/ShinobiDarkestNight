using Pathfinding;
using Pathfinding.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    private EnemyDamage damageRange;
    private float dmg;
    private bool hitPlayer;
    [SerializeField] LayerMask playerLayer;
    private GameObject hit;

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
        if (damageRange == null)
        {
            damageRange = GetComponent<EnemyDamage>();
        }
        timebtwAttacks = 0;
        dmg = damageRange.enemyDamage;
        hitPlayer = false;
        canMove = true;
    }
    void Update()
    { 
        if (transform.parent.GetComponent<AiBrain>().playerIn && (player.position - transform.position).magnitude < attackRadius && timebtwAttacks < 0.3f &&GetComponent<EnemyHealth>().stundTime <0.1f)
        {
            if (Physics2D.Raycast(transform.position, (player.position - transform.position).normalized, 30, LayerMask.GetMask("Player")))
            {
                hit = Physics2D.Raycast(transform.position, -transform.position + player.position, 100, playerLayer).transform.gameObject;
                if (hit.layer == player.gameObject.layer)
                {
                    StartCoroutine(SpearDash());
                    timebtwAttacks = 12;
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
        if ((player.position - transform.position).magnitude < 1.1f)
        {
            ai.enabled = false;
        }
        else
        {
            ai.enabled = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player.gameObject && !damageRange.enabled && !hitPlayer)
        {
            player.GetComponent<PlayerHealth>().AddDamage(dmg * 1.5f);
            hitPlayer = true;
        }
    }
    private IEnumerator SpearDash()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 startPos = transform.position;
        canMove = false;
        damageRange.enabled = false;
        float i = 0.4f;
        ai.canMove = false;
   
        while(i > 0)
        {

            if (Random.value < 0.3f )
            {
                transform.position = startPos + Random.insideUnitCircle / 30;
            }

            i -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = startPos;
        hit = Physics2D.Raycast(transform.position, -transform.position + player.position, 100, playerLayer).transform.gameObject;
        if (hit.layer == player.gameObject.layer)
        {
            attackPoint = player.position + ((player.position - transform.position).normalized * 2);
            i = 1;
            rb.AddForce((attackPoint - transform.position) * 3, ForceMode2D.Impulse);
            while (i > 0)
            {
                i -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            Debug.Log("not in range");
            timebtwAttacks = 0;
        }
        canMove = true;
        damageRange.enabled = true;
        hitPlayer = false;
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

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, -transform.position + player.position, Color.red);
    }
}
