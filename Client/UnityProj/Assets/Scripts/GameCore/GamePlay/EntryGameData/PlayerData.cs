using System.Collections.Generic;
using BiangStudio.GridBackpack;

namespace GameCore
{
    public class PlayerData
    {
        public MechaInfo MechaInfo;
        public List<MechaComponentInfo> MechaComponentInfos = new List<MechaComponentInfo>();
        public Backpack BackPack;
    }
}