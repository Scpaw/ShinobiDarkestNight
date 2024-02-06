using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AnimationToCode : MonoBehaviour
{
    public Coroutine slowing;
    private PlayerController playerController;
    private void Awake()
    {
        playerController = transform.parent.GetComponent<PlayerController>();
    }
    public void Shoot()
    {
        if (playerController != null)
        {
            playerController.SpawnPoint();
        }
    }

    public void StartHeal()
    {
        playerController.canHeal = true;
    }

    public void DesumiruAttackRight()
    {
        playerController.DesumiruAttack(true);
    }
    public void DesumiruAttackleft()
    {
        playerController.DesumiruAttack(false);
    }

    public void SlowTime()
    {
        if (!playerController.desumiruPressed)
        {
            if (slowing != null)
            {
                StopCoroutine(slowing);
                slowing = StartCoroutine(playerController.SlowTime());
            }
            else
            {
                slowing = StartCoroutine(playerController.SlowTime());
            }
        }
    }
    public void StopDesumiru()
    { 
        playerController.StopDesumiru();
    }

    public void StartShokyaku()
    {
        playerController.shokyaku = true;
        playerController.canAttack = true;
        playerController.canMove = true;
        playerController.CurrentState = playerController.RunAnim;
    }


    public void SpawnItaiken()
    { 
        playerController.canAttack = true;
        playerController.canMove = true;
        playerController.SpawnItaiken(false);
    }

    public void TimeToDie()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void startItaiken()
    { 
        playerController.itaiken = true;
        playerController.canAttack = true;
        playerController.canMove = true;
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
