using BiangStudio.GameDataFormat;
using BiangStudio.Singleton;
using UnityEngine;

namespace GameCore
{
    public class LevelManager : TSingleton<LevelManager>
    {
        public static SRandom SRandom
        {
            get
            {
                if (Instance.CurrentLevel == null)
                {
                    Debug.LogError("未初始化关卡前禁止调用关卡随机数");
                    return null;
                }
                else
                {
                    return Instance.CurrentLevel.SRandom;
                }
            }
        }

        public Level CurrentLevel;

        public void Init(uint randomSeed)
        {
            Instance.CurrentLevel = new Level();
            Instance.CurrentLevel.Init(randomSeed);
        }
    }
}