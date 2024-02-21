using Pathfinding;
using Pathfinding.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Spear : MonoBehaviour
{
    private EnemyHealth enemyScript;
    private float enemySpeed;
    [SerializeField] float attackRadius;
    private float attackRadiusToUse;
    private Transform player;
    private Vector3 attackPoint;
    private float timebtwAttacks;
    private DamageRange damageRange;
    [SerializeField]private float dmg;
    private bool hitPlayer;
    [SerializeField] LayerMask playerLayer;
    private GameObject hit;
    private float offScreenSpeed;
    private Dash dash;
    //private AI_Move ai_Move;
    private NewAi AI;
    private int changeDashState;
    private Animator anim;
    [SerializeField] private float detectRadius;
    private bool moving;

    //attack
    [SerializeField] float attackTime;
    private float timeToAttack;

    private void Awake()
    {
        enemySpeed = GetComponent<AIPath>().maxSpeed;
        offScreenSpeed = enemySpeed * 2;
    }

    private void OnEnable()
    {
        if (enemyScript == null)
        {
            enemyScript = GetComponent<EnemyHealth>();
        }
        if (player == null)
        {
            player = PlayerStateMachine.Instance.transform;
        }

        if (transform.GetComponent<AIDestinationSetter>().target == null)
        {
            transform.GetComponent<AIDestinationSetter>().target = player;
        }
        if (damageRange == null)
        {
            damageRange =  GetComponentInChildren<DamageRange>();
        }
        if (dash == null)
        { 
            dash = GetComponent<Dash>();
        }
        if (anim == null)
        { 
            anim = GetComponentInChildren<Animator>();
        }
        if (AI == null)
        {
            AI = GetComponent<NewAi>();
        }
        timebtwAttacks = 0;
        hitPlayer = false;
        //attackRadiusToUse = attackRadius + AI.detectRadius/2;
        attackRadiusToUse = attackRadius + detectRadius / 2;

        AI.attackRate = attackTime;
    }
    void Update()
    {
        if (transform.parent.GetComponent<AiBrain>().playerIn && (player.position - transform.position).magnitude < attackRadiusToUse && timebtwAttacks < 0.3f && GetComponent<EnemyHealth>().stundTime < 0.1f && !damageRange.playerInRange)
        {
            if (Physics2D.Raycast(transform.position, (player.position - transform.position).normalized, 30, LayerMask.GetMask("Player")))
            {
                hit = Physics2D.Raycast(transform.position, -transform.position + player.position, 100, playerLayer).transform.gameObject;
                if (hit.layer == player.gameObject.layer)
                {
                    dash.StartDash();
                    timebtwAttacks = 12;
                }
            }
        }
        else if (damageRange.playerInRange && changeDashState == 0)
        {
            Attack();
        }
        if (timebtwAttacks > 0)
        { 
            timebtwAttacks -= Time.deltaTime;
            if (dash.timebtwAttacks == 0)
            {
                timebtwAttacks = 0;
            }
        }

        if (dash.dashState != changeDashState)
        {
            changeDashState = dash.dashState;
            anim.SetInteger("DashState", changeDashState);
        }

        if (player.transform.position.x - transform.position.x > 0 && transform.localScale.x < 0 || player.transform.position.x - transform.position.x < 0 && transform.localScale.x > 0)
        {
            Flip(transform);
            Flip(transform.GetComponentInChildren<Canvas>().transform);
        }

        //Debug.Log(ai_Move.moving);
        if (AI.IsMoving())
        {
            anim.SetFloat("Blend", 1);
        }
        else
        {
            anim.SetFloat("Blend", 0);
        }

    }
    private void Flip(Transform changeThis)
    {
        changeThis.localScale = new Vector3(-changeThis.localScale.x, changeThis.localScale.y, changeThis.localScale.z);
    }

    public void Attack()
    {
        if (damageRange.playerInRange && AI.attack)
        {
            player.GetComponent<PlayerHealth>().AddDamage(dmg);
            AI.EndAttack();
        }
    }

    private void FixedUpdate()
    {
        if (damageRange.playerInRange || transform.parent.GetComponent<AiBrain>().playerIn && (player.position - transform.position).magnitude > detectRadius)
        {
            AI.canMove = false;
        }
        else
        {
            AI.canMove = true;
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
}
