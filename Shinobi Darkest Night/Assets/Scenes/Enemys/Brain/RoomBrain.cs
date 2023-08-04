using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomBrain : MonoBehaviour
{
    public List<GameObject> enemies;
    void Start()
    {
        foreach (Transform child in transform)
        { 
            enemies.Add(child.gameObject);
        }
        foreach(GameObject enemy in enemies)
        {
            if (enemy.activeInHierarchy)
            { 
                enemy.SetActive(false);
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            foreach (GameObject enemy in enemies)
            {
                if (!enemy.activeInHierarchy)
                {
                    enemy.SetActive(true);
                }
            }
        }
    }


    public void SpawnEnemies()
    {
        int i = 0;
        foreach (GameObject enemy in enemies)
        {
            if (enemy.activeInHierarchy)
            {
                i++;
            }
        }
        if(i == 0)
        {
            StartCoroutine(WaitToSpawn());
        }
    }

    IEnumerator WaitToSpawn()
    {
        yield return new WaitForSeconds(3);

        foreach (GameObject enemy in enemies)
        {
            if (!enemy.activeInHierarchy)
            {
                enemy.SetActive(true);
            }
        }


    }
}
