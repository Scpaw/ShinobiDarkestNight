using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class DropCandy : MonoBehaviour
{
    [SerializeField] GameObject candyToDrop;

    public void Drop()
    { 
        if (candyToDrop != null) 
        {
            StartCoroutine(Dropping());
            candyToDrop = null;
        }
    }

    private IEnumerator Dropping()
    {
        Vector2 point = new Vector2( transform.position.x + Mathf.Cos(Random.Range(0,360)) * 1.1f,
            transform.position.y + Mathf.Sin(Random.Range(0, 360)) * 1.1f);
        Vector2 point2 = new Vector2(Mathf.Lerp(transform.position.x, point.x, 0.2f),
            Mathf.Lerp(transform.position.y, point.y, 0.2f));
        GameObject candy = Instantiate(candyToDrop, transform.position, Quaternion.Euler(0, 0, 0));
        candy.GetComponent<PickUpCandy>().enabled = false;
        float timer = 1;
        float scale = 0;
        while (timer > 0)
        {
       
            if (timer > 0.7)
            {
                scale = Mathf.Lerp(1, 1.5f,(1- timer)/0.3f);
                candy.transform.position = new Vector2(Mathf.Lerp(transform.position.x, point2.x, (1 - timer) / 0.3f),
                    Mathf.Lerp(transform.position.y, point2.y, (1 - timer) / 0.3f));
            }
            else
            {
                scale = Mathf.Lerp(1.5f, 1, 1- timer/0.7f);
                candy.transform.position = new Vector2(Mathf.Lerp(point2.x, point.x, 1 - timer / 0.7f),
                    Mathf.Lerp(point2.y, point.y, 1 - timer / 0.7f));
            }
            candy.transform.localScale = new Vector3 (scale, scale, scale);
            timer -= Time.deltaTime *2;
            yield return new WaitForEndOfFrame();
        }

        timer = 0.1f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        candy.GetComponent<PickUpCandy>().enabled = true;
    }
}
