using UnityEngine;

public class BagGridSnapper : MonoBehaviour
{
    void LateUpdate()
    {
        int x = Mathf.FloorToInt((((RectTransform) transform).anchoredPosition.x) / BagManager.Instance.BagItemGridSize) * BagManager.Instance.BagItemGridSize;
        int y = Mathf.FloorToInt((((RectTransform) transform).anchoredPosition.y) / BagManager.Instance.BagItemGridSize) * BagManager.Instance.BagItemGridSize;

        ((RectTransform) transform).anchoredPosition = new Vector2(x, y);
    }
}