﻿using System;
using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GridBag;

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