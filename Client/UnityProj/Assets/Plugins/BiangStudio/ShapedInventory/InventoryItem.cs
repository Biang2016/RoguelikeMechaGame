using System;
using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

namespace BiangStudio.ShapedInventory
{
    [Serializable]
    public class InventoryItem : IClone<InventoryItem>
    {
        public delegate void OnSetGridPosDelegate(GridPosR gridPos);

        public delegate void OnIsolatedDelegate(bool isolated);

        public delegate void OnConflictedDelegate(GridPos gridPos);

        public delegate void OnResetConflictDelegate();

        public delegate bool AmIRootItemInIsolationCalculationDelegate();

        private static int guidGenerator;

        [HideInInspector] public int GUID;

        public IInventoryItemContentInfo ItemContentInfo;

        public Inventory Inventory;

        public GridPosR GridPos_Matrix { get; private set; }
        public GridPosR GridPos_World { get; private set; }
        public List<GridPos> OccupiedGridPositions_Matrix; // x: col, z: row

        [HideInInspector] public GridRect BoundingRect;

        public OnSetGridPosDelegate OnSetGridPosHandler;
        public OnIsolatedDelegate OnIsolatedHandler;
        public OnConflictedDelegate OnConflictedHandler;
        public OnResetConflictDelegate OnResetConflictHandler;
        public AmIRootItemInIsolationCalculationDelegate AmIRootItemInIsolationCalculationHandler;

        public string ItemContentName
        {
            get
            {
                if (ItemContentInfo != null)
                {
                    return ItemContentInfo.ItemName;
                }
                else
                {
                    return "";
                }
            }
        }

        public InventoryItem(IInventoryItemContentInfo itemContentInfo, Inventory inventory)
        {
            GUID = guidGenerator;
            guidGenerator++;

            Inventory = inventory;
            ItemContentInfo = itemContentInfo;

            OccupiedGridPositions_Matrix = new List<GridPos>();
            foreach (GridPos gp in ItemContentInfo.OriginalOccupiedGridPositions)
            {
                GridPos gp_matrix = Inventory.CoordinateTransformationHandler_FromPosToMatrixIndex(gp);
                OccupiedGridPositions_Matrix.Add(gp_matrix);
            }

            RefreshSize();
        }

        public void SetGridPosition(GridPosR gp_matrix)
        {
            if (!gp_matrix.Equals(GridPos_Matrix))
            {
                foreach (GridPos gp in OccupiedGridPositions_Matrix)
                {
                    GridPos gp_rot = GridPos.RotateGridPos(gp - (GridPos) GridPos_Matrix, (GridPosR.Orientation) ((gp_matrix.orientation - GridPos_Matrix.orientation + 4) % 4));
                    GridPosR newGP = gp_matrix + (GridPosR) gp_rot;
                    if (newGP.x > Inventory.Columns)
                    {
                        SetGridPosition(new GridPosR(gp_matrix.x - 1, gp_matrix.z, gp_matrix.orientation));
                        return;
                    }

                    if (newGP.x < 0)
                    {
                        SetGridPosition(new GridPosR(gp_matrix.x + 1, gp_matrix.z, gp_matrix.orientation));
                        return;
                    }

                    if (newGP.z > Inventory.Rows)
                    {
                        SetGridPosition(new GridPosR(gp_matrix.x, gp_matrix.z - 1, gp_matrix.orientation));
                        return;
                    }

                    if (newGP.z < 0)
                    {
                        SetGridPosition(new GridPosR(gp_matrix.x, gp_matrix.z + 1, gp_matrix.orientation));
                        return;
                    }
                }

                GridPos_Matrix = gp_matrix;
                GridPos_World = Inventory.CoordinateTransformationHandler_FromMatrixIndexToPos(GridPos_Matrix);
                OnSetGridPosHandler?.Invoke(GridPos_World);
         }
        }

        public void RefreshSize()
        {
            BoundingRect = OccupiedGridPositions_Matrix.GetBoundingRectFromListGridPos();
        }

        public InventoryItem Clone()
        {
            InventoryItem ii = new InventoryItem(CloneVariantUtils.TryGetClone(ItemContentInfo), Inventory);
            ii.GridPos_Matrix = GridPos_Matrix;
            ii.BoundingRect = BoundingRect;
            ii.OccupiedGridPositions_Matrix = OccupiedGridPositions_Matrix.Clone();
            return ii;
        }
    }
}