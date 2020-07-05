using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ObjectPool;
using BiangStudio.ShapedInventory;
using UnityEngine;
using UnityEngine.UI;

namespace BiangStudio.GridBackpack
{
    public class BackpackItem : PoolObject, IDraggable
    {
        public override void PoolRecycle()
        {
            base.PoolRecycle();
            Backpack = null;
            Data = null;
            OccupiedPositionsInBackpackPanel_Moving = null;
        }

        public Backpack Backpack;
        public InventoryItem Data;

        [SerializeField] private Image Image;
        [SerializeField] private BackpackItemGridHitBoxRoot BackpackItemGridHitBoxRoot;

        private RectTransform RectTransform => (RectTransform) transform;

        private GridPosR GridPos_Moving;

        private List<GridPos> OccupiedPositionsInBackpackPanel_Moving;

        private float _dragComponentDragMinDistance;
        private float _dragComponentDragMaxDistance;

        private Vector2 size;

        private Vector2 sizeRev;

        public void Initialize(Backpack backpack, InventoryItem data)
        {
            Backpack = backpack;
            Data = data;
            //Image.sprite = BackpackManager.Instance.GetBackpackItemSprite(Data.ItemContentInfo.ItemSpriteKey);
            GridPos_Moving = Data.GridPos;
            OccupiedPositionsInBackpackPanel_Moving = Data.OccupiedGridPositions.Clone();
            size = new Vector2(Data.BoundingRect.size.x * Backpack.GridSize, Data.BoundingRect.size.z * Backpack.GridSize);
            sizeRev = new Vector2(size.y, size.x);
            RefreshView();
        }

        private void RefreshView()
        {
            int UI_Pos_X = GridPos_Moving.x * Backpack.GridSize;
            int UI_Pos_Z = -GridPos_Moving.z * Backpack.GridSize;

            bool isRotated = GridPos_Moving.orientation == GridPosR.Orientation.Right || GridPos_Moving.orientation == GridPosR.Orientation.Left;
            Image.rectTransform.sizeDelta = size;
            Image.rectTransform.rotation = Quaternion.Euler(0, 0, 90f * (int) GridPos_Moving.orientation);

            if (isRotated)
            {
                RectTransform.sizeDelta = sizeRev;
            }
            else
            {
                RectTransform.sizeDelta = size;
            }

            RectTransform.anchoredPosition = new Vector2(UI_Pos_X, UI_Pos_Z);
            BackpackItemGridHitBoxRoot.Initialize(Backpack, OccupiedPositionsInBackpackPanel_Moving, GridPos_Moving);
        }

        #region IDraggable

        private GridPos lastPickedUpHitBoxGridPos;

        public void Draggable_OnMouseDown(DragArea dragArea, Collider collider)
        {
            BackpackItemGridHitBox hitBox = BackpackItemGridHitBoxRoot.FindHitBox(collider);
            if (hitBox)
            {
                lastPickedUpHitBoxGridPos = hitBox.LocalGridPos + (GridPos) GridPos_Moving;
                Backpack.PickUpItem(Data);
            }
        }

        public void MoveBaseOnHitBox(GridPos hitBoxTargetPos)
        {
            GridPosR targetGPR = hitBoxTargetPos - lastPickedUpHitBoxGridPos + (GridPos) GridPos_Moving;
            targetGPR.orientation = GridPos_Moving.orientation;
            SetGridPosition(targetGPR);
        }

        private void SetGridPosition(GridPosR targetGPR)
        {
            GridPos diff = targetGPR - GridPos_Moving;
            if (diff.x != 0 || diff.z != 0)
            {
                if (Backpack.CheckSpaceAvailable(OccupiedPositionsInBackpackPanel_Moving, diff))
                {
                    GridPos_Moving = targetGPR;
                    for (int i = 0; i < OccupiedPositionsInBackpackPanel_Moving.Count; i++)
                    {
                        OccupiedPositionsInBackpackPanel_Moving[i] += diff;
                    }

                    lastPickedUpHitBoxGridPos += diff;
                    RefreshView();
                }
            }
        }

        public void Draggable_OnMousePressed(DragArea dragArea)
        {
            if (dragArea.Equals(Backpack.DragArea))
            {
                if (Backpack.RotateItemKeyDownHandler != null && Backpack.RotateItemKeyDownHandler.Invoke())
                {
                    Rotate();
                }
            }
        }

        private void Rotate()
        {
            int checkTime = 0;
            GridPosR.Orientation newOrientation = GridPos_Moving.orientation;
            List<GridPos> newRealPositions = OccupiedPositionsInBackpackPanel_Moving.Clone();
            while (checkTime < 4)
            {
                newOrientation = GridPosR.RotateOrientationClockwise90(newOrientation);
                List<GridPos> _newRealPositions = new List<GridPos>();
                foreach (GridPos gp in newRealPositions)
                {
                    GridPos newLocalGrid = GridPos.RotateGridPos(gp - lastPickedUpHitBoxGridPos, (GridPosR.Orientation) ((newOrientation - GridPos_Moving.orientation + 4) % 4));
                    GridPos newRealGrid = newLocalGrid + lastPickedUpHitBoxGridPos;
                    _newRealPositions.Add(newRealGrid);
                }

                if (Backpack.CheckSpaceAvailable(_newRealPositions, GridPos.Zero))
                {
                    GridPos_Moving.orientation = newOrientation;
                    OccupiedPositionsInBackpackPanel_Moving = _newRealPositions;
                    GridPos_Moving.x = _newRealPositions.GetBoundingRectFromListGridPos().x_min;
                    GridPos_Moving.z = _newRealPositions.GetBoundingRectFromListGridPos().z_min;
                    RefreshView();
                    return;
                }
                else
                {
                    checkTime++;
                }
            }
        }

        public void Draggable_OnMouseUp(DragArea dragArea)
        {
            if (dragArea.Equals(Backpack.DragArea))
            {
                if (Backpack.CheckSpaceAvailable(OccupiedPositionsInBackpackPanel_Moving, GridPos.Zero))
                {
                    Backpack.MoveItem(Data.OccupiedGridPositions, OccupiedPositionsInBackpackPanel_Moving);
                    Data.GridPos = GridPos_Moving;
                    Data.OccupiedGridPositions = OccupiedPositionsInBackpackPanel_Moving.Clone();
                    RefreshView();
                }
            }
        }

        public void Draggable_SetStates(ref bool canDrag, ref DragArea dragFrom)
        {
            canDrag = true;
            dragFrom = Backpack.DragArea;
        }

        float IDraggable.Draggable_DragMinDistance => 0f;

        float IDraggable.Draggable_DragMaxDistance => 99f;

        public void Draggable_DragOutEffects()
        {
            Backpack.DragItemOutBackpackCallback?.Invoke(this);
        }

        #endregion
    }
}