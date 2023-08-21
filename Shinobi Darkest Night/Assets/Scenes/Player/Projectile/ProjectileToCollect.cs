using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileToCollect : MonoBehaviour
{
    public bool onEnemy;
    public float pickUpSpeed;
    private Vector2 startpos;
    private Vector2 player;

    private void Start()
    {
        player = PlayerController.Instance.GetPlayer().transform.position;
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
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (onEnemy && GetComponent<Rigidbody2D>().velocity.magnitude < 0.5f && transform.parent == null)
        { 
            onEnemy = false;
        }
        if (collision.gameObject.tag == "Player" && !onEnemy)
        {
            transform.position = Vector2.MoveTowards(transform.position, collision.transform.position,Time.deltaTime * pickUpSpeed);
            if ((transform.position - collision.transform.position).magnitude < 0.1f)
            {
                collision.GetComponent<PlayerController>().projectileNumber += 1;
                Destroy(gameObject);
            }
        }
    }

}
