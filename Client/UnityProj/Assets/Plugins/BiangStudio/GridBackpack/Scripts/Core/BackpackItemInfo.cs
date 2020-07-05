using System;
using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;

namespace BiangStudio.GridBackpack
{
    [Serializable]
    public class BackpackItemInfo : IClone<BackpackItemInfo>
    {
        private static int guidGenerator;

        public int GUID;

        public IBackpackItemContentInfo BackpackItemContentInfo;

        public string BackpackItemContentName
        {
            get
            {
                if (BackpackItemContentInfo != null)
                {
                    return BackpackItemContentInfo.BackpackItemName;
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

        public BackpackItemInfo(IBackpackItemContentInfo backpackItemContentInfo)
        {
            GUID = guidGenerator;
            guidGenerator++;

            BackpackItemContentInfo = backpackItemContentInfo;
            OccupiedGridPositions = BackpackItemContentInfo.OriginalOccupiedGridPositions.Clone();
            RefreshSize();
        }

        public void RefreshSize()
        {
            BoundingRect = OccupiedGridPositions.GetBoundingRectFromListGridPos();
        }

        public BackpackItemInfo Clone()
        {
            BackpackItemInfo bii = new BackpackItemInfo(CloneVariantUtils.TryGetClone(BackpackItemContentInfo));
            bii.GridPos = GridPos;
            bii.BoundingRect = BoundingRect;
            bii.OccupiedGridPositions = OccupiedGridPositions.Clone();
            return bii;
        }
    }
}