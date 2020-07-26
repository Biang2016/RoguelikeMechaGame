using BiangStudio.GameDataFormat;
using UnityEngine;

namespace GameCore
{
    /// <summary>
    /// 反映单个Level中的所有信息，包括地图、可破坏物状态等
    /// </summary>
    public class LevelInfo
    {
        private static uint guidGenerator = 10000;

        [HideInInspector]
        public uint GUID;

        private uint GetGUID()
        {
            return guidGenerator++;
        }

        public uint RandomSeed;
        public SRandom SRandom;

        public string LevelName;

        public LevelInfo(uint randomSeed, string levelName)
        {
            GUID = GetGUID();
            RandomSeed = randomSeed;
            SRandom = new SRandom(randomSeed);
            LevelName = levelName;
        }
    }
}