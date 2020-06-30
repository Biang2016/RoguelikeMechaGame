﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameCore
{
    [Serializable]
    public class MechaComponentInfo : IBagItemContentInfo
    {
        public MechaComponentType MechaComponentType;
        public GridPos GridPos;
        public List<GridPos> OccupiedGridPositions = new List<GridPos>();

        public int DropProbability;
        public int TotalLife;

        public List<GridPos> OriginalOccupiedGridPositions => OccupiedGridPositions;

        public string BagItemSpriteKey => typeof(MechaComponentType).FullName + "." + MechaComponentType;

        public MechaComponentInfo(MechaComponentType mechaComponentType, GridPos gridPos, int totalLife, int dropProbability)
        {
            MechaComponentType = mechaComponentType;
            GridPos = gridPos;
            TotalLife = totalLife;
            DropProbability = dropProbability;
            OccupiedGridPositions = CloneVariantUtils.List(ConfigManager.MechaComponentOccupiedGridPosDict[mechaComponentType]);
        }

        public MechaComponentInfo Clone()
        {
            MechaComponentInfo mci = new MechaComponentInfo(MechaComponentType, GridPos, TotalLife, DropProbability);
            mci.OccupiedGridPositions = CloneVariantUtils.List(OccupiedGridPositions);
            return mci;
        }
    }
}