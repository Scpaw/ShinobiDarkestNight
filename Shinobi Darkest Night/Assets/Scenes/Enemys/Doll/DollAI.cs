using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollAI : MonoBehaviour
{
    private enum State
    {
        Roaming,
        Following        
    }

    private State state;
    private DollPathFinding dollPathFinding;
    private GameObject player;
    public float speed;
    private bool detected;
    private float distance;
    private bool rom;

    private void Awake()
    {
        dollPathFinding = GetComponent<DollPathFinding>();
        state = State.Roaming;
        detected = false;
        player = GameObject.Find("Shinobi");
    }

    private void Update()
    {
        if (GetComponent<Enemy>().stundTime <= 0)
        {
            distance = Vector2.Distance(transform.position, player.transform.position);
            //Vector2 direction = player.transform.position - transform.position;
           // direction.Normalize();

            //Debug.Log(distance);
            //Debug.Log(direction);

            if (distance > 4 && detected == false)
            {
                if (rom == false)
                {
                    StartCoroutine(RoamingRoutine());
                    rom = true;
                }
            }
            else if (distance < 4)
            {
                rom = false;
                detected = true;
                StopCoroutine(RoamingRoutine());
                transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
                //dollPathFinding.MoveTo(player.transform.position);
            }

            if (distance > 4)
            {
                detected = false;
            }
        }
        else
        {
            StopCoroutine(RoamingRoutine());
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
