using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    public static AttackCollider instance;
    public List<GameObject> enemiesThatCanHit;

    private void Awake()
    {        
        instance = this;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {       
        if (collision.gameObject.layer == 6 && !enemiesThatCanHit.Contains(collision.gameObject))
        { 
            enemiesThatCanHit.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            enemiesThatCanHit.Remove(collision.gameObject);
        }
    }
}
