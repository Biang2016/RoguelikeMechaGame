using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagItem : PoolObject, IDraggable
{
    [SerializeField] private Image Image;
    [SerializeField] private BagGridSnapper BagGridSnapper;
    [SerializeField] private BoxCollider BoxCollider;

    public MechaComponentInfo MechaComponentInfo;
    public List<GridPos> RealPositionsInBagPanel;
    private float _dragComponentDragMinDistance;
    private float _dragComponentDragMaxDistance;
    internal int Width;
    internal int Height;
    internal GridPos GridPos;

    public void Initialize(MechaComponentInfo mci, int width, int height, GridPos myPos, List<GridPos> realPositionsInBagPanel)
    {
        MechaComponentInfo = mci;
        Image.sprite = BagManager.Instance.MechaComponentSpriteDict[mci.MechaComponentType];
        Width = width;
        Height = height;
        GridPos = myPos;

        // Resize and rotate to fit the grid
        Vector2 size = new Vector2(Width * BagManager.Instance.BagItemGridSize, Height * BagManager.Instance.BagItemGridSize);
        Vector2 size_rev = new Vector2(Height * BagManager.Instance.BagItemGridSize, Width * BagManager.Instance.BagItemGridSize);
        bool isRotated = GridPos.orientation == GridPos.Orientation.Right || GridPos.orientation == GridPos.Orientation.Left;
        if (isRotated)
        {
            ((RectTransform) transform).sizeDelta = size;
            BoxCollider.size = new Vector3(size.x, size.y, 1);
            BoxCollider.center = new Vector3(size.x / 2f, size.y / -2f, 0);
            Image.rectTransform.sizeDelta = size_rev;
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

        ((RectTransform) transform).anchoredPosition = new Vector2(GridPos.x * BagManager.Instance.BagItemGridSize, -GridPos.z * BagManager.Instance.BagItemGridSize);
        RealPositionsInBagPanel = realPositionsInBagPanel;
    }

    public void Rotate()
    {
        GridPos.Orientation newOrientation = (GridPos.Orientation) (((int) GridPos.orientation + 1) % 4);
        //todo edit RealPositionsInBagPanel and change in bagpanel
        Initialize(MechaComponentInfo, Height, Width, new GridPos(GridPos.x, GridPos.z, newOrientation), RealPositionsInBagPanel);
    }

    #region IDraggable

    public void DragComponent_OnMouseDown()
    {
    }

    public void DragComponent_OnMousePressed(DragAreaTypes dragAreaTypes)
    {
        switch (dragAreaTypes)
        {
            case DragAreaTypes.Bag:
            {
                if (Input.GetKeyUp(KeyCode.R))
                {
                    Rotate();
                    return;
                }

                break;
            }
        }
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