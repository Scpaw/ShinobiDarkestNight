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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && collision.GetComponent<EnemyHealth>())
        {
            collision.GetComponent<EnemyHealth>().enemyAddDamage(1, false, true);
            StartCoroutine(collision.GetComponent<EnemyHealth>().Stuned());
        }
    }
}
