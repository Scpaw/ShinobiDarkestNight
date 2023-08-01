using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationToCode : MonoBehaviour
{
    public void Shoot()
    {
        if (transform.parent.GetComponent<PlayerController>() != null)
        {
            transform.parent.GetComponent<PlayerController>().SpawnPoint();
        }
    }

    public void StartHeal()
    {
        transform.parent.GetComponent<PlayerController>().canHeal = true;
    }
}
