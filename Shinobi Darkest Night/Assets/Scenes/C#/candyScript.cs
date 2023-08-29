using System.Collections;
using System.Collections.Generic;
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
        if (transform.localScale.x >= 0.9f && onScreen)
        {
            PlayerController.Instance.CandyText(description);
        }
    }

}
