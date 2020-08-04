using System.Collections.Generic;
using BiangStudio.Singleton;
using GameCore;

namespace Client
{
    /// <summary>
    /// 主要管理关卡拼接
    /// </summary>
    public class ClientLevelManager : TSingletonBaseManager<ClientLevelManager>
    {
        public ClientLevel CurrentClientLevel;
        public SortedDictionary<uint, ClientLevel> ClientLevelDict = new SortedDictionary<uint, ClientLevel>();

        public void Init()
        {
            LevelManager.Instance.OnGenerateLevel += GenerateClientLevel;
            LevelManager.Instance.OnSetCurrentLevel += SetCurrentLevel;
        }

        private void GenerateClientLevel(LevelInfo levelInfo)
        {
            ClientLevel newLevel = ClientLevel.GenerateClientLevel(levelInfo);
            ClientLevelDict.Add(levelInfo.GUID, newLevel);
        }

        private void SetCurrentLevel(uint guid)
        {
            CurrentClientLevel = ClientLevelDict[guid];
            CurrentClientLevel.name = "[Current]Level_" + CurrentClientLevel.LevelInfo.LevelName;
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