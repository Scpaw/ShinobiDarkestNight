using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hideObsticle : MonoBehaviour
{
    private GameObject player;
    private SpriteRenderer sprite;
    private void Start()
    {
        player = PlayerController.Instance.GetPlayer();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        { 
            StopAllCoroutines();
            StartCoroutine(Change(0.6f));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            StopAllCoroutines();
            StartCoroutine(Change(1));
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
