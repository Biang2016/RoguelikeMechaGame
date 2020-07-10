using System.Collections.Generic;

namespace GameCore
{
    public class GameData
    {
        public static List<MechaInfo> AllMechaInfo = new List<MechaInfo>();

        public static List<MechaComponentInfo> AllMechaComponentInfo = new List<MechaComponentInfo>();

        public static List<ProjectileInfo> AllProjectileInfo = new List<ProjectileInfo>();

        public static int CurrentLogicFrame = 0;

        public static bool IsReplayMode = false;

        public static bool IsBattleEnd = false;

        public static void Clear()
        {
            AllMechaInfo.Clear();
            AllMechaComponentInfo.Clear();
            AllProjectileInfo.Clear();
        }
    }
}