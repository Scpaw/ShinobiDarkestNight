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
                    ai.canMove = true;
                }
            }
        }
        else
        {
            ai.canMove = false;
        }

    }

    private IEnumerator ResetPathf()
    {
        ai.speed = 0;
        while (enemySpeed > ai.speed)
        {
            ai.speed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("NewPathf");
    }
}
