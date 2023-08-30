using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class candyScript : MonoBehaviour
{
    float r;
    private Vector2 screenPos;
    public bool onScreen;
    public int id;
    public float angle;
    [SerializeField] string description;
    [SerializeField] CandyUse candyState;

    void Update()
    {
        r = PlayerController.Instance.GetShadePos();
        if (transform.position.y / (r) - 0.8 >= 0)
        {
            if (!GetComponent<Image>().enabled)
            {
                GetComponent<Image>().enabled = true;
            }
            float scale = ((transform.position.y / (r) - 0.8f) * 5f);
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        { 
            GetComponent<Image>().enabled = false;

        }
        transform.rotation = Quaternion.Euler(0, 0, 0);
        if (GetComponent<Image>().enabled)
        {
            if (!onScreen)
            {
                PlayerController.Instance.AddObjectOnScreen(gameObject);
                onScreen = true;
            }
        }
        else
        {
            if (onScreen)
            {
                PlayerController.Instance.RemoveObjectOnScreen(gameObject);
                onScreen = false;
            }
        }
        if (transform.localScale.x >= 0.7f && onScreen)
        {
            PlayerController.Instance.CandyText(description,gameObject);
        }
    }

    public void DoStuff()
    {
        if (candyState == CandyUse.sakuramochi)
        {
            PlayerController.Instance.sakuramochi = 60;
            PlayerController.Instance.GetComponent<PlayerHealth>().AddHealth(100);
            PlayerController.Instance.powerCoolDown = 60 + PlayerController.Instance.sakuramochi;
        }
        else if (candyState == CandyUse.yokan)
        {
            PlayerController.Instance.yokan = 60;
            PlayerController.Instance.powerCoolDown = 60 + PlayerController.Instance.yokan;
        }
        else if (candyState == CandyUse.dango)
        {
            PlayerController.Instance.dango = 60;
            Object[] enemies = Resources.FindObjectsOfTypeAll(typeof(EnemyHealth));
            foreach (Object enemy in enemies)
            {
                if (enemy.GameObject().activeInHierarchy)
                {
                    enemy.GameObject().GetComponent<EnemyHealth>().ProjectilesOff(15);
                }
            }
            PlayerController.Instance.powerCoolDown = 60 + PlayerController.Instance.dango;
        }
        else if (candyState == CandyUse.mizuame)
        {
            PlayerController.Instance.mizuame = 40;
            PlayerController.Instance.powerCoolDown = 60 + PlayerController.Instance.mizuame;
        }
    }

}
public enum CandyUse
{
    sakuramochi,
    yokan,
    dango,
    mizuame
}
