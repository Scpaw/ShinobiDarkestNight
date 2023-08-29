using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpCandy : MonoBehaviour
{
    private Transform player;
    [SerializeField] private float detectDistance;
    [SerializeField] private GameObject candy;

    void Start()
    {
        player = PlayerController.Instance.GetPlayer().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if ((player.position - transform.position).magnitude < detectDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, Time.deltaTime * 4);
            if ((player.position - transform.position).magnitude < 0.1f)
            {
                player.GetComponent<PlayerController>().candy.Add(candy);
                Destroy(gameObject);
            }
        }
    }
}
