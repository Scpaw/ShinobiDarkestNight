using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doll_AI: MonoBehaviour
{
    private AIPath ai;
    private EnemyHealth enemyScript;
    private float enemySpeed;
    public float detectRadius;
    private Transform player;
    private EnemyDamage damageRange;
    private float offScreenSpeed;

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
        if (damageRange == null)
        {
            damageRange = GetComponent<EnemyDamage>();
        }
        ai.maxSpeed = enemySpeed;
    }
    void Update()
    {
        if (transform.parent.GetComponent<AiBrain>().playerIn && (player.position - transform.position).magnitude < detectRadius)
        {
            if (enemyScript.stundTime > 0)
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
        if (damageRange.playerIn ||transform.parent.GetComponent<AiBrain>().playerIn && (player.position - transform.position).magnitude > detectRadius)
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
