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
    [Tooltip("variation of the bullet's firing angle")]
    public float projectileVariation;
    public float projectileDmg;

    [Header("Run stab")]
    [SerializeField] private float startRunDistance;
    public bool runing;
    [SerializeField] private float jumpDistance;
    public bool jumping;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpingSpeed;
    public float jumpStabDmg;

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
        if ((player.position - transform.position).magnitude > startRunDistance && !runing && (player.position - transform.position).magnitude < ai_Move.detectRadius)
        {
            runing = true;
            ai_Move.enemySpeed += runSpeed;
            ai.maxSpeed += runSpeed;
        }
        else if ((runing && ((player.position - transform.position).magnitude < jumpDistance + 0.3f && !jumping)))
        {
            runing = false;
            jumping = true;
            StartCoroutine(JumpStab());
            ai_Move.enemySpeed -= runSpeed;
            ai.maxSpeed -= runSpeed;
        }
        else if (runing && (player.position - transform.position).magnitude > ai_Move.detectRadius)
        {
            runing = false;
            ai_Move.enemySpeed -= runSpeed;
            ai.maxSpeed -= runSpeed;
        }

        if (!runing && !jumping && !damage.canAttack)
        {
            damage.canAttack = true;
        }
        else if ((runing || jumping) && damage.canAttack)
        {
            damage.canAttack = false;
        }

        if (anim.GetBool("Jump") != jumping)
        {
            anim.SetBool("Jump", jumping);
        }
        if (anim.GetBool("Run") != runing)
        {
            anim.SetBool("Run", runing);
        }
    }


    public void AfterAtttack()
    {
        damage.canAttack = true;
        ai_Move.canMove = true;
    }

    private IEnumerator JumpStab()
    {
        ai_Move.canMove = false;
        float timer = 2;
        Vector3 pos = player.position;
        while (timer > 0 && (pos - transform.position).magnitude > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, pos, jumpingSpeed/100);
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        ai_Move.canMove = true;
    }
}
