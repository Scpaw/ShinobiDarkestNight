using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_AnimToCode : MonoBehaviour
{
    private Archer archerScript;
    void Awake()
    {
        archerScript = transform.parent.GetComponent<Archer>();
    }


    public void Shoot()
    {
        archerScript.SpawnPoint(false);
    }

    public void Shoot3()
    {
        archerScript.SpawnPoint(true);
    }
    public void Attack()
    {
        archerScript.Attack();
    }
}
