using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hideObsticle : MonoBehaviour
{
    private GameObject player;
    private GameObject playerP;
    private float playerT;
    private SpriteRenderer playerSprite;
    private SpriteRenderer sprite;
    private Coroutine changeCorutine;
    private float O;
    private int somethinIn;
    private void Start()
    {
        playerP = PlayerController.Instance.GetPlayer();
        playerSprite = playerP.transform.Find("Grafika").GetComponent<SpriteRenderer>();
        player = PlayerController.Instance.GetPlayer();
        if (GetComponent<SpriteRenderer>())
        {
            sprite = GetComponent<SpriteRenderer>();
        }
        else
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
        }


        O = gameObject.transform.position.y;
        playerT = playerP.transform.position.y;
    }

    private void Update()
    {
        O = gameObject.transform.position.y;
        playerT = playerP.transform.position.y;

        if(gameObject.tag == "Obstacle"  && (playerP.transform.position - transform.position).magnitude < 3)
        {
            if (GetComponent<DropCandy>())
            {
                GetComponent<DropCandy>().Drop();
            }
            if (playerT > O && somethinIn > 0)
            {
                sprite.sortingOrder = playerSprite.sortingOrder + 1;
            }
            else if (O > playerT && somethinIn <=0)
            {
                sprite.sortingOrder = playerSprite.sortingOrder - 1;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player || collision.gameObject.layer == 6)
        {
            if (somethinIn == 0)
            {
                if (changeCorutine != null)
                {
                    StopCoroutine(changeCorutine);
                    changeCorutine = StartCoroutine(Change(0.6f));
                }
                else
                {
                    changeCorutine = StartCoroutine(Change(0.6f));
                }
            }
            somethinIn++;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player || collision.gameObject.layer == 6)
        {
            somethinIn--;
            if (somethinIn == 0)
            {
                if (changeCorutine != null)
                {
                    StopCoroutine(changeCorutine);
                    changeCorutine = StartCoroutine(Change(1));
                }
                else
                {
                    changeCorutine = StartCoroutine(Change(1));
                }
            }
        }

    }

    IEnumerator Change(float changeTo)
    {
        float alpha = sprite.color.a;
        if (sprite.color.a > changeTo)
        {
            while (sprite.color.a > changeTo)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha -=2*Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }
        else if (sprite.color.a < changeTo)
        {
            while (sprite.color.a < changeTo)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha += 2*Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
