using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileToCollect : MonoBehaviour
{
    public bool onEnemy;
    public float pickUpSpeed;
    private Vector2 startpos;
    private Transform player;
    public float pickUpRange;

    private void Start()
    {
        player = PlayerController.Instance.GetPlayer().transform;
    }
    private void FixedUpdate()
    {
        if (startpos == Vector2.zero)
        {
            startpos = transform.localPosition;
        }
        if (transform.parent && onEnemy)
        {
            transform.position = new Vector2(transform.parent.position.x + startpos.x, transform.parent.position.y + startpos.y);
            if (transform.GetChild(0).gameObject.activeInHierarchy)
            {
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        else
        {
            if (!transform.GetChild(0).gameObject.activeInHierarchy)
            {
                transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        if ((player.position - transform.position).magnitude <= pickUpRange && !onEnemy)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, Time.deltaTime * pickUpSpeed);
            if ((player.position - transform.position).magnitude < 0.1f)
            {
                player.GetComponent<PlayerController>().projectileNumber += 1;
                Destroy(gameObject);
            }
        }
        if (onEnemy && GetComponent<Rigidbody2D>().velocity.magnitude < 0.5f && transform.parent == null)
        {
            onEnemy = false;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

}
