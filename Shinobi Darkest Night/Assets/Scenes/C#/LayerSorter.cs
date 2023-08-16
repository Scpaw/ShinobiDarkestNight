using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSorter : MonoBehaviour
{
    /*
    private SpriteRenderer Renderer;

    private List<Obstacle> obstacleList = new List<Obstacle>();

    void Start()
    {
        Renderer = transform.Find("Grafika").GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Obstacle")
        {
            Obstacle o = collision.GetComponent<Obstacle>();

            if (obstacleList.Count == 0 || o.SpriteRenderer.sortingOrder - 1 < Renderer.sortingOrder)
            {
                Renderer.sortingOrder = collision.GetComponent<SpriteRenderer>().sortingOrder - 1;
            }

            obstacleList.Add(o);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Obstacle")
        {
            Obstacle o = collision.GetComponent<Obstacle>();

            obstacleList.Remove(o);

            if(obstacleList.Count == 0)
            {
                Renderer.sortingOrder = 200;
            }
            else
            {
                obstacleList.Sort();
                Renderer.sortingOrder = obstacleList[0].SpriteRenderer.sortingOrder - 1;
            }
        }
    }
    */
}
