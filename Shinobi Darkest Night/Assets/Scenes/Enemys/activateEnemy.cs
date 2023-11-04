using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activateEnemy : MonoBehaviour
{
    public List<AI_Move> ai;

    public void Activate()
    { 
        foreach (AI_Move move in ai) 
        {
            move.stop = false;
        }
    }
}
