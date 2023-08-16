using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomBrain : MonoBehaviour
{
    [Tooltip("Current enemies spawned")]
    public List<GameObject> enemies;

    [Tooltip("points to spawn enemies")]
    [SerializeField] List<Transform> points;

    [Tooltip("Category of enemies to spawn")]
    [SerializeField] List<GameObject> enemiesToSpaw;

    [Tooltip("how meny enemies to spawn")]
    public int enemyNumber;

    [Tooltip("Deviation form spawn point")]
    public float maxDeviationFromPoint;


    //Rigidbody
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
