using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Move : MonoBehaviour
{
    private AIPath ai;
    private EnemyHealth enemyScript;
    public float enemySpeed;
    public float detectRadius;
    private Transform player;
    private DamageRange damageRange;
    private float offScreenSpeed;
    private AiBrain room;
    private Dash dash;

    public bool moving;
    public bool canMove;

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
            damageRange = GetComponentInChildren<DamageRange>();
        }
        if (room == null)
        {
            room = transform.parent.GetComponent<AiBrain>();
        }
        if (GetComponent<Dash>())
        { 
            dash = GetComponent<Dash>();
        }
        ai.maxSpeed = enemySpeed;
        canMove = true;
    }
    void Update()
    {
        if(room.playerIn && (player.position - transform.position).magnitude < detectRadius)
        {
            if (enemyScript.stundTime > 0 || dash != null && !dash.canMove)
            {
                moving = false;
            }
            else
            {
                if (!moving)
                {
                    StartCoroutine(ResetPathf());
                }
            }
        }
        else
        {
            moving = false;
        }

        if (!canMove)
        {
            ai.canMove = false;
            moving = false;
        }
        else if (moving != ai.canMove)
        {
            ai.canMove = moving;
        }

    }
    private void FixedUpdate()
    {
        if (damageRange.playerInRange || room.playerIn && (player.position - transform.position).magnitude > detectRadius)
        {
            moving = false;
        }
        else
        {
            moving = true;
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
    public IEnumerator ResetPathf()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        ai.maxSpeed = 0;
        while (enemySpeed > ai.maxSpeed)
        {
            if (ai.maxSpeed > 0.1f)
            {
                moving = true;
            }
            ai.maxSpeed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
