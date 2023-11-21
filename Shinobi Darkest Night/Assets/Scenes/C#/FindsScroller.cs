
using UnityEngine;

public class FindsScroller : MonoBehaviour
{
    private float minY;
    private float maxY;
    public float scrollSpeed;
    private RectTransform rect;
    private bool canScroll;
    [SerializeField] private float scalingZone = 20f;

    void Start()
    {
        minY = transform.GetChild(0).GetComponent<RectTransform>().position.y - 5;
        maxY = minY - Screen.height/1.65f;
        rect = GetComponent<RectTransform>();
        foreach (Transform child in transform)
        {
            if (child.GetComponent<FindUI>())
            {
                child.GetComponent<FindUI>().minY = minY;
                child.GetComponent<FindUI>().maxY = maxY;
                child.GetComponent<FindUI>().bufferZone = scalingZone;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy && canScroll)
        {
            rect.position = new Vector2(rect.position.x, rect.position.y + (Input.mouseScrollDelta.y) * scrollSpeed );
        }

        if (transform.GetChild(0).GetComponent<RectTransform>().position.y < minY)
        {
            if (canScroll)
            {
                canScroll = false;
            }
            rect.position = new Vector2(rect.position.x,rect.position.y + scrollSpeed *10* Time.unscaledDeltaTime);
        }
        else if (transform.GetChild(transform.childCount - 1).GetComponent<RectTransform>().position.y > maxY)
        {
            if (canScroll)
            {
                canScroll = false;
            }
            rect.position = new Vector2(rect.position.x, rect.position.y - scrollSpeed *10* Time.unscaledDeltaTime);
        }
        else if(!canScroll)
        {
            canScroll = true;
        }
    }
}
