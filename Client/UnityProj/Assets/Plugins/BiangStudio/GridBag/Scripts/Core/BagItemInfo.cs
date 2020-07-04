using System;
using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;

namespace BiangStudio.GridBag
{
    [Serializable]
    public class BagItemInfo : IClone<BagItemInfo>
    {
        private static int guidGenerator;

        public int GUID;

        public IBagItemContentInfo BagItemContentInfo;

        public string BagItemContentName
        {
            get
            {
                if (BagItemContentInfo != null)
                {
                    return BagItemContentInfo.BagItemName;
                }
                else
                {
                    return "";
                }
            }
        }

        public GridPosR GridPos;

        public List<GridPos> OccupiedGridPositions = new List<GridPos>();

        public GridRect BoundingRect;

        public BagItemInfo(IBagItemContentInfo bagItemContentInfo)
        {
            GUID = guidGenerator;
            guidGenerator++;

            BagItemContentInfo = bagItemContentInfo;
            OccupiedGridPositions = BagItemContentInfo.OriginalOccupiedGridPositions.Clone();
            RefreshSize();
        }

        public void RefreshSize()
        {
            BoundingRect = OccupiedGridPositions.GetBoundingRectFromListGridPos();
        }

        public BagItemInfo Clone()
        {
            BagItemInfo bii = new BagItemInfo(CloneVariantUtils.TryGetClone(BagItemContentInfo));
            bii.GridPos = GridPos;
            bii.BoundingRect = BoundingRect;
            bii.OccupiedGridPositions = OccupiedGridPositions.Clone();
            return bii;
        }
    }
}