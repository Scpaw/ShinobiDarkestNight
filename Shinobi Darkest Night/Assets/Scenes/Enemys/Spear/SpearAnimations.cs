using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearAnimations : MonoBehaviour
{
    public void StartMoving()
    {
        transform.GetComponentInParent<NewAi>().canMove = true;
    }


    public void StopMoving()
    {
        //Debug.Log(transform.GetComponentInParent<Dash>().dashState);
        transform.GetComponentInParent<NewAi>().Stun((1 - GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime) * GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length, Vector2.zero);
        transform.GetComponentInParent<Rigidbody2D>().velocity = Vector3.zero;
        transform.GetComponentInParent<Spear>().Attack();
    }
}
