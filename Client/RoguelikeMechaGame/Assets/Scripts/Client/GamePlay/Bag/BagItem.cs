using System.Collections.Generic;
using GameCore;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class BagItem : PoolObject, IDraggable
    {
        private BagItemInfo Data;

        [SerializeField] private Image Image;
        [SerializeField] private BagItemGridHitBoxes BagItemGridHitBoxes;

        public List<GridPos> OccupiedPositionsInBagPanel;
        public GridPos GridPos_AfterMove;
        public List<GridPos> OccupiedPositionsInBagPanel_AfterMove;

        private float _dragComponentDragMinDistance;
        private float _dragComponentDragMaxDistance;

        public void Initialize(BagItemInfo bii, bool moving)
        {
            Data = bii;
            Image.sprite = BagManager.Instance.MechaComponentSpriteDict[Data.MechaComponentInfo.MechaComponentType];
            GridPos_AfterMove = bii.GridPos;
            if (!moving) Data.GridPos = bii.GridPos;

            // Resize and rotate to fit the grid
            Vector2 size = new Vector2(bii.Size.width * BagManager.Instance.BagItemGridSize, bii.Size.height * BagManager.Instance.BagItemGridSize);
            Vector2 size_rev = new Vector2(bii.Size.height * BagManager.Instance.BagItemGridSize, bii.Size.width * BagManager.Instance.BagItemGridSize);
            bool isRotated = GridPos_AfterMove.orientation == GridPos.Orientation.Right || GridPos_AfterMove.orientation == GridPos.Orientation.Left;

            ((RectTransform) transform).sizeDelta = size;
            BagItemGridHitBoxes.Initialize(bii.OccupiedGridPositions, bii.GridPos);
            if (isRotated)
            {
                Image.rectTransform.sizeDelta = size_rev;
                Image.transform.rotation = Quaternion.Euler(0, 0, 90f);
            }
            else
            {
                Image.rectTransform.sizeDelta = size;
                Image.transform.rotation = Quaternion.Euler(0, 0, 0f);
            }

            ((RectTransform) transform).anchoredPosition = new Vector2(GridPos_AfterMove.x * BagManager.Instance.BagItemGridSize, -GridPos_AfterMove.z * BagManager.Instance.BagItemGridSize);
            OccupiedPositionsInBagPanel_AfterMove = CloneVariantUtils.List(bii.OccupiedGridPositions);
            if (!moving)
            {
                OccupiedPositionsInBagPanel = CloneVariantUtils.List(OccupiedPositionsInBagPanel_AfterMove);
            }
        }

        private void Rotate()
        {
            GridPos.Orientation newOrientation = GridPos_AfterMove.orientation == GridPos.Orientation.Up ? GridPos.Orientation.Right : GridPos.Orientation.Up;

            List<GridPos> newRealPositions = new List<GridPos>();
            foreach (GridPos gp in OccupiedPositionsInBagPanel_AfterMove)
            {
                GridPos newLocalGrid = GridPos.RotateGridPos(gp - GridPos_AfterMove, GridPos_AfterMove.orientation == GridPos.Orientation.Up ? GridPos.Orientation.Right : GridPos.Orientation.Left);
                GridPos newRealGrid = newLocalGrid + GridPos_AfterMove;
                newRealPositions.Add(newRealGrid);
            }

            OccupiedPositionsInBagPanel_AfterMove.Clear();
            OccupiedPositionsInBagPanel_AfterMove = newRealPositions;

            Data.GridPos = new GridPos(GridPos_AfterMove.x, GridPos_AfterMove.z, newOrientation);
            Data.OccupiedGridPositions = newRealPositions;

            Initialize(Data, true);
        }

        #region IDraggable

        public void DragComponent_OnMouseDown()
        {
            BagManager.Instance.BagInfo.PickUpItem(Data);
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

            int x_delta = x - GridPos_AfterMove.x;
            int y_delta = y - GridPos_AfterMove.z;

            if (x_delta != 0 || y_delta != 0)
            {
                List<GridPos> newRealPositions = new List<GridPos>();
                GridPos_AfterMove = new GridPos(x, y, GridPos_AfterMove.orientation);
                foreach (GridPos gp in OccupiedPositionsInBagPanel_AfterMove)
                {
                    GridPos newRealGrid = new GridPos(gp.x + x_delta, gp.z + y_delta, gp.orientation);
                    newRealPositions.Add(newRealGrid);
                }

                OccupiedPositionsInBagPanel_AfterMove.Clear();
                OccupiedPositionsInBagPanel_AfterMove = newRealPositions;
            }
        }

        public void DragComponent_OnMouseUp(DragAreaTypes dragAreaTypes)
        {
            Data.GridPos = GridPos_AfterMove;
            BagManager.Instance.BagInfo.RemoveItem(Data);
            if (dragAreaTypes == DragAreaTypes.Bag)
            {
                bool suc = BagManager.Instance.BagInfo.TryAddItem(Data.Clone(), GridPos_AfterMove.orientation, CloneVariantUtils.List(OccupiedPositionsInBagPanel_AfterMove));
                if (!suc)
                {
                    BagManager.Instance.BagInfo.TryAddItem(Data.Clone(), Data.GridPos.orientation, CloneVariantUtils.List(OccupiedPositionsInBagPanel));
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
            MechaComponentBase mcb = MechaComponentBase.BaseInitialize(Data.MechaComponentInfo.Clone(), BattleManager.Instance.PlayerMecha);
            GridPos gp = ClientUtils.GetGridPosByMousePos(BattleManager.Instance.PlayerMecha.transform, Vector3.up, GameManager.GridSize);
            mcb.SetGridPosition(gp);
            BattleManager.Instance.PlayerMecha.AddMechaComponent(mcb);
            DragManager.Instance.CancelCurrentDrag();
            DragManager.Instance.CurrentDrag = mcb.Draggable;
            mcb.Draggable.IsOnDrag = true;
            BagManager.Instance.BagInfo.RemoveItem(Data);
            PoolRecycle();
        }

        #endregion
    }
}