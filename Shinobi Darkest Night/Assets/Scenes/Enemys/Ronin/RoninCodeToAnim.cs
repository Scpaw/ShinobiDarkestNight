using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoninCodeToAnim : MonoBehaviour
{
    private Ronin_AI ronin;
    private void Start()
    { 
        ronin = transform.parent.GetComponent<Ronin_AI>();
    }
    public void MeleeAttack()
    { 
    
    }

    public void DashAttack()
    {
        //transform.parent.GetComponent<AILerp>().enabled = false;
    }
}
