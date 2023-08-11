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

    //atak wlosami
    private bool attacking;
    private int attackNum;
    
    private void Start()
    {
        ai = GetComponent<AILerp>();
        enemyScript = GetComponent<EnemyHealth>();
        enemySpeed = ai.speed;
        player = PlayerController.Instance.GetPlayer().transform;
    }
    void Update()
    {
        if (anim == null)
        { 
            anim = transform.FindChild("Grafika").GetComponent<Animator>();
            GetComponent<EnemyDamage>().attackAnim = true;
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
