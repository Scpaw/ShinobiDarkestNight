using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AnimationToCode : MonoBehaviour
{
    public Coroutine slowing;
    private PlayerStateMachine player;
    private Coroutine cor;
    private Vector2 point2; 
    private void Awake()
    {
        player = transform.parent.GetComponent<PlayerStateMachine>();
    }
    public void Shoot()
    {
        if (player != null)
        {
            player.Shoot();
        }
    }

    public void StartHeal()
    {
        //player.canHeal = true;
    }

    public void DesumiruAttackRight()
    {
        DesumiruAttack(true);
    }
    public void DesumiruAttackleft()
    {
        DesumiruAttack(false);
    }

    public void SlowTime()
    {
       //if (!player.desumiruPressed)
       //{
       //    if (slowing != null)
       //    {
       //        StopCoroutine(slowing);
       //        slowing = StartCoroutine(player.SlowTime());
       //    }
       //    else
       //    {
       //        slowing = StartCoroutine(player.SlowTime());
       //    }
       //}
    }
    public void StopDesumiru()
    { 
        //player.StopDesumiru();
    }

    public void StartShokyaku()
    {
       //player.shokyaku = true;
       //player.canAttack = true;
       //player.canMove = true;
       //player.CurrentState = player.RunAnim;
    }


    public void SpawnItaiken()
    {
        Instantiate(player.itaikenToSpawn, player.transform.position, Quaternion.Euler(new Vector3(player.projectileSpawnPoint.transform.eulerAngles.x, player.projectileSpawnPoint.transform.eulerAngles.y, player.projectileSpawnPoint.transform.eulerAngles.z - 90)));
        //player.SpawnItaiken(false);
    }

    public void TimeToDie()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void startItaiken()
    { 
        //player.itaiken = true;
        //player.canAttack = true;
        //player.canMove = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && collision.GetComponent<EnemyHealth>())
        {
            if (collision.GetComponent<NewAi>())
            {
                collision.GetComponent<NewAi>().Stun(0.8f, point2*3f);
            }
            else if(collision.GetComponent<Rigidbody2D>())
            {
                collision.GetComponent<EnemyHealth>().Stun();
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.parent.GetComponent<PlayerController>().point2 * 1, ForceMode2D.Impulse);
            }

            collision.GetComponent<EnemyHealth>().enemyAddDamage(15, true, true);
            //   if (transform.parent.GetComponent<PlayerController>().desumiruState == 2)
            //   {
            //       if (collision.gameObject.GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Static)
            //       {
            //           collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            //           collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.parent.GetComponent<PlayerController>().point2 * 1, ForceMode2D.Impulse);
            //       }
            //   }
            //   else if (transform.parent.GetComponent<PlayerController>().desumiruState == 3)
            //   {
            //       if (collision.gameObject.GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Static)
            //       {
            //           collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            //           collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.parent.GetComponent<PlayerController>().point2 * 2, ForceMode2D.Impulse);
            //       }
            //   }
        }
    }

    public void DesumiruAttack(bool right)
    {
        if (cor != null)
        { 
            StopCoroutine(cor);
        }
        cor = StartCoroutine(DesumiruAttackUse(right));
    }

    private IEnumerator DesumiruAttackUse(bool right)
    {
            List<Vector2> vectors = new List<Vector2>();
            float animSpeed = player.anim.GetCurrentAnimatorStateInfo(0).speed;
            float test = player.anim.GetCurrentAnimatorStateInfo(0).length * animSpeed;
            float starAnimTime = player.anim.GetCurrentAnimatorStateInfo(0).length * animSpeed;
            vectors.Add(Vector2.zero);
            vectors.Add(Vector2.zero);
            vectors[1] = point2;
            while (test > 0)
            {
                if (!GetComponent<EdgeCollider2D>().enabled)
                {
                    GetComponent<EdgeCollider2D>().enabled = true;
                }
                if (right)
                {
                    point2 = new Vector2(player.desumiuRadius * Mathf.Cos((((starAnimTime - test) / starAnimTime) * 360 * Mathf.Deg2Rad) - (Mathf.Deg2Rad * 90)), player.desumiuRadius * Mathf.Sin((((starAnimTime - test) / starAnimTime) * 360 * Mathf.Deg2Rad) - (Mathf.Deg2Rad * 90)));
                }
                else
                {
                    point2 = new Vector2(player.desumiuRadius * Mathf.Cos(((test / starAnimTime) * 360 * Mathf.Deg2Rad) - (Mathf.Deg2Rad * 90)), player.desumiuRadius * Mathf.Sin(((test / starAnimTime) * 360 * Mathf.Deg2Rad) - (Mathf.Deg2Rad * 90)));
                }
                test -= Time.deltaTime * animSpeed * 1.6f;
                vectors[1] = point2;
                GetComponent<EdgeCollider2D>().SetPoints(vectors);
                yield return new WaitForEndOfFrame();
            }
            GetComponent<EdgeCollider2D>().enabled = false;
    }

}
