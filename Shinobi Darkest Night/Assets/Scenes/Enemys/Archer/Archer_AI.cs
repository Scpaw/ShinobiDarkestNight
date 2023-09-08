using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Archer_AI : MonoBehaviour
{
    private AIPath ai;
    private Archer archerScript;
    private float archerSpeed;
    public float detectRadius;
    private Transform player;
    private float offScreenSpeed;
    private void Start()
    {
        ai = GetComponent<AIPath>();
        archerScript = GetComponent<Archer>();
        player = PlayerController.Instance.GetPlayer().transform;
        archerSpeed= ai.maxSpeed;
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
    }
    private IEnumerator ResetPathf()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        ai.maxSpeed = 0;
        while (archerSpeed > ai.maxSpeed)
        {
            if (ai.maxSpeed > 0.1f)
            {
                ai.canMove = true;
            }
            ai.maxSpeed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
