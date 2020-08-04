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

        private static uint guidGenerator = 10000;

        [HideInInspector] public uint GUID;

        public IInventoryItemContentInfo ItemContentInfo;

        public Inventory Inventory;

        public GridPosR GridPos_Matrix;

        public GridPosR GridPos_World => Inventory.CoordinateTransformationHandler_FromMatrixIndexToPos(GridPos_Matrix);

        public List<GridPos> OccupiedGridPositions_Matrix
        {
            get { return GridPosR.TransformOccupiedPositions(GridPos_Matrix, ItemContentInfo.OriginalOccupiedGridPositions); }
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

        private uint GetGUID()
        {
            return guidGenerator++;
        }

        public InventoryItem(IInventoryItemContentInfo itemContentInfo, Inventory inventory, GridPosR gp_matrix)
        {
            GUID = GetGUID();
            Inventory = inventory;
            GridPos_Matrix = gp_matrix;
            ItemContentInfo = itemContentInfo;
        }

        public void SetGridPosition(GridPosR gp_matrix)
        {
            if (!gp_matrix.Equals(GridPos_Matrix))
            {
                GridPosR oriGPR = GridPos_Matrix;
                GridPos_Matrix = gp_matrix;
                foreach (GridPos gp in OccupiedGridPositions_Matrix)
                {
                    if (gp.x >= Inventory.Columns)
                    {
                        GridPos_Matrix = oriGPR;
                        SetGridPosition(new GridPosR(gp_matrix.x - 1, gp_matrix.z, gp_matrix.orientation));
                        return;
                    }

                    if (gp.x < 0)
                    {
                        GridPos_Matrix = oriGPR;
                        SetGridPosition(new GridPosR(gp_matrix.x + 1, gp_matrix.z, gp_matrix.orientation));
                        return;
                    }

                    if (gp.z >= Inventory.Rows)
                    {
                        GridPos_Matrix = oriGPR;
                        SetGridPosition(new GridPosR(gp_matrix.x, gp_matrix.z - 1, gp_matrix.orientation));
                        return;
                    }

                    if (gp.z < 0)
                    {
                        GridPos_Matrix = oriGPR;
                        SetGridPosition(new GridPosR(gp_matrix.x, gp_matrix.z + 1, gp_matrix.orientation));
                        return;
                    }
                }

                OnSetGridPosHandler?.Invoke(GridPos_World);
            }
        }

        public void Rotate()
        {
            SetGridPosition(new GridPosR(GridPos_Matrix.x, GridPos_Matrix.z, GridPosR.RotateOrientationClockwise90(GridPos_Matrix.orientation)));
        }

        public InventoryItem Clone()
        {
            InventoryItem ii = new InventoryItem(CloneVariantUtils.TryGetClone(ItemContentInfo), Inventory, GridPos_Matrix);
            ii.GridPos_Matrix = GridPos_Matrix;
            return ii;
        }
    }
}