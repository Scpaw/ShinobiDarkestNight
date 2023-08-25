using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniOkubi : MonoBehaviour
{
    private AILerp ai;
    private EnemyHealth enemyScript;
    private EnemyDamage damage;
    private float enemySpeed;
    public float detectRadius;
    private Transform player;
    private Animator anim;
    private Vector2 startPos;
    private float scaleX;
    private float enemyAddSpeed;

    //atak wlosami
    private int attackNum;

    private void Awake()
    {
        startPos = transform.position;
    }
    private void OnEnable()
    {
        transform.position = startPos;
        ai = GetComponent<AILerp>();
        enemyScript = GetComponent<EnemyHealth>();
        enemyScript.deflectAtRandom = true;
        player = PlayerController.Instance.GetPlayer().transform;
        GetComponent<AIDestinationSetter>().target = player;
        damage = GetComponent<EnemyDamage>();
        enemySpeed = ai.speed;
        enemyAddSpeed = 0;
    }
    private void OnDisable()
    {
        ai.speed -= enemyAddSpeed;
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
        attackNum = damage.attacksInt;

        int x = (int)((enemyScript.enemyHealth / enemyScript.enemyMaxHealth) *10-4);
        if (x <= 0)
        {
            if (enemyAddSpeed == 0)
            {
                enemyAddSpeed = 2;
                ai.speed = enemySpeed + enemyAddSpeed;
            }
        }
        if (attackNum > Random.Range(3, 2 * x))
        {
            anim.SetTrigger("HairAttack");
            damage.attackAnim = false;
            damage.attacksInt = 0;
            damage.canAttack = false;

        }
        if (player.position.x > transform.position.x)
        {
            anim.transform.localScale = new Vector3(-scaleX, anim.transform.localScale.y, anim.transform.localScale.z);
        }
        else
        {
            anim.transform.localScale = new Vector3(scaleX, anim.transform.localScale.y, anim.transform.localScale.z);
        }
    }

    private IEnumerator ResetPathf()
    {
        ai.SetNewPath();
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        ai.speed = 0;
        while (enemySpeed + enemyAddSpeed > ai.speed)
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
