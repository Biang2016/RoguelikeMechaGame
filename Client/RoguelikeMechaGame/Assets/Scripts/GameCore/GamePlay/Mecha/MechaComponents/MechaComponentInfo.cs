using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameCore
{
    [Serializable]
    public class MechaComponentInfo : IBagItemContentInfo
    {
        public MechaComponentType MechaComponentType;
        public GridPosR GridPos;
        public List<GridPos> OccupiedGridPositions = new List<GridPos>();

        public int DropProbability;
        public int TotalLife;

        public List<GridPos> OriginalOccupiedGridPositions => OccupiedGridPositions;

        public string BagItemSpriteKey => typeof(MechaComponentType).FullName + "." + MechaComponentType;

        public string BagItemName => "机甲组件." + MechaComponentType;

        public MechaComponentInfo(MechaComponentType mechaComponentType, GridPosR gridPos, int totalLife, int dropProbability)
        {
            MechaComponentType = mechaComponentType;
            GridPos = gridPos;
            TotalLife = totalLife;
            DropProbability = dropProbability;
            if (ConfigManager.MechaComponentOccupiedGridPosDict.TryGetValue(mechaComponentType, out List<GridPos> ops))
            {
                OccupiedGridPositions = CloneVariantUtils.List(ops);
            }
        }

        public MechaComponentInfo Clone()
        {
            MechaComponentInfo mci = new MechaComponentInfo(MechaComponentType, GridPos, TotalLife, DropProbability);
            mci.OccupiedGridPositions = CloneVariantUtils.List(OccupiedGridPositions);
            return mci;
        }
    }
}