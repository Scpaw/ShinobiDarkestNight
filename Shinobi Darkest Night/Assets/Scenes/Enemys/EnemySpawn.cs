using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject parent;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            StartCoroutine(SpawnObjects());
        }
    }

    IEnumerator SpawnObjects()
    {
        var Instantiated0 = Instantiate(enemyPrefab, transform.GetChild(0).position, transform.rotation);
        Instantiated0.transform.parent = parent.transform;

        var Instantiated1 = Instantiate(enemyPrefab, transform.GetChild(1).position, transform.rotation);
        Instantiated1.transform.parent = parent.transform;

        var Instantiated2 = Instantiate(enemyPrefab, transform.GetChild(2).position, transform.rotation);
        Instantiated2.transform.parent = parent.transform;

        var Instantiated3 = Instantiate(enemyPrefab, transform.GetChild(3).position, transform.rotation);
        Instantiated3.transform.parent = parent.transform;

        var Instantiated4 = Instantiate(enemyPrefab, transform.GetChild(4).position, transform.rotation);
        Instantiated4.transform.parent = parent.transform;

        var Instantiated5 = Instantiate(enemyPrefab, transform.GetChild(5).position, transform.rotation);
        Instantiated5.transform.parent = parent.transform;

        DestroySpawn();
        yield return new WaitForSeconds(2f);
    }

    void DestroySpawn()
    {
        Destroy(gameObject, 0);
    }
}
