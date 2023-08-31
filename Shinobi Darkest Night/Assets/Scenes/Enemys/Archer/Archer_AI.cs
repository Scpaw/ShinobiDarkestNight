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
    private float enemySpeed;
    private float offScreenSpeed;
    private void Awake()
    {
        enemySpeed = GetComponent<AILerp>().speed;
        offScreenSpeed = enemySpeed * 2;
    }
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
        }
        else
        {
            ai.canMove = false;
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
        if (Camera.main.WorldToScreenPoint(transform.position).x > 0 && Camera.main.WorldToScreenPoint(transform.position).x < Screen.width && Camera.main.WorldToScreenPoint(transform.position).y > 0 && Camera.main.WorldToScreenPoint(transform.position).y < Screen.height)
        {
            ai.speed = enemySpeed;
        }
        else
        {
            ai.speed = offScreenSpeed;
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
