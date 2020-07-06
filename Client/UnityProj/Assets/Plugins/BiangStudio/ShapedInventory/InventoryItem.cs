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

        public GridPosR GridPos_Matrix;

        public GridPosR GridPos_World => Inventory.CoordinateTransformationHandler_FromMatrixIndexToPos(GridPos_Matrix);

        public List<GridPos> OccupiedGridPositions_Matrix
        {
            get
            {
                return GridPosR.TransformOccupiedPositions(GridPos_Matrix, ItemContentInfo.OriginalOccupiedGridPositions);
            }
        }

        [HideInInspector] public GridRect BoundingRect => OccupiedGridPositions_Matrix.GetBoundingRectFromListGridPos();

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

        public InventoryItem(IInventoryItemContentInfo itemContentInfo, Inventory inventory, GridPosR gp_matrix)
        {
            GUID = guidGenerator;
            guidGenerator++;

            GridPos_Matrix = gp_matrix;
            Inventory = inventory;
            ItemContentInfo = itemContentInfo;
        }

        public void SetGridPosition(GridPosR gp_matrix)
        {
            if (!gp_matrix.Equals(GridPos_Matrix))
            {
                foreach (GridPos gp in OccupiedGridPositions_Matrix)
                {
                    GridPos gp_rot = GridPos.RotateGridPos(gp - (GridPos) GridPos_Matrix, (GridPosR.Orientation) ((gp_matrix.orientation - GridPos_Matrix.orientation + 4) % 4));
                    GridPosR newGP = gp_matrix + (GridPosR) gp_rot;
                    if (newGP.x >= Inventory.Columns)
                    {
                        SetGridPosition(new GridPosR(gp_matrix.x - 1, gp_matrix.z, gp_matrix.orientation));
                        return;
                    }

                    if (newGP.x < 0)
                    {
                        SetGridPosition(new GridPosR(gp_matrix.x + 1, gp_matrix.z, gp_matrix.orientation));
                        return;
                    }

                    if (newGP.z >= Inventory.Rows)
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
                OnSetGridPosHandler?.Invoke(GridPos_World);
            }
        }

        public void Rotate()
        {
        }

        public InventoryItem Clone()
        {
            InventoryItem ii = new InventoryItem(CloneVariantUtils.TryGetClone(ItemContentInfo), Inventory, GridPos_Matrix);
            ii.GridPos_Matrix = GridPos_Matrix;
            return ii;
        }
    }
}