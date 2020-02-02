using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagItem : PoolObject, IDraggable
{
    [SerializeField] private Image Image;
    [SerializeField] private BagItemGridHitBoxes BagItemGridHitBoxes;

    public MechaComponentInfo MechaComponentInfo;
    internal GridPos GridPos_BeforeMove;
    public List<GridPos> RealPositionsInBagPanel_BeforeMove;
    internal GridPos GridPos;
    public List<GridPos> RealPositionsInBagPanel;
    private float _dragComponentDragMinDistance;
    private float _dragComponentDragMaxDistance;
    internal int Width;
    internal int Height;

    public void Initialize(MechaComponentInfo mci, int width, int height, GridPos myPos, List<GridPos> realPositionsInBagPanel, bool moving)
    {
        MechaComponentInfo = mci;
        Image.sprite = BagManager.Instance.MechaComponentSpriteDict[mci.MechaComponentType];
        Width = width;
        Height = height;
        GridPos = myPos;
        if (!moving) GridPos_BeforeMove = myPos;

        // Resize and rotate to fit the grid
        Vector2 size = new Vector2(Width * BagManager.Instance.BagItemGridSize, Height * BagManager.Instance.BagItemGridSize);
        Vector2 size_rev = new Vector2(Height * BagManager.Instance.BagItemGridSize, Width * BagManager.Instance.BagItemGridSize);
        bool isRotated = GridPos.orientation == GridPos.Orientation.Right || GridPos.orientation == GridPos.Orientation.Left;
        if (isRotated)
        {
            ((RectTransform) transform).sizeDelta = size;
            BagItemGridHitBoxes.Initialize(realPositionsInBagPanel, myPos);
            Image.rectTransform.sizeDelta = size_rev;
            Image.transform.rotation = Quaternion.Euler(0, 0, 90f);
        }
        else
        {
            ((RectTransform) transform).sizeDelta = size;
            BagItemGridHitBoxes.Initialize(realPositionsInBagPanel, myPos);
            Image.rectTransform.sizeDelta = size;
            Image.transform.rotation = Quaternion.Euler(0, 0, 0f);
        }

        ((RectTransform) transform).anchoredPosition = new Vector2(GridPos.x * BagManager.Instance.BagItemGridSize, -GridPos.z * BagManager.Instance.BagItemGridSize);
        RealPositionsInBagPanel = realPositionsInBagPanel;
        if (!moving)
        {
            RealPositionsInBagPanel_BeforeMove = CloneVariantUtils.List(RealPositionsInBagPanel);
        }
    }

    private void Rotate()
    {
        GridPos.Orientation newOrientation = GridPos.orientation == GridPos.Orientation.Up ? GridPos.Orientation.Right : GridPos.Orientation.Up;

        List<GridPos> newRealPositions = new List<GridPos>();
        foreach (GridPos gp in RealPositionsInBagPanel)
        {
            GridPos newLocalGrid = GridPos.RotateGridPos(new GridPos(gp.x - GridPos.x, gp.z - GridPos.z), GridPos.orientation == GridPos.Orientation.Up ? GridPos.Orientation.Right : GridPos.Orientation.Left);
            GridPos newRealGrid = new GridPos(newLocalGrid.x + GridPos.x, newLocalGrid.z + GridPos.z, GridPos.Orientation.Up);
            newRealPositions.Add(newRealGrid);
        }

        RealPositionsInBagPanel.Clear();
        RealPositionsInBagPanel = newRealPositions;

        //todo edit RealPositionsInBagPanel and change in bagpanel
        Initialize(MechaComponentInfo, Height, Width, new GridPos(GridPos.x, GridPos.z, newOrientation), newRealPositions, true);
    }

    #region IDraggable

    public void DragComponent_OnMouseDown()
    {
        BagManager.Instance.RemoveMechaComponentFromBag(this, true);
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
                }

                RefreshPreviewGridPositions();
                break;
            }
        }
    }

    private void RefreshPreviewGridPositions()
    {
        Vector2 pos = ((RectTransform) transform).anchoredPosition;
        int x = Mathf.FloorToInt((pos.x) / BagManager.Instance.BagItemGridSize);
        int y = Mathf.FloorToInt(-(pos.y) / BagManager.Instance.BagItemGridSize);

        int x_delta = x - GridPos.x;
        int y_delta = y - GridPos.z;

        if (x_delta != 0 || y_delta != 0)
        {
            List<GridPos> newRealPositions = new List<GridPos>();
            GridPos = new GridPos(x, y, GridPos.orientation);
            foreach (GridPos gp in RealPositionsInBagPanel)
            {
                GridPos newRealGrid = new GridPos(gp.x + x_delta, gp.z + y_delta, gp.orientation);
                newRealPositions.Add(newRealGrid);
            }

            RealPositionsInBagPanel.Clear();
            RealPositionsInBagPanel = newRealPositions;
        }
    }

    public void DragComponent_OnMouseUp(DragAreaTypes dragAreaTypes)
    {
        BagManager.Instance.RemoveMechaComponentFromBag(this, false);
        if (dragAreaTypes == DragAreaTypes.Bag)
        {
            bool suc = BagManager.Instance.AddMechaComponentToBag(MechaComponentInfo, GridPos.orientation, RealPositionsInBagPanel, out BagItem _);
            if (!suc)
            {
                BagManager.Instance.AddMechaComponentToBag(MechaComponentInfo, GridPos_BeforeMove.orientation, RealPositionsInBagPanel_BeforeMove, out BagItem _);
            }
        }
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
        GameManager.Instance.PlayerMecha.AddMechaComponent(mcb);
        DragManager.Instance.CancelCurrentDrag();
        DragManager.Instance.CurrentDrag = mcb.Draggable;
        mcb.Draggable.IsOnDrag = true;
        BagManager.Instance.RemoveMechaComponentFromBag(this, false);
        PoolRecycle();

        if (!BagManager.Instance.InfiniteComponents)
        {
            BagManager.Instance.AddMechaComponentToBag(MechaComponentInfo, out BagItem _);
        }
    }

    #endregion
}