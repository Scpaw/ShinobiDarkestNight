using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_Bow : MonoBehaviour
{
    GameObject player;
    private float distance;
    private float sightRange;

    private void Start()
    {
        player = PlayerStateMachine.Instance.gameObject;
        sightRange = GetComponentInParent<NewAi>().detectionRange;
    }
    void Update()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (distance < sightRange)
        {
            transform.rotation = Quaternion.Euler(Vector3.forward * angle);
        }
    }
}
