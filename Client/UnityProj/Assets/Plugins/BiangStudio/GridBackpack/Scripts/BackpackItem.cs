﻿using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ObjectPool;
using UnityEngine;
using UnityEngine.UI;

namespace BiangStudio.GridBackpack
{
    public class BackpackItem : PoolObject, IDraggable
    {
        public BackpackItemInfo Data;

        [SerializeField] private Image Image;

        [SerializeField] private BackpackItemGridHitBoxRoot BackpackItemGridHitBoxes;

        private RectTransform RectTransform => (RectTransform) transform;

        private GridPosR GridPos_Moving;

        private List<GridPos> OccupiedPositionsInBackpackPanel_Moving;

        private float _dragComponentDragMinDistance;
        private float _dragComponentDragMaxDistance;

        private Vector2 size;

        private Vector2 sizeRev;

        public void Initialize(BackpackItemInfo data)
        {
            Data = data;
            Image.sprite = BackpackManager.Instance.GetBackpackItemSprite(Data.BackpackItemContentInfo.BackpackItemSpriteKey);
            GridPos_Moving = Data.GridPos;
            OccupiedPositionsInBackpackPanel_Moving = Data.OccupiedGridPositions.Clone();
            size = new Vector2(Data.BoundingRect.size.x * BackpackManager.Instance.BackpackItemGridSize, Data.BoundingRect.size.z * BackpackManager.Instance.BackpackItemGridSize);
            sizeRev = new Vector2(size.y, size.x);
            RefreshView();
        }

        private void RefreshView()
        {
            int UI_Pos_X = GridPos_Moving.x * BackpackManager.Instance.BackpackItemGridSize;
            int UI_Pos_Z = -GridPos_Moving.z * BackpackManager.Instance.BackpackItemGridSize;

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
            BackpackItemGridHitBoxes.Initialize(OccupiedPositionsInBackpackPanel_Moving, GridPos_Moving);
        }

        #region IDraggable

        private GridPos lastPickedUpHitBoxGridPos;

        public void Draggable_OnMouseDown(string dragAreaName, Collider collider)
        {
            BackpackItemGridHitBox hitBox = BackpackItemGridHitBoxes.FindHitBox(collider);
            if (hitBox)
            {
                lastPickedUpHitBoxGridPos = hitBox.LocalGridPos + (GridPos) GridPos_Moving;
                BackpackManager.Instance.BackpackInfo.PickUpItem(Data);
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
                if (BackpackManager.Instance.BackpackInfo.CheckSpaceAvailable(OccupiedPositionsInBackpackPanel_Moving, diff))
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

        public void Draggable_OnMousePressed(string dragAreaTypes)
        {
            switch (dragAreaTypes)
            {
                case DragAreaDefines.Backpack:
                {
                    if (BackpackManager.Instance.RotateItemKeyDownHandler != null && BackpackManager.Instance.RotateItemKeyDownHandler.Invoke())
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

                if (BackpackManager.Instance.BackpackInfo.CheckSpaceAvailable(_newRealPositions, GridPos.Zero))
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

        public void Draggable_OnMouseUp(string dragAreaTypes)
        {
            if (dragAreaTypes == DragAreaDefines.Backpack)
            {
                if (BackpackManager.Instance.BackpackInfo.CheckSpaceAvailable(OccupiedPositionsInBackpackPanel_Moving, GridPos.Zero))
                {
                    BackpackManager.Instance.BackpackInfo.MoveItem(Data.OccupiedGridPositions, OccupiedPositionsInBackpackPanel_Moving);
                    Data.GridPos = GridPos_Moving;
                    Data.OccupiedGridPositions = OccupiedPositionsInBackpackPanel_Moving.Clone();
                    RefreshView();
                }
            }
        }

        public void Draggable_SetStates(ref bool canDrag, ref string dragFrom)
        {
            canDrag = true;
            dragFrom = DragAreaDefines.Backpack;
        }

        float IDraggable.Draggable_DragMinDistance => 0f;

        float IDraggable.Draggable_DragMaxDistance => 99f;

        public void Draggable_DragOutEffects()
        {
            BackpackManager.Instance.DragItemOutBackpackCallback?.Invoke(this);
        }

        #endregion
    }
}