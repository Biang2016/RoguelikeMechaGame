using BiangStudio.GameDataFormat;
using BiangStudio.Singleton;
using UnityEngine;
using UnityEngine.Events;

namespace GameCore
{
    public class LevelManager : TSingleton<LevelManager>
    {
        public static SRandom SRandom
        {
            get
            {
                if (Instance.LevelInfoGroup.CurrentLevelInfo == null)
                {
                    Debug.LogError("未初始化关卡前禁止调用关卡随机数");
                    return null;
                }
                else
                {
                    return Instance.LevelInfoGroup.CurrentLevelInfo.SRandom;
                }
            }
        }

        public LevelGroupInfo LevelInfoGroup = new LevelGroupInfo();

        public void Init()
        {
            LevelInfo startLevelInfo = new LevelInfo(6789, "StartLevel");
            AddLevel(startLevelInfo);
            SetCurrentLevel(startLevelInfo);
        }

        public void AddLevel(LevelInfo levelInfo)
        {
            LevelInfoGroup.LevelInfoDict.Add(levelInfo.GUID, levelInfo);
            OnGenerateLevel?.Invoke(levelInfo);
        }

        public void SetCurrentLevel(LevelInfo levelInfo)
        {
            LevelInfoGroup.CurrentLevelInfo = levelInfo;
            OnSetCurrentLevel?.Invoke(levelInfo.GUID);
        }

        public UnityAction<LevelInfo> OnGenerateLevel;
        public UnityAction<uint> OnSetCurrentLevel;
    }
}