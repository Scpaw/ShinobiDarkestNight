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

    public int enemiesActive;
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
                if (enemiesToSpaw.Count > 0)
                {
                    startSpawnEnemies();
                }
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
            Vector3 spawnPoint = pointNotOnScreen();
            var enemy = Instantiate(enemiesToSpaw[Random.Range(0, enemiesToSpaw.Count)], spawnPoint, Quaternion.Euler(Vector3.zero),transform);
            if (enemy.GetComponent<AI_Move>())
            {
                enemy.GetComponent<AI_Move>().room = GetComponent<AiBrain>();
            }
            enemies.Add(enemy);
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
                if (enemy.GetComponent<AI_Move>())
                {
                    enemy.transform.position = pointNotOnScreen();
                }
                enemy.SetActive(true);
            }
        }


    }

    private void OnDrawGizmos()
    {
      /// foreach (Transform point in points)
      /// {
      ///     //UnityEditor.Handles.DrawWireDisc(point.position, Vector3.back, maxDeviationFromPoint);
      /// }
        //UnityEditor.Handles.color = Color.red;
    }
}
