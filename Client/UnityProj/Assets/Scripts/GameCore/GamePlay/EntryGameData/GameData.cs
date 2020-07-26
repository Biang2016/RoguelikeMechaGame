using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// 游戏数据切片，代表关卡、玩家所有信息
    /// </summary>
    public class GameData
    {
        public SortedDictionary<uint, LevelInfo> LevelInfoDict = new SortedDictionary<uint, LevelInfo>();
        public LevelGroupInfo LevelGroupInfo;

        public PlayerData PlayerData;
    }
}