using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AnimationToCode : MonoBehaviour
{
    public Coroutine slowing;
    private PlayerStateMachine player;
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
        //player.DesumiruAttack(true);
    }
    public void DesumiruAttackleft()
    {
        //player.DesumiruAttack(false);
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
                collision.GetComponent<NewAi>().Stun(0.8f, transform.parent.GetComponent<PlayerController>().point2*3f);
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
}
