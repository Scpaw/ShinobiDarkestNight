using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<EnemyHealth>().enemyAddDamage(damage, true, true);
            if (collision.GetComponent<Rigidbody2D>())
            {
                if (collision.GetComponent<NewAi>())
                {
                    collision.GetComponent<NewAi>().Stun(1.2f, (collision.transform.position - transform.position) * pushForce);
                }
                else
                {

                    collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce((collision.transform.position - transform.position) * pushForce, ForceMode2D.Impulse);
                    collision.gameObject.GetComponent<EnemyHealth>().Stun();
                }

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
