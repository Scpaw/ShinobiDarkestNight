using UnityEngine;
using UnityEngine.UI;

public class FindUI : MonoBehaviour
{
    public float minY;
    public float maxY;
    private RectTransform rect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        if (rect.position.y > minY + 10 )
        {
            float scale = 0;
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else if (rect.position.y < maxY - 10)
        {
            float scale = 0;
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    void Update()
    {
        if (rect.position.y > minY + 10 && transform.localScale.x > 0)
        {
            float scale = 1-((rect.position.y - minY)/20);
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else if (rect.position.y < maxY -10 && transform.localScale.x > 0)
        {
            float scale = ((rect.position.y - maxY) / 20);
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else if (transform.localScale.x < 1 &&( rect.position.y <= minY + 10 && rect.position.y >= maxY - 10))
        {
            if (Mathf.Abs(rect.position.y - minY) < Mathf.Abs(rect.position.y - maxY))
            {
                float scale = 1 - ((rect.position.y - minY) / 20);
                transform.localScale = new Vector3(scale, scale, scale);
            }
            else
            {
                float scale = ((rect.position.y - maxY) / 20);
                transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        if (transform.localScale.x < 0 && transform.GetChild(0).gameObject.activeInHierarchy)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else if (transform.localScale.x > 0 && !transform.GetChild(0).gameObject.activeInHierarchy)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
