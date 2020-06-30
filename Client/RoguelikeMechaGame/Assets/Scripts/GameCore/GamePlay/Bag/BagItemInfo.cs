using System;
using System.Collections.Generic;

namespace GameCore
{
    [Serializable]
    public class BagItemInfo : IClone<BagItemInfo>
    {
        private static int guidGenerator;
        public int GUID;

        public IBagItemContentInfo BagItemContentInfo;

        public GridPos GridPos;
        public List<GridPos> OccupiedGridPositions = new List<GridPos>();

        public GridRect Size;

        public BagItemInfo(IBagItemContentInfo bagItemContentInfo)
        {
            GUID = guidGenerator;
            guidGenerator++;

            BagItemContentInfo = bagItemContentInfo;
            OccupiedGridPositions = CloneVariantUtils.List(BagItemContentInfo.OriginalOccupiedGridPositions);
            RefreshSize();
        }

        public void RefreshSize()
        {
            Size = OccupiedGridPositions.GetSizeFromListGridPos();
        }

        public BagItemInfo Clone()
        {
            BagItemInfo bii = new BagItemInfo(CloneVariantUtils.TryGetClone(BagItemContentInfo));
            bii.GridPos = GridPos;
            bii.Size = Size;
            bii.OccupiedGridPositions = CloneVariantUtils.List(OccupiedGridPositions);
            return bii;
        }
    }
}