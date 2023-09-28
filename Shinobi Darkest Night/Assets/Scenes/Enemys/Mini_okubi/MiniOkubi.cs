using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniOkubi : MonoBehaviour
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

    //atak wlosami
    private int attackNum;

    private void Awake()
    {
        startPos = transform.position;
    }
    private void OnEnable()
    {
        transform.position = startPos;
        ai = GetComponent<AIPath>();
        enemyScript = GetComponent<EnemyHealth>();
        enemyScript.deflectAtRandom = true;
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

        int x = (int)((enemyScript.enemyHealth / enemyScript.enemyMaxHealth) *10-4);
        if (x <= 0)
        {
            if (enemyAddSpeed == 0)
            {
                enemyAddSpeed = 1.3f;
                ai_Move.enemySpeed = enemySpeed + enemyAddSpeed;
                ai.maxSpeed = enemySpeed + enemyAddSpeed;
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
}
