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

        private RectTransform RectTransform => (RectTransform) transform;

        internal GridPos GridPos_Moving;
        public List<GridPos> OccupiedPositionsInBagPanel_Moving;

        private float _dragComponentDragMinDistance;
        private float _dragComponentDragMaxDistance;

        public void Initialize(BagItemInfo data, bool moving)
        {
            Data = data;
            Image.sprite = BagManager.Instance.BagItemSpriteDict[data.BagItemContentInfo.BagItemSpriteKey];
            GridPos_Moving = Data.GridPos;

            Vector2 size = new Vector2(Data.Size.width * BagManager.Instance.BagItemGridSize, Data.Size.height * BagManager.Instance.BagItemGridSize);
            Vector2 sizeRev = new Vector2(size.y, size.x);
            bool isRotated = GridPos_Moving.orientation == GridPos.Orientation.Right || GridPos_Moving.orientation == GridPos.Orientation.Left;

            BagItemGridHitBoxes.Initialize(Data.OccupiedGridPositions, Data.GridPos);
            Image.rectTransform.sizeDelta = size;
            if (isRotated)
            {
                RectTransform.sizeDelta = sizeRev;
                RectTransform.rotation = Quaternion.Euler(0, 0, 90f);
            }
            else
            {
                RectTransform.sizeDelta = size;
                RectTransform.rotation = Quaternion.Euler(0, 0, 0f);
            }

            RectTransform.anchoredPosition = new Vector2(GridPos_Moving.x * BagManager.Instance.BagItemGridSize, -GridPos_Moving.z * BagManager.Instance.BagItemGridSize);
            OccupiedPositionsInBagPanel_Moving = CloneVariantUtils.List(Data.OccupiedGridPositions);
        }

        private void Rotate()
        {
            GridPos.Orientation newOrientation = GridPos_Moving.orientation == GridPos.Orientation.Up ? GridPos.Orientation.Right : GridPos.Orientation.Up;

            List<GridPos> newRealPositions = new List<GridPos>();
            foreach (GridPos gp in OccupiedPositionsInBagPanel_Moving)
            {
                GridPos newLocalGrid = GridPos.RotateGridPos(gp - GridPos_Moving, GridPos_Moving.orientation == GridPos.Orientation.Up ? GridPos.Orientation.Right : GridPos.Orientation.Left);
                GridPos newRealGrid = newLocalGrid + GridPos_Moving;
                newRealPositions.Add(newRealGrid);
            }

            OccupiedPositionsInBagPanel_Moving.Clear();
            OccupiedPositionsInBagPanel_Moving = newRealPositions;

            Data.GridPos = new GridPos(GridPos_Moving.x, GridPos_Moving.z, newOrientation);
            Data.OccupiedGridPositions = newRealPositions;

            //Initialize(Data, true);
        }

        #region IDraggable

        public GridPos LastPickedUpHitBoxGridPos;

        public void DragComponent_OnMouseDown(Collider collider)
        {
            foreach (BagItemGridHitBox hitBox in BagItemGridHitBoxes.bagItemGridHitBoxes)
            {
                if (collider == hitBox.BoxCollider)
                {
                    LastPickedUpHitBoxGridPos = hitBox.LocalGridPos + GridPos_Moving;
                    BagManager.Instance.BagInfo.PickUpItem(Data);
                    return;
                }
            }
        }

        public void MoveBaseOnHitBox(GridPos hitBoxTargetPos)
        {
            GridPos targetGP = hitBoxTargetPos - LastPickedUpHitBoxGridPos + GridPos_Moving;
            Debug.Log(targetGP.ToShortString());
            MoveToGridPos(targetGP);
        }

        private void MoveToGridPos(GridPos targetGP)
        {
            GridPos diff = targetGP - GridPos_Moving;
            if (diff.x != 0 || diff.z != 0)
            {
                bool suc = BagManager.Instance.BagInfo.CheckSpaceAvailable(DragManager.Instance.CurrentDrag_BagItem.OccupiedPositionsInBagPanel_Moving, diff);
                if (suc)
                {
                    int UI_Pos_X = targetGP.x * BagManager.Instance.BagItemGridSize;
                    int UI_Pos_Z = -targetGP.z * BagManager.Instance.BagItemGridSize;
                    RectTransform.anchoredPosition = new Vector2(UI_Pos_X, UI_Pos_Z);

                    GridPos_Moving = targetGP;

                    for (int i = 0; i < OccupiedPositionsInBagPanel_Moving.Count; i++)
                    {
                        OccupiedPositionsInBagPanel_Moving[i] += diff;
                    }

                    LastPickedUpHitBoxGridPos += diff;

                    BagItemGridHitBoxes.Initialize(OccupiedPositionsInBagPanel_Moving, GridPos_Moving);
                }
                else
                {
                    int a = 0;
                }
            }
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

                    break;
                }
            }
        }

        public void DragComponent_OnMouseUp(DragAreaTypes dragAreaTypes)
        {
            if (dragAreaTypes == DragAreaTypes.Bag)
            {
                bool suc = BagManager.Instance.BagInfo.CheckSpaceAvailable(OccupiedPositionsInBagPanel_Moving, GridPos.Zero);
                if (suc)
                {
                    BagManager.Instance.BagInfo.MoveItem(Data.OccupiedGridPositions, OccupiedPositionsInBagPanel_Moving);
                    Data.GridPos = GridPos_Moving;
                    Data.OccupiedGridPositions = CloneVariantUtils.List(OccupiedPositionsInBagPanel_Moving);
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