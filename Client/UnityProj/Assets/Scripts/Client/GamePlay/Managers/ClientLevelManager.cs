using BiangStudio.GameDataFormat;
using BiangStudio.Singleton;
using GameCore;
using UnityEngine;

namespace Client
{
    public class ClientLevelManager : TSingletonBaseManager<ClientLevelManager>
    {
        public ClientLevel ClientLevel;

        public void Init()
        {
            GameObject levelGO = new GameObject($"Level_{LevelManager.Instance.CurrentLevel.RandomSeed}");
            ClientLevel = levelGO.AddComponent<ClientLevel>();
            ClientLevel.Init(LevelManager.Instance.CurrentLevel);
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