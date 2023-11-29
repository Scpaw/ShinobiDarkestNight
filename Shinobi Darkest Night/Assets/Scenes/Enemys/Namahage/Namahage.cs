using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Namahage : MonoBehaviour
{
    private AIPath ai;
    private EnemyHealth enemyScript;
    private EnemyDamage damage;
    private float enemySpeed;
    private Transform player;
    private Animator anim;
    private Vector2 startPos;
    private float scaleX;
    private float enemyAddSpeed;
    private AI_Move ai_Move;
    private int attackNum;
    private float bucketTimer;

    [Header("Bucket")]
    public GameObject projectile;
    public float power;

    [Header("Run stab")]
    [SerializeField] private float startRunDistance;
    public bool runing;
    [SerializeField] private float jumpDistance;
    public bool jumping;

    private void Awake()
    {
        startPos = transform.position;
    }
    private void OnEnable()
    {
        transform.position = startPos;
        ai = GetComponent<AIPath>();
        enemyScript = GetComponent<EnemyHealth>();
        player = PlayerController.Instance.GetPlayer().transform;
        GetComponent<AIDestinationSetter>().target = player;
        damage = GetComponent<EnemyDamage>();
        ai_Move = GetComponent<AI_Move>();
        enemySpeed = ai.maxSpeed;
        enemyAddSpeed = 0;
    }
    private void OnDisable()
    {
        ai.maxSpeed -= enemyAddSpeed;
        ai_Move.enemySpeed -= enemyAddSpeed;
        enemyAddSpeed = 0;
    }
    void Update()
    {

        if (anim == null)
        {
            anim = transform.Find("Grafika").GetComponent<Animator>();
            damage.attackAnim = true;
            scaleX = anim.transform.localScale.x;
        }
        attackNum = damage.attacksInt;

        int x = (int)((enemyScript.enemyHealth / enemyScript.enemyMaxHealth) * 10 - 4);
        if (x <= 0)
        {
            if (enemyAddSpeed == 0)
            {
                enemyAddSpeed = 1.3f;
                ai_Move.enemySpeed = enemySpeed + enemyAddSpeed;
                ai.maxSpeed = enemySpeed + enemyAddSpeed;
            }
        }
        if (0 > Random.Range(1 - attackNum, x - attackNum) && !damage.playerIn && bucketTimer < Time.time && !runing && !jumping)
        {
            anim.SetTrigger("Bucket");
            damage.attacksInt = 0;
            damage.canAttack = false;
            ai_Move.canMove = false;
            bucketTimer = Time.time + Random.Range(0.75f, 2.2f);
        }
        if (player.position.x > transform.position.x)
        {
            anim.transform.localScale = new Vector3(-scaleX, anim.transform.localScale.y, anim.transform.localScale.z);
        }
        else
        {
            anim.transform.localScale = new Vector3(scaleX, anim.transform.localScale.y, anim.transform.localScale.z);
        }
        if (ai_Move.IsMoving() && ai.enabled )
        {
            anim.SetFloat("Moving", 1);
        }
        else
        {
            anim.SetFloat("Moving", 0);
        }


        //runing
        if ((player.position - transform.position).magnitude > startRunDistance && !runing)
        {
            runing = true;
            Debug.Log("run now");
        }
        else if (runing && ((player.position - transform.position).magnitude < jumpDistance + 0.3f && !jumping))
        {
            runing = false;
            jumping = true;
            Debug.Log("jump");
        }

        if (!runing && !jumping && !damage.canAttack)
        {
            damage.canAttack = true;
        }
        else if ((runing || jumping) && damage.canAttack)
        {
            damage.canAttack = false;
        }
    }


    public void AfterAtttack()
    {
        damage.canAttack = true;
        ai_Move.canMove = true;
    }
}
