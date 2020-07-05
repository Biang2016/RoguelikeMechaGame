using BiangStudio.GameDataFormat;
using BiangStudio.Singleton;
using UnityEngine;

namespace Client
{
    public class ClientLevelManager : TSingletonBaseManager<ClientLevelManager>
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
            GameObject levelGO = new GameObject($"Level_{randomSeed}");
            CurrentLevel = levelGO.AddComponent<Level>();
            CurrentLevel.Init(randomSeed);
        }

        public override void Awake()
        {
        }

        public override void Start()
        {
        }

        public override void Update()
        {
        }

        public void StartLevel()
        {
        }
    }
}