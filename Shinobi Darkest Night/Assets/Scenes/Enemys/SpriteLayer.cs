using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLayer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer playerRenderer;
    private bool playerIN;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = transform.parent.GetComponentInChildren<SpriteRenderer>();
        playerRenderer = PlayerController.Instance.transform.GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerIN && playerRenderer.sortingOrder > spriteRenderer.sortingOrder)
        {
            spriteRenderer.sortingOrder = playerRenderer.sortingOrder + 1;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a / 1.4f);
        }
        else if (!playerIN && playerRenderer.sortingOrder < spriteRenderer.sortingOrder)
        {
            spriteRenderer.sortingOrder = playerRenderer.sortingOrder - 1;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a * 1.4f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        { 
            playerIN = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerIN = false;
        }
    }
}
