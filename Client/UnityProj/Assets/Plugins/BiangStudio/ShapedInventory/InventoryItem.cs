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
        public delegate void OnIsolatedDelegate(bool isolated);

        public delegate void OnConflictedDelegate(GridPos gridPos);

        public delegate void OnResetConflictDelegate();
        public delegate bool AmIRootItemInIsolationCalculationDelegate();

        private static int guidGenerator;

        [HideInInspector] public int GUID;

        public IInventoryItemContentInfo ItemContentInfo;

        public Inventory Inventory;

        public GridPosR GridPos;
        public List<GridPos> OccupiedGridPositions;

        [HideInInspector] public GridRect BoundingRect;

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

        public InventoryItem(IInventoryItemContentInfo itemContentInfo)
        {
            GUID = guidGenerator;
            guidGenerator++;

            ItemContentInfo = itemContentInfo;
            OccupiedGridPositions = ItemContentInfo.OriginalOccupiedGridPositions.Clone();
            RefreshSize();
        }

        public void RefreshSize()
        {
            BoundingRect = OccupiedGridPositions.GetBoundingRectFromListGridPos();
        }

        public InventoryItem Clone()
        {
            InventoryItem ii = new InventoryItem(CloneVariantUtils.TryGetClone(ItemContentInfo));
            ii.GridPos = GridPos;
            ii.BoundingRect = BoundingRect;
            ii.OccupiedGridPositions = OccupiedGridPositions.Clone();
            return ii;
        }
    }
}