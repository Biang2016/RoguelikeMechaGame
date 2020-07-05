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
        private static int guidGenerator;

        [HideInInspector]
        public int GUID;

        public IInventoryItemContentInfo ItemContentInfo;

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

        public GridPosR GridPos;

        public List<GridPos> OccupiedGridPositions = new List<GridPos>();

        [HideInInspector]
        public GridRect BoundingRect;

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