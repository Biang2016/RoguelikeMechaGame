using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ObjectPool;
using UnityEngine;
using UnityEngine.UI;

namespace BiangStudio.GridBag
{
    public class BagItem : PoolObject, IDraggable
    {
        public BagItemInfo Data;

        [SerializeField] private Image Image;

        [SerializeField] private BagItemGridHitBoxRoot BagItemGridHitBoxes;

        private RectTransform RectTransform => (RectTransform) transform;

        private GridPosR GridPos_Moving;

        private List<GridPos> OccupiedPositionsInBagPanel_Moving;

        private float _dragComponentDragMinDistance;
        private float _dragComponentDragMaxDistance;

        private Vector2 size;

        private Vector2 sizeRev;

        public void Initialize(BagItemInfo data)
        {
            Data = data;
            Image.sprite = BagManager.Instance.GetBagItemSprite(Data.BagItemContentInfo.BagItemSpriteKey);
            GridPos_Moving = Data.GridPos;
            OccupiedPositionsInBagPanel_Moving = Data.OccupiedGridPositions.Clone();
            size = new Vector2(Data.BoundingRect.size.x * BagManager.Instance.BagItemGridSize, Data.BoundingRect.size.z * BagManager.Instance.BagItemGridSize);
            sizeRev = new Vector2(size.y, size.x);
            RefreshView();
        }

        private void RefreshView()
        {
            int UI_Pos_X = GridPos_Moving.x * BagManager.Instance.BagItemGridSize;
            int UI_Pos_Z = -GridPos_Moving.z * BagManager.Instance.BagItemGridSize;

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
            BagItemGridHitBoxes.Initialize(OccupiedPositionsInBagPanel_Moving, GridPos_Moving);
        }

        #region IDraggable

        private GridPos lastPickedUpHitBoxGridPos;

        public void Draggable_OnMouseDown(string dragAreaName, Collider collider)
        {
            foreach (BagItemGridHitBox hitBox in BagItemGridHitBoxes.bagItemGridHitBoxes)
            {
                if (collider == hitBox.BoxCollider)
                {
                    lastPickedUpHitBoxGridPos = hitBox.LocalGridPos + (GridPos) GridPos_Moving;
                    BagManager.Instance.BagInfo.PickUpItem(Data);
                    return;
                }
            }
        }

        public void MoveBaseOnHitBox(GridPos hitBoxTargetPos)
        {
            GridPosR targetGPR = hitBoxTargetPos - lastPickedUpHitBoxGridPos + (GridPos) GridPos_Moving;
            targetGPR.orientation = GridPos_Moving.orientation;
            MoveToGridPos(targetGPR);
        }

        private void MoveToGridPos(GridPosR targetGPR)
        {
            GridPos diff = targetGPR - GridPos_Moving;
            if (diff.x != 0 || diff.z != 0)
            {
                if (BagManager.Instance.BagInfo.CheckSpaceAvailable(OccupiedPositionsInBagPanel_Moving, diff))
                {
                    GridPos_Moving = targetGPR;
                    for (int i = 0; i < OccupiedPositionsInBagPanel_Moving.Count; i++)
                    {
                        OccupiedPositionsInBagPanel_Moving[i] += diff;
                    }

                    lastPickedUpHitBoxGridPos += diff;
                    RefreshView();
                }
            }
        }

        public void Draggable_OnMousePressed(string dragAreaTypes)
        {
            switch (dragAreaTypes)
            {
                case DragAreaDefines.Bag:
                {
                    if (BagManager.Instance.RotateItemKeyDownHandler != null && BagManager.Instance.RotateItemKeyDownHandler.Invoke())
                    {
                        Rotate();
                    }

                    break;
                }
            }
        }

        private void Rotate()
        {
            int checkTime = 0;
            GridPosR.Orientation newOrientation = GridPos_Moving.orientation;
            List<GridPos> newRealPositions = OccupiedPositionsInBagPanel_Moving.Clone();
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

                if (BagManager.Instance.BagInfo.CheckSpaceAvailable(_newRealPositions, GridPos.Zero))
                {
                    GridPos_Moving.orientation = newOrientation;
                    OccupiedPositionsInBagPanel_Moving = _newRealPositions;
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

        public void Draggable_OnMouseUp(string dragAreaTypes)
        {
            if (dragAreaTypes == DragAreaDefines.Bag)
            {
                if (BagManager.Instance.BagInfo.CheckSpaceAvailable(OccupiedPositionsInBagPanel_Moving, GridPos.Zero))
                {
                    BagManager.Instance.BagInfo.MoveItem(Data.OccupiedGridPositions, OccupiedPositionsInBagPanel_Moving);
                    Data.GridPos = GridPos_Moving;
                    Data.OccupiedGridPositions = OccupiedPositionsInBagPanel_Moving.Clone();
                    RefreshView();
                }
            }
        }

        public void Draggable_SetStates(ref bool canDrag, ref string dragFrom)
        {
            canDrag = true;
            dragFrom = DragAreaDefines.Bag;
        }

        float IDraggable.Draggable_DragMinDistance => 0f;

        float IDraggable.Draggable_DragMaxDistance => 99f;

        public void Draggable_DragOutEffects()
        {
            BagManager.Instance.DragItemOutBagCallback?.Invoke(this);
        }

        #endregion
    }
}