using System.Collections.Generic;

namespace GameCore
{
    public class LevelGroupInfo
    {
        public SortedDictionary<uint, LevelInfo> LevelInfoDict = new SortedDictionary<uint, LevelInfo>();
        public LevelInfo CurrentLevelInfo;

        // todo Connection Info
    }
}