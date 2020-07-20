using System.Collections.Generic;

namespace GameCore
{
    public class PlayerData
    {
        public MechaInfo MechaInfo;
        public List<MechaComponentInfo> MechaComponentInfos = new List<MechaComponentInfo>();
        public Backpack BackPack;
    }
}