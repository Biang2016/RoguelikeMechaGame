using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GameCore
{
    [Serializable]
    public class BagItemInfo : IClone<BagItemInfo>
    {
        private static int guidGenerator;

        [ReadOnly]
        public int GUID;

        public IBagItemContentInfo BagItemContentInfo;

        [ShowInInspector]
        [PropertyOrder(-1)]
        [LabelText("物品名称")]
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
                        ;
                }
            }
        }

        [BoxGroup("背包相关数据")]
        [LabelText("背包坐标")]
        public GridPosR GridPos;

        [BoxGroup("背包相关数据")]
        [LabelText("背包占用坐标列表")]
        public List<GridPos> OccupiedGridPositions = new List<GridPos>();

        [BoxGroup("背包相关数据")]
        [LabelText("背包占用矩形")]
        public GridRect BoundingRect;

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
            BoundingRect = OccupiedGridPositions.GetBoundingRectFromListGridPos();
        }

        public BagItemInfo Clone()
        {
            BagItemInfo bii = new BagItemInfo(CloneVariantUtils.TryGetClone(BagItemContentInfo));
            bii.GridPos = GridPos;
            bii.BoundingRect = BoundingRect;
            bii.OccupiedGridPositions = CloneVariantUtils.List(OccupiedGridPositions);
            return bii;
        }
    }
}