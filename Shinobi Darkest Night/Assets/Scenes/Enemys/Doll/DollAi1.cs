using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollAi1 : MonoBehaviour
{
    private AILerp ai;
    private Enemy enemyScript;
    private float enemySpeed;
    public float detectRadius;
    private Transform player;

    private void Start()
    {
        ai = GetComponent<AILerp>();
        enemyScript = GetComponent<Enemy>();
        enemySpeed = ai.speed;
        player = PlayerController.Instance.GetPlayer().transform;
    }
    void Update()
    {
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
            //ai.autoRepath.interval = 0.1f;
        }
        else
        {
            ai.canMove = false;
            //ai.autoRepath.interval = 1;
        }

    }

    private IEnumerator ResetPathf()
    {
        ai.SetNewPath();
        Debug.Log("new path");
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
