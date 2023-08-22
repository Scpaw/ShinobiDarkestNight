using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniOkubi : MonoBehaviour
{
    private AILerp ai;
    private EnemyHealth enemyScript;
    private float enemySpeed;
    public float detectRadius;
    private Transform player;
    private Animator anim;
    private Vector2 startPos;
    private float scaleX;

    //atak wlosami
    private bool attacking;
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
        enemySpeed = ai.speed;
        player = PlayerController.Instance.GetPlayer().transform;
        GetComponent<AIDestinationSetter>().target = player;
    }
    void Update()
    {
        if (anim == null)
        { 
            anim = transform.Find("Grafika").GetComponent<Animator>();
            GetComponent<EnemyDamage>().attackAnim = true;
            scaleX = anim.transform.localScale.x;
            Debug.Log(anim.transform.localScale.x);
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
        attacking = GetComponent<EnemyDamage>().isAttacking;
        attackNum = GetComponent<EnemyDamage>().attacksInt;

        if (attackNum > Random.Range(3, 6))
        {
            Debug.Log("attack now");
            anim.SetTrigger("HairAttack");
            GetComponent<EnemyDamage>().attackAnim = false;
            GetComponent<EnemyDamage>().attacksInt = 0;
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
