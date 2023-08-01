using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollAI : MonoBehaviour
{
    private enum State
    {
        Roaming
    }

    private State state;
    private DollPathFinding dollPathFinding;
    public GameObject player;
    public float speed;
    private bool detected;
    private float distance;
    private bool rom;

    private void Awake()
    {
        dollPathFinding = GetComponent<DollPathFinding>();
        state = State.Roaming;
        detected = false;
    }

    private void Update()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();
        

        if (distance > 4 && detected == false)
        {
            if(rom == false)
            {
                StartCoroutine(RoamingRoutine());
                rom = true;
            }
        }
        else if(distance < 4)
        {
            rom = false;
            detected = true;
            StopCoroutine(RoamingRoutine());
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
        }

        if(distance > 4)
        {
            detected = false;
        }
    }

    private IEnumerator RoamingRoutine()
    {
        while(state == State.Roaming)
        {
            Vector2 roamingPosition = GetRoamingPosition();
            dollPathFinding.MoveTo(roamingPosition);
            yield return new WaitForSeconds(2f);
        }
    }

    private Vector2 GetRoamingPosition()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}
