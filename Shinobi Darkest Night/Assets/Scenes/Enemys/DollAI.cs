using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollAI: MonoBehaviour
{
    // Start is called before the first frame update
    private enum State
    {
        Roaming
    }

    private State state;
    private DollPathFinding enemyPathfinding;
    // Update is called once per frame
    void Awake()
    {
        enemyPathfinding = GetComponent<DollPathFinding>();
            state = State.Roaming;
    }

    private void Start()
    {
        StartCoroutine(RoamingRoutine());
    }

    private IEnumerator RoamingRoutine()
    {
        while(state == State.Roaming)
        {
            Vector2 roamingPosition = GetRoamingPosition();
            enemyPathfinding.MoveTo(roamingPosition);
            yield return new WaitForSeconds(2f);
        }
    }

    private Vector2 GetRoamingPosition()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}
