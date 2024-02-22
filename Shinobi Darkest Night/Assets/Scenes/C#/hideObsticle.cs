using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hideObsticle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer obstacleSprite;
    [SerializeField] private Color behindColor;
    [SerializeField] private LayerMask enemy;

    //private GameObject player;
    private GameObject playerP;
    //private float playerT;
    private SpriteRenderer playerSprite;
    //private Coroutine changeCorutine;
    //private float O;
    //private int somethinIn;
    public bool somethingBehind;
    private Color startColor;
    


    private void Start()
    {
        startColor = obstacleSprite.color;

        playerP = PlayerStateMachine.Instance.gameObject;
        playerSprite = playerP.transform.Find("Grafika").GetComponent<SpriteRenderer>();
        //player = PlayerStateMachine.Instance.gameObject;
        if (GetComponent<SpriteRenderer>())
        {
            obstacleSprite = GetComponent<SpriteRenderer>();
        }
        else
        {
            obstacleSprite = GetComponentInChildren<SpriteRenderer>();
        }


/*        O = gameObject.transform.position.y;
        playerT = playerP.transform.position.y;*/
    }

    private void Update()
    {
        /*O = gameObject.transform.position.y;
        playerT = playerP.transform.position.y;*/

        if(gameObject.tag == "Obstacle"  && (playerP.transform.position - transform.position).magnitude < 3)
        {
            if (GetComponent<DropCandy>())
            {
                GetComponent<DropCandy>().Drop();
            }
/*            if (playerT > O && somethingBehind)
            {
                obstacleSprite.sortingOrder = playerSprite.sortingOrder + 1;
            }
            else if (O > playerT && !somethingBehind)
            {
                obstacleSprite.sortingOrder = playerSprite.sortingOrder - 1;
            }*/
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == playerP )//|| collision.gameObject.layer == enemy)
        {
            /*            if (!somethingBehind)
                        {
                            if (changeCorutine != null)
                            {
                                StopCoroutine(changeCorutine);
                                changeCorutine = StartCoroutine(Change(0.6f));
                            }
                            else
                            {
                                changeCorutine = StartCoroutine(Change(0.6f));
                            }
                        }*/

            obstacleSprite.color = behindColor;
            somethingBehind = true;
            obstacleSprite.sortingOrder = playerSprite.sortingOrder + 1;
        }
/*
        if (collision.gameObject.layer == 6)
        {
            obstacleSprite.sortingOrder = playerSprite.sortingOrder + 1;
        }*/
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == playerP )//|| collision.gameObject.layer == enemy)
        {
            obstacleSprite.color = startColor;
            somethingBehind = false;
            obstacleSprite.sortingOrder = playerSprite.sortingOrder - 1;

            /*            if (!somethingBehind)
                        {


                            if (changeCorutine != null)
                            {
                                StopCoroutine(changeCorutine);
                                changeCorutine = StartCoroutine(Change(1));
                            }
                        }*/
        }
/*
        if (collision.gameObject.layer == 6)
        {
            obstacleSprite.sortingOrder = playerSprite.sortingOrder - 1;
        }*/

    }

/*    IEnumerator Change(float changeTo)
    {
        float alpha = obstacleSprite.color.a;
        if (obstacleSprite.color.a > changeTo)
        {
            while (obstacleSprite.color.a > changeTo)
            {
                obstacleSprite.color = new Color(obstacleSprite.color.r, obstacleSprite.color.g, obstacleSprite.color.b, alpha -=2*Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }
        else if (obstacleSprite.color.a < changeTo)
        {
            while (obstacleSprite.color.a < changeTo)
            {
                obstacleSprite.color = new Color(obstacleSprite.color.r, obstacleSprite.color.g, obstacleSprite.color.b, alpha += 2*Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }
    }*/
}
