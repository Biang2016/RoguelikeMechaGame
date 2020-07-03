using System.Collections.Generic;
using System.IO;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.Singleton;
using Newtonsoft.Json;
using UnityEngine;

namespace GameCore
{
    public class ConfigManager : TSingletonBaseManager<ConfigManager>
    {
        public override void Awake()
        {
            LoadMechaComponentOccupiedGridPosDict();
        }

        public const int GridSize = 1;

        public const int EDIT_AREA_HALF_SIZE = 9;
        public static int EDIT_AREA_FULL_SIZE => EDIT_AREA_HALF_SIZE * 2 + 1;

        public const float GunSpeed = 20;
        public const int GunDamage = 5;
        public const float MissileSpeed = 20;
        public const int MissileDamage = 10;

        public const int MaxPlayerNumber_Local = 2;

        public static string BlockOccupiedGridPosJsonFilePath = Application.streamingAssetsPath + "/BlockConfigs/BlockOccupiedGridPos.json";
        public static SortedDictionary<MechaComponentType, List<GridPos>> MechaComponentOccupiedGridPosDict = new SortedDictionary<MechaComponentType, List<GridPos>>();

        private static void LoadMechaComponentOccupiedGridPosDict()
        {
            StreamReader sr = new StreamReader(BlockOccupiedGridPosJsonFilePath);
            string content = sr.ReadToEnd();
            sr.Close();
            MechaComponentOccupiedGridPosDict = JsonConvert.DeserializeObject<SortedDictionary<MechaComponentType, List<GridPos>>>(content);
        }
    }
}