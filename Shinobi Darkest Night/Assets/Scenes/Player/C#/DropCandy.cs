using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCandy : MonoBehaviour
{
    [SerializeField] GameObject candyToDrop;

    public void Drop()
    { 
        if (candyToDrop != null) 
        {
            Vector2 point = transform.position + Random.insideUnitSphere;
            Instantiate(candyToDrop, point, Quaternion.Euler(0, 0, 0));
            candyToDrop = null;
        }
    }
}
