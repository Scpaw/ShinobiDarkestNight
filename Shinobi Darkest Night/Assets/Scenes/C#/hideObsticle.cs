using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hideObsticle : MonoBehaviour
{
    private GameObject player;
    private SpriteRenderer sprite;
    private Coroutine changeCorutine;
    private int somethinIn;
    private void Start()
    {
        player = PlayerController.Instance.GetPlayer();
        sprite = GetComponent<SpriteRenderer>();
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
