using UnityEngine;
using UnityEngine.UI;

public class FindUI : MonoBehaviour
{
    public float minY;
    public float maxY;
    private RectTransform rect;
    private float scale;
    public float bufferZone;

    private void Start()
    {
        minY += 5;
        maxY -= 5;
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if ((rect.position.y > minY && rect.position.y < minY + bufferZone))
        {
            scale = 1 + (minY - rect.position.y) / bufferZone;
        }
        else if (rect.position.y < minY && rect.position.y > maxY)
        {
            scale = 1;
        }
        else if ((rect.position.y < maxY && rect.position.y > maxY - bufferZone))
        {
            scale = 1 - (maxY - rect.position.y) / bufferZone;
        }
        else if (rect.position.y < maxY - 21 || rect.position.y > minY + bufferZone)
        {
            scale = 0;
        }
        transform.localScale = new Vector3(scale, scale, scale);

        if (transform.localScale.x < 0 && transform.GetChild(0).gameObject.activeInHierarchy)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else if (transform.localScale.x >= 0 && !transform.GetChild(0).gameObject.activeInHierarchy)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
