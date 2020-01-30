using UnityEngine;

public class BagGridSnapper : MonoBehaviour
{
    private RectTransform RectTransform;
    internal RectTransform Offset;

    void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        float x_delta = 0;
        float y_delta = 0;
        if (Offset)
        {
            x_delta = Offset.anchoredPosition.x % BagManager.Instance.BagItemGridSize;
            y_delta = Offset.anchoredPosition.y % BagManager.Instance.BagItemGridSize;
        }

        int x = Mathf.FloorToInt((RectTransform.anchoredPosition.x - x_delta) / BagManager.Instance.BagItemGridSize) * BagManager.Instance.BagItemGridSize + Mathf.RoundToInt(x_delta) ;
        int y = Mathf.FloorToInt((RectTransform.anchoredPosition.y - y_delta) / BagManager.Instance.BagItemGridSize) * BagManager.Instance.BagItemGridSize + Mathf.RoundToInt(y_delta);

        RectTransform.anchoredPosition = new Vector2(x,y);
    }
}