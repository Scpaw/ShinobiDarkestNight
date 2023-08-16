using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomBrain : MonoBehaviour
{
    public List<GameObject> enemies;
    [SerializeField] List<Transform> points;
    [SerializeField] List<GameObject> enemiesToSpaw;
    public int enemyNumber;
    public float maxDeviationFromPoint;

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
            if (enemies.Count == 0)
            {
                startSpawnEnemies();
            }
            else
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
    }

    public void startSpawnEnemies()
    {
        while (enemies.Count < enemyNumber)
        {
           enemies.Add(Instantiate(enemiesToSpaw[Random.Range(0, enemiesToSpaw.Count)], points[Random.Range(0, points.Count)].position + new Vector3(Random.Range(-maxDeviationFromPoint,maxDeviationFromPoint), Random.Range(-maxDeviationFromPoint, maxDeviationFromPoint)),Quaternion.Euler(Vector3.zero)));
        }

        foreach (GameObject enemy in enemies)
        { 
            enemy.transform.parent = transform;
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
