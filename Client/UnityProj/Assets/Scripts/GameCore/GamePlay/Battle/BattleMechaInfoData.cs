using System.Collections.Generic;

namespace GameCore
{
    public class BattleMechaInfoData
    {
        public MechaInfo PlayerMechaInfo;
        public SortedDictionary<uint, MechaInfo> EnemyMechaInfoDict = new SortedDictionary<uint, MechaInfo>();
    }
}