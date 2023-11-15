using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToFind : MonoBehaviour
{
    public Find stats;
    private Transform canvas;
    private Transform player;

    private void Start()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = stats.sprite;
        canvas = GetComponentInChildren<Canvas>().transform;
        player = PlayerController.Instance.transform;
        canvas.gameObject.SetActive(false);
    }
    public void AddToInventory()
    {
        PlayerController.Instance.AddFind(stats);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == player)
        {
            canvas.gameObject.SetActive(true) ;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform == player)
        {
            canvas.gameObject.SetActive(false);
        }
    }
}


[System.Serializable]
public class Find
{ 
    public Sprite sprite;
    public string name;
    public string description;
    public string dialogue;
}
