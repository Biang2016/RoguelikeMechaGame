using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore
{
    /// <summary>
    /// 游戏数据切片，代表关卡、玩家所有信息
    /// </summary>
    public class GameData
    {
        public SortedDictionary<uint, LevelInfo> LevelInfoDict = new SortedDictionary<uint, LevelInfo>();
        public LevelInfo CurrentLevelInfo;

        public PlayerData PlayerData;
    }
}