using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Itaiken : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float startForce;
    [SerializeField] float timeAlive;
    [SerializeField] float pushForce;

    private void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(transform.up * startForce, ForceMode2D.Impulse);
        StartCoroutine(TimeToDie(timeAlive));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            collision.gameObject.GetComponent<EnemyHealth>().enemyAddDamage(damage, true, true);
            if (collision.GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Static)
            {
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce((collision.transform.position - transform.position) * pushForce, ForceMode2D.Impulse);
                StartCoroutine(collision.gameObject.GetComponent<EnemyHealth>().Stuned(false));
            }
        }

    }
    

    private IEnumerator TimeToDie(float timeToKill)
    {
        while (timeToKill > 0)
        {
            timeToKill -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
