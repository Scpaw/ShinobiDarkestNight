using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Archer_AI : MonoBehaviour
{
    private AILerp ai;
    private Archer archerScript;
    private float archerSpeed;
    public float detectRadius;
    private Transform player;

    private void Start()
    {
        ai = GetComponent<AILerp>();
        archerScript = GetComponent<Archer>();
        archerSpeed = ai.speed;
        player = PlayerController.Instance.GetPlayer().transform;
    }
    void Update()
    {
        if (transform.parent.GetComponent<AiBrain>().playerIn && (player.position - transform.position).magnitude < detectRadius)
        {
            if (archerScript.stundTime > 0)
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
    private void FixedUpdate()
    {
        if (transform.parent.GetComponent<AiBrain>().playerIn && (player.position - transform.position).magnitude > detectRadius)
        {
            ai.enabled = false;
        }
        else
        {
            ai.enabled = true;
        }
    }
    private IEnumerator ResetPathf()
    {
        ai.SetNewPath();
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        ai.speed = 0;
        while (archerSpeed > ai.speed)
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
