using System;
using System.Collections.Generic;

namespace GameCore
{
    [Serializable]
    public class BagItemInfo : IClone<BagItemInfo>
    {
        private static int guidGenerator;
        public int GUID;

        public MechaComponentInfo MechaComponentInfo;

        public GridPos GridPos;
        public List<GridPos> OccupiedGridPositions = new List<GridPos>();

        public IntRect Size;

        public BagItemInfo(MechaComponentInfo mechaComponentInfo)
        {
            GUID = guidGenerator;
            guidGenerator++;

            MechaComponentInfo = mechaComponentInfo;
            OccupiedGridPositions = MechaComponentInfo.OccupiedGridPositions;
            RefreshSize();
        }

        public void RefreshSize()
        {
            int X_min = 999;
            int X_max = -999;
            int Z_min = 999;
            int Z_max = -999;
            foreach (GridPos gp in OccupiedGridPositions)
            {
                if (gp.x < X_min)
                {
                    X_min = gp.x;
                }

                if (gp.x > X_max)
                {
                    X_max = gp.x;
                }

                if (gp.z < Z_min)
                {
                    Z_min = gp.z;
                }

                if (gp.z > Z_max)
                {
                    Z_max = gp.z;
                }
            }

            Size = new IntRect(X_min, Z_min, X_max - X_min + 1, Z_max - Z_min + 1);
        }

        public BagItemInfo Clone()
        {
            BagItemInfo bii = new BagItemInfo(MechaComponentInfo.Clone());
            bii.GridPos = GridPos;
            bii.Size = Size;
            bii.OccupiedGridPositions = CloneVariantUtils.List(OccupiedGridPositions);
            return bii;
        }
    }
}