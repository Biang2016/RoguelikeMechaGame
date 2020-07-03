using System;
using System.Collections.Generic;
using GameCore;

namespace GameCore
{
    [Serializable]
    public class MechaInfo
    {
        public string MechaName;
        public MechaType MechaType;
        public List<MechaComponentInfo> MechaComponentInfos;

        public MechaInfo(string mechaName, MechaType mechaType, List<MechaComponentInfo> mechaComponentInfos)
        {
            MechaName = mechaName;
            MechaType = mechaType;
            MechaComponentInfos = mechaComponentInfos;
        }
    }

    public enum MechaType
    {
        None = 0,
        Self = 1,
        Enemy = 2,
    }
}