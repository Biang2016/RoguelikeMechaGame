using System;
using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ShapedInventory;

namespace GameCore
{
    [Serializable]
    public class MechaComponentInfo : IInventoryItemContentInfo
    {
        public MechaComponentType MechaComponentType;
        public GridPosR GridPos;
        public List<GridPos> OccupiedGridPositions = new List<GridPos>();

        public int DropProbability;
        public int TotalLife;

        public List<GridPos> OriginalOccupiedGridPositions => OccupiedGridPositions;

        public string ItemSpriteKey => typeof(MechaComponentType).FullName + "." + MechaComponentType;

        public string ItemName => "机甲组件." + MechaComponentType;

        public MechaComponentInfo(MechaComponentType mechaComponentType, GridPosR gridPos, int totalLife, int dropProbability)
        {
            MechaComponentType = mechaComponentType;
            GridPos = gridPos;
            TotalLife = totalLife;
            DropProbability = dropProbability;
            if (ConfigManager.MechaComponentOccupiedGridPosDict.TryGetValue(mechaComponentType, out List<GridPos> ops))
            {
                OccupiedGridPositions = ops.Clone();
            }
        }

        public MechaComponentInfo Clone()
        {
            MechaComponentInfo mci = new MechaComponentInfo(MechaComponentType, GridPos, TotalLife, DropProbability);
            mci.OccupiedGridPositions = OccupiedGridPositions.Clone();
            return mci;
        }
    }
}