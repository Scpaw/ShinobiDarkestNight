using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpPoint : MonoBehaviour
{
    [SerializeField] private float minHp;
    [SerializeField] private float maxHp;
    private Transform player;
    [SerializeField] private float speed;
    [SerializeField] private float startForce;
    private Rigidbody2D rb;
    private float waitTime;


    void Start()
    {
        player = PlayerStateMachine.Instance.transform;
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(Random.insideUnitCircle * startForce, ForceMode2D.Impulse);
        waitTime = 0.3f;
        StartCoroutine(Wait());
    }

    private void Heal()
    {
        player.GetComponent<PlayerHealth>().AddHealth(Random.Range(minHp, maxHp));
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == player.gameObject && waitTime == 0)
        {
            if (rb.velocity.magnitude > 0)
            {
                rb.velocity = Vector2.zero;
            }
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

            if ((transform.position - player.position).magnitude < 0.1f)
            {
                Heal();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player.gameObject && waitTime == 0)
        {
            rb.AddForce((player.position - transform.position) * speed/5, ForceMode2D.Impulse);
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        waitTime = 0;
    }
}
