using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AnimationToCode : MonoBehaviour
{
    [SerializeField] private Coroutine slowing;
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

    public void DesumiruAttack()
    {
        transform.parent.GetComponent<PlayerController>().DesumiruAttack();
    }

    public void SlowTime()
    {
        if (transform.parent.GetComponent<PlayerController>().desumiruState < 4)
        {
            if (slowing != null)
            {
                StopCoroutine(slowing);
                slowing = StartCoroutine(transform.parent.GetComponent<PlayerController>().SlowTime());
            }
            else
            {
                slowing = StartCoroutine(transform.parent.GetComponent<PlayerController>().SlowTime());
            }
        }

    }

    public void StartShokyaku()
    {
        transform.parent.GetComponent<PlayerController>().shokyaku = true;
    }

    public void TimeToDie()
    {
        SceneManager.LoadScene(0);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && collision.GetComponent<EnemyHealth>())
        {
            collision.GetComponent<EnemyHealth>().enemyAddDamage(20, true, true);
            StartCoroutine(collision.GetComponent<EnemyHealth>().Stuned());
            if (transform.parent.GetComponent<PlayerController>().desumiruState == 2)
            {
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.parent.GetComponent<PlayerController>().point2 * 1, ForceMode2D.Impulse);
            }
            else if (transform.parent.GetComponent<PlayerController>().desumiruState == 3)
            {
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.parent.GetComponent<PlayerController>().point2 * 2, ForceMode2D.Impulse);
            }

        }
    }
}
