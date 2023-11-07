using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class activateEnemy : MonoBehaviour
{
    public List<AI_Move> whatToActivate;

    [Tooltip("check what graph NUMBER in AstarCore is on this")]
    public int graphNumber;

    public void Activate()
    {
        foreach (AI_Move enemy in whatToActivate)
        {
            enemy.stop = false;
        }
        AstarPath.active.graphs[graphNumber-1].Scan();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        { 
            Activate();
        }
    }
}
