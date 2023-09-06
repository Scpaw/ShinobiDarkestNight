using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ronin_AI : MonoBehaviour
{
    private AILerp ai;
    private EnemyHealth enemyScript;
    private float enemySpeed;
    public float detectRadius;
    private Transform player;
    private DamageRange damageRange;
    private float offScreenSpeed;
    [Header("Animations")]
    private Animator anim;
    [SerializeField] float enemyAttackTime;
    private Dash dash;
    private int dashState;
    public bool canMove;
    public bool attacking;

    private void Awake()
    {
        enemySpeed = GetComponent<AILerp>().speed;
        offScreenSpeed = enemySpeed * 2;
        anim = GetComponentInChildren<Animator>();
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
            damageRange = GetComponentInChildren<DamageRange>();
        }
        if (dash == null)
        {
            dash = GetComponent<Dash>();
            dash.stopAtPlayer = true;
            dash.dontAttackWhileDashing = true;
        }
        enemyScript.canDeflect = 3;
    }
    void Update()
    {
        if (transform.parent.GetComponent<AiBrain>().playerIn && (player.position - transform.position).magnitude < detectRadius)
        {
            if (enemyScript.stundTime > 0 || !dash.canMove || !canMove)
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

        if (enemyScript.deflectAgain > 0)
        {
            enemyScript.deflectAgain -= Time.deltaTime;
        }
        else
        {
            if (enemyScript.canDeflect <= 0)
            {
                enemyScript.canDeflect = 3;
            }
        }

        if ((player.position.x - transform.position.x > 0 && transform.localScale.x < 0 || player.position.x - transform.position.x < 0 && transform.localScale.x > 0) && dashState!=2)
        {
            Flip(transform);
            Flip(transform.GetComponentInChildren<Canvas>().transform);
        }

        //attack
        Attack();
    }

    private void FixedUpdate()
    {
        if (damageRange.playerInRange || transform.parent.GetComponent<AiBrain>().playerIn && (player.position - transform.position).magnitude > detectRadius)
        {
            ai.enabled = false;
        }
        else
        {
            if (!ai.enabled)
            {
                StartCoroutine(ResetPathf());
                ai.enabled = true;
            }

        }
        if (Camera.main.WorldToScreenPoint(transform.position).x > 0 && Camera.main.WorldToScreenPoint(transform.position).x < Screen.width && Camera.main.WorldToScreenPoint(transform.position).y > 0 && Camera.main.WorldToScreenPoint(transform.position).y < Screen.height)
        {
            ai.speed = enemySpeed;
        }
        else
        {
            ai.speed = offScreenSpeed;
        }

        if (ai.speed > 0 && ai.enabled)
        {
            anim.SetFloat("Blend", 1);
        }
        else
        {
            anim.SetFloat("Blend", 0);
        }
    }

    private void Attack()
    {
        if (enemyAttackTime < Time.time)
        {
            if (damageRange.playerInRange)
            {
                anim.SetTrigger("Slash");
            }
            else
            {
                dash.StartDash();
            }

            enemyAttackTime = Time.time + Random.Range(4f,6f);
        }

        if ( dash.dashState != dashState)
        {
            if (dash.dashState == 0)
            {
                dashState = dash.dashState;
                return;
            }
            dashState = dash.dashState;
        }
        anim.SetInteger("Dash", dashState);
    }

    public void DoDmg(int dmg)
    {
        if (damageRange.playerInRange)
        {
            player.GetComponent<PlayerHealth>().AddDamage(dmg);
        }
    }

    private void Flip(Transform changeThis)
    {
        changeThis.localScale = new Vector3(-changeThis.localScale.x, changeThis.localScale.y, changeThis.localScale.z);
    }

    public IEnumerator ResetPathf()
    {
        if (dash.canMove)
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
