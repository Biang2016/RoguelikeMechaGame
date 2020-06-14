using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace GameCore
{
    public static class ConfigManager
    {
        public const float GunSpeed= 20;
        public const int GunDamage = 5;
        public const float MissileSpeed = 20;
        public const int MissileDamage = 10;

        public const int MaxPlayerNumber_Local = 2;

        public static string BlockOccupiedGridPosJsonFilePath = Application.streamingAssetsPath + "/BlockConfigs/BlockOccupiedGridPos.json";
        public static SortedDictionary<MechaComponentType, List<GridPos>> MechaComponentOccupiedGridPosDict = new SortedDictionary<MechaComponentType, List<GridPos>>();

        public static void LoadMechaComponentOccupiedGridPosDict()
        {
            StreamReader sr = new StreamReader(BlockOccupiedGridPosJsonFilePath);
            string content = sr.ReadToEnd();
            sr.Close();
            MechaComponentOccupiedGridPosDict = JsonConvert.DeserializeObject<SortedDictionary<MechaComponentType, List<GridPos>>>(content);
        }
    }
}