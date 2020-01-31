using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagItem : PoolObject, IDraggable
{
    [SerializeField] private Image Image;
    [SerializeField] private BagGridSnapper BagGridSnapper;
    [SerializeField] private BoxCollider BoxCollider;

    public void Initialize(MechaComponentInfo mci, int width, int height, GridPos myPos)
    {
        Image.sprite = BagManager.Instance.MechaComponentSpriteDict[mci.M_MechaComponentType];
        Vector2 size = new Vector2(width * BagManager.Instance.BagItemGridSize, height * BagManager.Instance.BagItemGridSize);
        Vector2 size_rev = new Vector2(height * BagManager.Instance.BagItemGridSize, width * BagManager.Instance.BagItemGridSize);
        bool isRotated = myPos.orientation == GridPos.Orientation.Right || myPos.orientation == GridPos.Orientation.Left;
        if (isRotated)
        {
            ((RectTransform) transform).sizeDelta = size_rev;
            BoxCollider.size = new Vector3(size_rev.x, size_rev.y, 1);
            BoxCollider.center = new Vector3(size_rev.x / 2f, size_rev.y / -2f, 0);
            Image.rectTransform.sizeDelta = size;
            Image.transform.rotation = Quaternion.Euler(0, 0, 90f);
        }
        else
        {
            ((RectTransform) transform).sizeDelta = size;
            BoxCollider.size = new Vector3(size.x, size.y, 1);
            BoxCollider.center = new Vector3(size.x / 2f, size.y / -2f, 0);
            Image.rectTransform.sizeDelta = size;
            Image.transform.rotation = Quaternion.Euler(0, 0, 0f);
        }

        ((RectTransform) transform).anchoredPosition = new Vector2(myPos.x * BagManager.Instance.BagItemGridSize, -myPos.z * BagManager.Instance.BagItemGridSize);
    }

    #region  IDraggable

    public void DragComponent_OnMouseDown()
    {
    }

    public void DragComponent_OnMousePressed(DragAreaTypes dragAreaTypes)
    {
    }

    public void DragComponent_OnMouseUp(DragAreaTypes dragAreaTypes)
    {
    }

    public void DragComponent_SetStates(ref bool canDrag, ref DragAreaTypes dragFrom)
    {
        canDrag = true;
        dragFrom = DragAreaTypes.Bag;
    }

    public float DragComponent_DragMinDistance()
    {
        return 0f;
    }

    public float DragComponent_DragMaxDistance()
    {
        return 999f;
    }

    public void DragComponent_DragOutEffects()
    {
    }

    #endregion
}