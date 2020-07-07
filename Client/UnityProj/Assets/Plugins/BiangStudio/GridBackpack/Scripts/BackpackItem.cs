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
            InventoryItem = null;
            OccupiedPositionsInBackpackPanel_Moving = null;
        }

        public Backpack Backpack;
        public InventoryItem InventoryItem;
        private Draggable Draggable;
        private RectTransform PanelRectTransform => (RectTransform) Backpack.BackpackPanel.ItemContainer;

        [SerializeField]
        private Image Image;

        [SerializeField]
        private BackpackItemGridHitBoxRoot BackpackItemGridHitBoxRoot;

        private RectTransform RectTransform => (RectTransform) transform;

        private GridPosR GridPos_Moving;
        private List<GridPos> OccupiedPositionsInBackpackPanel_Moving;

        private Vector2 size;
        private Vector2 sizeRev;

        private void Awake()
        {
            Draggable = GetComponent<Draggable>();
        }

        public void Initialize(Backpack backpack, InventoryItem inventoryItem)
        {
            Backpack = backpack;
            InventoryItem = inventoryItem;
            //Image.sprite = BackpackManager.Instance.GetBackpackItemSprite(Data.ItemContentInfo.ItemSpriteKey);
            GridPos_Moving = InventoryItem.GridPos_Matrix;
            OccupiedPositionsInBackpackPanel_Moving = InventoryItem.OccupiedGridPositions_Matrix.Clone();
            size = new Vector2(InventoryItem.BoundingRect.size.x * Backpack.GridSize, InventoryItem.BoundingRect.size.z * Backpack.GridSize);
            sizeRev = new Vector2(size.y, size.x);
            RefreshView();
        }

        public void SetInventoryItem(InventoryItem inventoryItem)
        {
            InventoryItem = inventoryItem;
            InventoryItem.OnSetGridPosHandler = SetGridPos;
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

        private Vector3 dragStartLocalPos;

        public void Draggable_OnMouseDown(DragArea dragArea, Collider collider)
        {
            dragStartLocalPos = RectTransform.anchoredPosition;
        }

        private void SetVirtualGridPos(GridPosR targetGPR)
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

                    // lastPickedUpHitBoxGridPos += diff;
                    RefreshView();
                }
            }
        }

        private void SetGridPos(GridPosR gridPos_World)
        {
            // todo 显示合法位置虚框

        }

        public void Draggable_OnMousePressed(DragArea dragArea, Vector3 diffFromStart, Vector3 deltaFromLastFrame)
        {
            if (dragArea.Equals(Backpack.DragArea))
            {
                if (Backpack.RotateItemKeyDownHandler != null && Backpack.RotateItemKeyDownHandler.Invoke())
                {
                    Rotate();
                }

                if (diffFromStart.magnitude <= Draggable_DragMinDistance)
                {
                    //不动
                }
                else
                {
                    Debug.Log(diffFromStart);
                    Vector3 currentLocalPos = dragStartLocalPos + RectTransform.parent.InverseTransformVector(diffFromStart);
                    GridPosR gp_world = GridPos.GetGridPosByPointXY(currentLocalPos, Backpack.GridSize);
                    gp_world.orientation = InventoryItem.GridPos_Matrix.orientation;
                    GridPosR gp_matrix = Backpack.CoordinateTransformationHandler_FromPosToMatrixIndex(gp_world);
                    InventoryItem.SetGridPosition(gp_matrix);
                    // if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    //     PanelRectTransform,
                    //     Draggable.MyDragProcessor.CurrentMousePosition_World,
                    //     Draggable.MyDragProcessor.Camera,
                    //     out Vector2 anchoredPos))
                    // {
                    //     anchoredPos.x += PanelRectTransform.rect.width / 2f;
                    //     anchoredPos.y -= PanelRectTransform.rect.height / 2f;
                    //     int grid_X = Mathf.FloorToInt((anchoredPos.x) / Backpack.GridSize);
                    //     int grid_Z = Mathf.FloorToInt((-anchoredPos.y) / Backpack.GridSize);
                    //     MoveBaseOnHitBox(new GridPos(grid_X, grid_Z));
                    // }
                }
            }
            else
            {
                Backpack.DragItemOutBackpackCallback?.Invoke(this);
            }
        }

        private void Rotate()
        {
            // int checkTime = 0;
            // GridPosR.Orientation newOrientation = GridPos_Moving.orientation;
            // List<GridPos> newRealPositions = OccupiedPositionsInBackpackPanel_Moving.Clone();
            // while (checkTime < 4)
            // {
            //     newOrientation = GridPosR.RotateOrientationClockwise90(newOrientation);
            //     List<GridPos> _newRealPositions = new List<GridPos>();
            //     foreach (GridPos gp in newRealPositions)
            //     {
            //         GridPos newLocalGrid = GridPos.RotateGridPos(gp - lastPickedUpHitBoxGridPos, (GridPosR.Orientation) ((newOrientation - GridPos_Moving.orientation + 4) % 4));
            //         GridPos newRealGrid = newLocalGrid + lastPickedUpHitBoxGridPos;
            //         _newRealPositions.Add(newRealGrid);
            //     }
            //
            //     if (Backpack.CheckSpaceAvailable(_newRealPositions, GridPos.Zero))
            //     {
            //         GridPos_Moving.orientation = newOrientation;
            //         OccupiedPositionsInBackpackPanel_Moving = _newRealPositions;
            //         GridPos_Moving.x = _newRealPositions.GetBoundingRectFromListGridPos().x_min;
            //         GridPos_Moving.z = _newRealPositions.GetBoundingRectFromListGridPos().z_min;
            //         RefreshView();
            //         return;
            //     }
            //     else
            //     {
            //         checkTime++;
            //     }
            // }
        }

        public void Draggable_OnMouseUp(DragArea dragArea, Vector3 diffFromStart, Vector3 deltaFromLastFrame)
        {
            if (dragArea.Equals(Backpack.DragArea))
            {
                if (Backpack.CheckSpaceAvailable(OccupiedPositionsInBackpackPanel_Moving, GridPos.Zero))
                {
                    Backpack.MoveItem(InventoryItem.OccupiedGridPositions_Matrix, OccupiedPositionsInBackpackPanel_Moving);
                    InventoryItem.GridPos_Matrix = GridPos_Moving;
                    RefreshView();
                }
            }
        }

        public void Draggable_SetStates(ref bool canDrag, ref DragArea dragFrom)
        {
            canDrag = true;
            dragFrom = Backpack.DragArea;
        }

        public float Draggable_DragMinDistance => 0f;

        public float Draggable_DragMaxDistance => 99f;

        #endregion
    }
}