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
    private EnemyDamage damageRange;
    private float dmg;
    private bool hitPlayer;

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
    }
    void Update()
    {
        if (transform.parent.GetComponent<AiBrain>().playerIn && (player.position - transform.position).magnitude < attackRadius && timebtwAttacks < 0.3f &&GetComponent<EnemyHealth>().stundTime <0.1f)
        {
            StartCoroutine(SpearDash());
            timebtwAttacks = 5;
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
            Debug.Log("Collision");
            player.GetComponent<PlayerHealth>().AddDamage(dmg * 1.5f);
            hitPlayer = true;
        }
    }
    private IEnumerator SpearDash()
    {
        canMove = false;
        damageRange.enabled = false;
        float i = 1;
        ai.canMove = false;
        attackPoint = player.position + ((player.position - transform.position).normalized * 2);
        GetComponent<Rigidbody2D>().AddForce((attackPoint - transform.position) * 3, ForceMode2D.Impulse);
        while (i > 0)
        {
            i -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
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
}
