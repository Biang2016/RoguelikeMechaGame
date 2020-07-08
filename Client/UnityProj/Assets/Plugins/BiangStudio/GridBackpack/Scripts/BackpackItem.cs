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
        }

        public Backpack Backpack;
        public InventoryItem InventoryItem;
        private Draggable Draggable;
        private RectTransform PanelRectTransform => (RectTransform) Backpack.BackpackPanel.ItemContainer;

        [SerializeField] private Image Image;

        [SerializeField] private BackpackItemGridHitBoxRoot BackpackItemGridHitBoxRoot;

        private RectTransform RectTransform => (RectTransform) transform;

        private Vector2 size;
        private Vector2 sizeRev;

        private void Awake()
        {
            Draggable = GetComponent<Draggable>();
        }

        public void Initialize(Backpack backpack, InventoryItem inventoryItem)
        {
            Backpack = backpack;
            //Image.sprite = BackpackManager.Instance.GetBackpackItemSprite(Data.ItemContentInfo.ItemSpriteKey);
            SetInventoryItem(inventoryItem);
            size = new Vector2(InventoryItem.BoundingRect.size.x * Backpack.GridSize, InventoryItem.BoundingRect.size.z * Backpack.GridSize);
            sizeRev = new Vector2(size.y, size.x);
            PutDown();
        }

        public void SetInventoryItem(InventoryItem inventoryItem)
        {
            InventoryItem = inventoryItem;
            InventoryItem.OnSetGridPosHandler = SetVirtualGridPos;
        }

        #region IDraggable

        private Vector2 dragStartLocalPos;
        private GridPosR dragStartGridPos_Matrix;
        private List<GridPos> dragStartOccupiedPositions_Matrix = new List<GridPos>();

        public void Draggable_OnMouseDown(DragArea dragArea, Collider collider)
        {
            PickUp();
        }

        private void PickUp()
        {
            dragStartLocalPos = RectTransform.anchoredPosition;
            dragStartOccupiedPositions_Matrix = InventoryItem.OccupiedGridPositions_Matrix.Clone();
            dragStartGridPos_Matrix = InventoryItem.GridPos_Matrix;
            InventoryItem.Inventory.PickUpItem(InventoryItem);
        }

        private void SetVirtualGridPos(GridPosR gridPos_World)
        {
            Backpack.BackpackPanel.BackpackItemVirtualOccupationRoot.Clear();
            foreach (GridPos gp_matrix in InventoryItem.OccupiedGridPositions_Matrix)
            {
                BackpackVirtualOccupationQuad quad = Backpack.CreateBackpackItemVirtualOccupationQuad(Backpack.BackpackPanel.BackpackItemVirtualOccupationRoot.transform);
                quad.Init(InventoryItem.Inventory.GridSize, gp_matrix, InventoryItem.Inventory);
                Backpack.BackpackPanel.BackpackItemVirtualOccupationRoot.backpackVirtualOccupationQuads.Add(quad);
            }
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
                    Vector2 diffLocal = RectTransform.parent.InverseTransformVector(diffFromStart);
                    Vector2 currentLocalPos = dragStartLocalPos + diffLocal;
                    GridPosR diff_world = GridPos.GetGridPosByPointXY(diffLocal, Backpack.GridSize);
                    diff_world.orientation = InventoryItem.GridPos_Matrix.orientation;
                    GridPosR diff_matrix = Backpack.CoordinateTransformationHandler_FromPosToMatrixIndex(diff_world);
                    GridPosR gp_matrix = dragStartGridPos_Matrix + diff_matrix;
                    gp_matrix.orientation = InventoryItem.GridPos_Matrix.orientation;
                    InventoryItem.SetGridPosition(gp_matrix);
                    RectTransform.anchoredPosition = currentLocalPos;
                }
            }
            else
            {
                if (Backpack.DragItemOutBackpackCallback != null)
                {
                    Backpack.BackpackPanel.BackpackItemVirtualOccupationRoot.Clear();
                    if (Backpack.DragItemOutBackpackCallback.Invoke(this))
                    {
                        InventoryItem.GridPos_Matrix = dragStartGridPos_Matrix;
                        Backpack.RemoveItem(InventoryItem);
                        PoolRecycle();
                    }
                }
            }
        }

        private void Rotate()
        {
            InventoryItem.GridPos_Matrix.orientation = GridPosR.RotateOrientationClockwise90(InventoryItem.GridPos_Matrix.orientation);
            InventoryItem.SetGridPosition(InventoryItem.GridPos_Matrix);
            dragStartLocalPos += new Vector2(InventoryItem.BoundingRect.x_min * Backpack.GridSize, -InventoryItem.BoundingRect.z_min * Backpack.GridSize) - RectTransform.anchoredPosition;
            PutDown();
        }

        public void Draggable_OnMouseUp(DragArea dragArea, Vector3 diffFromStart, Vector3 deltaFromLastFrame)
        {
            Backpack.BackpackPanel.BackpackItemVirtualOccupationRoot.Clear();
            if (dragArea.Equals(Backpack.DragArea))
            {
                if (Backpack.CheckSpaceAvailable(InventoryItem.OccupiedGridPositions_Matrix, GridPos.Zero))
                {
                    Backpack.MoveItem(dragStartOccupiedPositions_Matrix, InventoryItem.OccupiedGridPositions_Matrix);
                }
                else
                {
                    InventoryItem.GridPos_Matrix = dragStartGridPos_Matrix;
                }

                PutDown();
            }
        }

        private void PutDown()
        {
            int UI_Pos_X = InventoryItem.BoundingRect.x_min * Backpack.GridSize;
            int UI_Pos_Z = -InventoryItem.BoundingRect.z_min * Backpack.GridSize;

            bool isRotated = InventoryItem.GridPos_Matrix.orientation == GridPosR.Orientation.Right || InventoryItem.GridPos_Matrix.orientation == GridPosR.Orientation.Left;
            Image.rectTransform.sizeDelta = size;
            Image.rectTransform.rotation = Quaternion.Euler(0, 0, 90f * (int) InventoryItem.GridPos_Matrix.orientation);

            RectTransform.sizeDelta = isRotated ? sizeRev : size;

            RectTransform.anchoredPosition = new Vector2(UI_Pos_X, UI_Pos_Z);
            BackpackItemGridHitBoxRoot.Initialize(Backpack, InventoryItem);
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