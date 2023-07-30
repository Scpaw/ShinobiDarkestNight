using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackController : MonoBehaviour
{
    Vector2 attackOffset;

    Collider2D swordCollider;

    private void Start()
    {
        swordCollider = GetComponent<Collider2D>();
        swordCollider.enabled = false;
        attackOffset = transform.position;
    }

    public void AttackRight()
    {
        swordCollider.enabled = true;

        //transform.position = attackOffset;
    }

    public void AttackLeft()
    {
        swordCollider.enabled = true;

        //transform.position = new Vector3(rightAttackOffset.x * -1, rightAttackOffset.x);
    }

    public void StopAttack()
    {
        swordCollider.enabled = true;
    }
}
