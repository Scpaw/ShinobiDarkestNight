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
        transform.GetComponentInParent<NewAi>().canMove = false;
        transform.GetComponentInParent<Rigidbody2D>().velocity = Vector3.zero;
        transform.GetComponentInParent<Spear>().Attack();
    }
}
