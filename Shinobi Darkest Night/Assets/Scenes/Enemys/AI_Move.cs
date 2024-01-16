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
    public AiBrain room;
    private Dash dash;

    public bool moving;
    public bool canMove;
    public bool stop;

    private Coroutine reseting;
    public Coroutine hit;

    private void Awake()
    {
        enemySpeed = GetComponent<AIPath>().maxSpeed;
        offScreenSpeed = enemySpeed * 2;
        reseting = null;
        hit = null;
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
        if(room.playerIn && (player.position - transform.position).magnitude < detectRadius && !stop)
        {
            if (enemyScript.stundTime > 0 || dash != null && !dash.canMove)
            {
                moving = false;
            }
            else
            {
                if (!moving && reseting == null && hit == null)
                {
                    reseting = StartCoroutine(ResetPathf());
                }
            }
        }

        if (!canMove || stop)
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
        else if(room.playerIn)
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

    public void Hit(float timeToWait,Vector2 pushForce, float dmg)
    {
        if (hit != null)
        {
            StopCoroutine(hit);
        }
        hit = StartCoroutine(HitAndPush(timeToWait, pushForce,dmg));
    }

    private IEnumerator HitAndPush(float timeToWait, Vector2 pushForce,float dmg)
    {
        canMove = false;
        yield return new WaitForSeconds(timeToWait);
        GetComponent<EnemyHealth>().enemyAddDamage(dmg, false, true);
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<Rigidbody2D>().AddForce(pushForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        canMove = true;
        hit = null;
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
        reseting = null;
    }

    public bool IsMoving()
    {
        return (enemyScript.stundTime <= 0 && moving);
    }
}
