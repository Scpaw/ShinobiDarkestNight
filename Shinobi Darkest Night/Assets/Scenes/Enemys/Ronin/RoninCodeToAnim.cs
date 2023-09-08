using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoninCodeToAnim : MonoBehaviour
{
    private Ronin_AI ronin;
    private int deflectNum;
    private void Start()
    {
        ronin = transform.parent.GetComponent<Ronin_AI>();
    }
    public void Deflect()
    {
        transform.GetComponent<Animator>().SetInteger("Deflect", 0);
    }
    public void MeleeAttack()
    {
        ronin.DoDmg(15);
    }

    public void DashStart()
    {
        transform.parent.GetComponent<Ronin_AI>().canMove = false;
    }

    public void DashAttack()
    {
        ronin.DoDmg(20);
    }

    public void DashEnd() 
    {
        transform.parent.GetComponent<Ronin_AI>().canMove = true;
    }

    public void StopMoving()
    {
        transform.parent.GetComponent<Ronin_AI>().canMove = false;
    }

    public void StartMoving()
    {
        transform.parent.GetComponent<Ronin_AI>().canMove = true;
    }

    public void SaveDeflect()
    {
        deflectNum = transform.parent.GetComponent<EnemyHealth>().canDeflect;
        if (deflectNum > 0)
        {
            transform.parent.GetComponent<EnemyHealth>().canDeflect = 0;
        }
    }
    public void ApplyDeflect()
    {
        if (deflectNum > 0)
        {
            transform.parent.GetComponent<EnemyHealth>().canDeflect = deflectNum;
        }
    }
}
