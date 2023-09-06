using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dash : MonoBehaviour
{
    private AILerp ai;
    private Transform player;
    private Vector3 attackPoint;
    public float timebtwAttacks;
    public bool canMove;
    private EnemyDamage damageRange;
    private bool hitPlayer;
    [SerializeField] LayerMask playerLayer;
    private GameObject hit;
    private Coroutine dashing;
    [SerializeField] private int dmg;
    public int dashState;
    public bool stopAtPlayer;
    public bool dontAttackWhileDashing;
    [SerializeField] float dashPower;

    private void Awake()
    {
        ai = GetComponent<AILerp>();
        player = PlayerController.Instance.GetPlayer().transform;
        if (GetComponent<EnemyDamage>())
        {
            damageRange = GetComponent<EnemyDamage>();
        }

    }


    public void StartDash()
    {
        if (Physics2D.Raycast(transform.position, (player.position - transform.position).normalized, 30, LayerMask.GetMask("Player")) && dashing == null)
        {
            hit = Physics2D.Raycast(transform.position, -transform.position + player.position, 100, playerLayer).transform.gameObject;
            if (hit.layer == player.gameObject.layer)
            {
                dashing = StartCoroutine(DashCor());
                timebtwAttacks = 1;
            }
        }
    }
    private IEnumerator DashCor()
    {
        dashState = 1;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 startPos = transform.position;
        canMove = false;
        if (GetComponent<EnemyDamage>())
        {
            damageRange.enabled = false;
        }
        float i = 0.4f;
        ai.canMove = false;

        while (i > 0)
        {

            if (Random.value < 0.3f)
            {
                transform.position = startPos + Random.insideUnitCircle / 30;
            }

            i -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        dashState = 2;
        transform.position = startPos;
        hit = Physics2D.Raycast(transform.position, -transform.position + player.position, 100, playerLayer).transform.gameObject;
        if (hit.layer == player.gameObject.layer)
        {
            ParticleManager.instance.UseParticle("Dust", transform.position, Vector3.zero);
            attackPoint = player.position + ((player.position - transform.position).normalized * 2);
            i = 1;
            rb.AddForce((attackPoint - transform.position) * dashPower, ForceMode2D.Impulse);
            if (!stopAtPlayer)
            {
                while (i >0)
                {
                    i -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                while (i > 0 && (transform.position - player.position).magnitude > 0.6f)
                {
                    i -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                if ((transform.position - player.position).magnitude < 0.62f)
                {
                    rb.velocity = Vector3.zero;
                }
            }
        }
        else
        {
            timebtwAttacks = 0;
            dashState = 0;
            dashState = 0;
        }
        dashState = 3;
        yield return new WaitForSeconds(0.1f);
        dashState = 0;
        canMove = true;
        if (GetComponent<EnemyDamage>())
        {
            damageRange.enabled = true;
        }
        hitPlayer = false;
        //ai.SetNewPath();
        dashing = null;


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player.gameObject && !hitPlayer && !dontAttackWhileDashing)
        {
            player.GetComponent<PlayerHealth>().AddDamage(dmg * 1.5f);
            hitPlayer = true;
        }
    }
}
