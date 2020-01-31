using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagItem : PoolObject, IDraggable
{
    [SerializeField] private Image Image;
    [SerializeField] private BagGridSnapper BagGridSnapper;
    [SerializeField] private BoxCollider BoxCollider;

    public MechaComponentInfo MechaComponentInfo;
    public List<GridPos> RealPosInBagPanel;
    private float _dragComponentDragMinDistance;
    private float _dragComponentDragMaxDistance;

    public void Initialize(MechaComponentInfo mci, int width, int height, GridPos myPos, List<GridPos> realPosInBagPanel)
    {
        MechaComponentInfo = mci;
        Image.sprite = BagManager.Instance.MechaComponentSpriteDict[mci.MechaComponentType];

        // Resize and rotate to fit the grid
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

        RealPosInBagPanel = realPosInBagPanel;
    }

    #region IDraggable

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

    float IDraggable.DragComponent_DragMinDistance => 0f;

    float IDraggable.DragComponent_DragMaxDistance => 99f;

    public void DragComponent_DragOutEffects()
    {
        MechaComponentBase mcb = MechaComponentBase.BaseInitialize(MechaComponentInfo, GameManager.Instance.PlayerMecha.transform, GameManager.Instance.PlayerMecha);
        GridPos gp = GridPos.GetGridPosByMousePos(GameManager.Instance.PlayerMecha.transform, Vector3.up, GameManager.GridSize);
        GridPos.ApplyGridPosToLocalTrans(gp, mcb.transform, GameManager.GridSize);
        DragManager.Instance.CancelCurrentDrag();
        DragManager.Instance.CurrentDrag = mcb.Draggable;
        mcb.Draggable.IsOnDrag = true;
        BagManager.Instance.RemoveMechaComponentFromBag(this);
        PoolRecycle();
    }

    #endregion
}