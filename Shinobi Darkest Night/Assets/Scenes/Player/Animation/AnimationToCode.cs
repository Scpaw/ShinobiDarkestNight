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

    public void TimeToDie()
    {
        SceneManager.LoadScene(0);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && collision.GetComponent<EnemyHealth>())
        {
            collision.GetComponent<EnemyHealth>().enemyAddDamage(1, false, true);
            StartCoroutine(collision.GetComponent<EnemyHealth>().Stuned());
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.parent.GetComponent<PlayerController>().point2 * 1, ForceMode2D.Impulse);
        }
    }
}
