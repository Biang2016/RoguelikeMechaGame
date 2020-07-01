using System.Collections.Generic;
using GameCore;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class BagItem : PoolObject, IDraggable
    {
        [ShowInInspector] private BagItemInfo Data;

        [SerializeField] private Image Image;

        [SerializeField] private BagItemGridHitBoxes BagItemGridHitBoxes;

        private RectTransform RectTransform => (RectTransform) transform;

        [ShowInInspector] private GridPosR GridPos_Moving;

        [ShowInInspector] private List<GridPos> OccupiedPositionsInBagPanel_Moving;

        private float _dragComponentDragMinDistance;
        private float _dragComponentDragMaxDistance;

        [ShowInInspector] private Vector2 size;

        [ShowInInspector] private Vector2 sizeRev;

        public void Initialize(BagItemInfo data)
        {
            Data = data;
            Image.sprite = BagManager.Instance.BagItemSpriteDict[Data.BagItemContentInfo.BagItemSpriteKey];
            GridPos_Moving = Data.GridPos;
            OccupiedPositionsInBagPanel_Moving = CloneVariantUtils.List(Data.OccupiedGridPositions);
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

        public void DragComponent_OnMouseDown(Collider collider)
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

        public void DragComponent_OnMousePressed(DragAreaTypes dragAreaTypes)
        {
            switch (dragAreaTypes)
            {
                case DragAreaTypes.Bag:
                {
                    if (ControlManager.Instance.Building_RotateItem.Down)
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
            List<GridPos> newRealPositions = CloneVariantUtils.List(OccupiedPositionsInBagPanel_Moving);
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

        public void DragComponent_OnMouseUp(DragAreaTypes dragAreaTypes)
        {
            if (dragAreaTypes == DragAreaTypes.Bag)
            {
                if (BagManager.Instance.BagInfo.CheckSpaceAvailable(OccupiedPositionsInBagPanel_Moving, GridPos.Zero))
                {
                    BagManager.Instance.BagInfo.MoveItem(Data.OccupiedGridPositions, OccupiedPositionsInBagPanel_Moving);
                    Data.GridPos = GridPos_Moving;
                    Data.OccupiedGridPositions = CloneVariantUtils.List(OccupiedPositionsInBagPanel_Moving);
                    RefreshView();
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
            // todo 应该做成分治的，没有很好的办法剥离出去，暂时这样
            switch (Data.BagItemContentInfo)
            {
                case MechaComponentInfo mechaComponentInfo:
                {
                    MechaComponentBase mcb = MechaComponentBase.BaseInitialize(mechaComponentInfo.Clone(), BattleManager.Instance.PlayerMecha);
                    GridPos gp = ClientUtils.GetGridPosByMousePos(BattleManager.Instance.PlayerMecha.transform, Vector3.up, GameManager.GridSize);
                    mcb.SetGridPosition(gp);
                    BattleManager.Instance.PlayerMecha.AddMechaComponent(mcb);
                    DragManager.Instance.CancelCurrentDrag();
                    DragManager.Instance.CurrentDrag = mcb.Draggable;
                    mcb.Draggable.SetOnDrag(true, null);
                    BagManager.Instance.BagInfo.RemoveItem(Data);
                    PoolRecycle();
                    break;
                }
            }
        }

        #endregion
    }
}