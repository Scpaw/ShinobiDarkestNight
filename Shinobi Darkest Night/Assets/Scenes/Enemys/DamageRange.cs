using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageRange : MonoBehaviour
{

    public bool playerInRange;

    void Start()
    {
        playerInRange = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            playerInRange = false;
        }
    }
}
