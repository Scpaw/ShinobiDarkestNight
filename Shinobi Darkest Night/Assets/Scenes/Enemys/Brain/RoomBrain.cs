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
    public List<GameObject> enemiesToSpaw;

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
                Debug.Log("123");
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

    private void Update()
    {
        //Debug.Log(pointNotOnScreen());
    }
    public void startSpawnEnemies()
    {
        while (enemies.Count < enemyNumber)
        {
           enemies.Add(Instantiate(enemiesToSpaw[Random.Range(0, enemiesToSpaw.Count)], pointNotOnScreen() ,Quaternion.Euler(Vector3.zero)));

        }

        foreach (GameObject enemy in enemies)
        { 
            enemy.transform.parent = transform;
        }
    }

    public Vector3 pointNotOnScreen()
    {
        Vector3 currentPoint = points[Random.Range(0, points.Count)].position + new Vector3(Random.Range(-maxDeviationFromPoint, maxDeviationFromPoint), Random.Range(-maxDeviationFromPoint, maxDeviationFromPoint));
        Vector2 thisPoint = Camera.main.WorldToViewportPoint(currentPoint);
        if ((thisPoint.x > 1 || thisPoint.x < 0) && (thisPoint.y > 1 || thisPoint.y < 0))
        {
            return currentPoint;
        }
        else
        {
            return pointNotOnScreen();
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
                enemy.GetComponent<EnemyHealth>().startPos = pointNotOnScreen();
                enemy.SetActive(true);              
            }
        }


    }
}
