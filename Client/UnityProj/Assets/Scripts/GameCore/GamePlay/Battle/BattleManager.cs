using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.Messenger;
using BiangStudio.Singleton;
using GameCore.AbilityDataDriven;
using UnityEngine;

namespace GameCore
{
    public class BattleManager : TSingletonBaseManager<BattleManager>
    {
        private BattleInfo BattleInfo;
        public MechaInfo PlayerMechaInfo;
        public SortedDictionary<uint, MechaInfo> MechaInfoDict = new SortedDictionary<uint, MechaInfo>();

        public Messenger BattleMessenger = new Messenger();

        public void Clear()
        {
            BattleInfo?.Clear();
            BattleInfo = null;
            MechaInfoDict.Clear();
            PlayerMechaInfo = null;
        }

        public override void Awake()
        {
        }

        public override void Start()
        {
        }

        public override void Update()
        {
            BattleInfo?.BattleMechaInfoData?.PlayerMechaInfo?.UpdateLifeChange();
            if (BattleInfo?.BattleMechaInfoData != null)
            {
                foreach (KeyValuePair<uint, MechaInfo> kv in BattleInfo.BattleMechaInfoData.EnemyMechaInfoDict)
                {
                    kv.Value?.UpdateLifeChange();
                }
            }
        }

        public void StartBattle(BattleInfo battleInfo)
        {
            Clear();
            BattleInfo = battleInfo;
            BattleInfo.OnRemoveMechaInfoSuc = RemoveMechaInfo;

            PlayerMechaInfo = battleInfo.BattleMechaInfoData.PlayerMechaInfo;
            battleInfo.SetPlayerMecha(PlayerMechaInfo);
            foreach (KeyValuePair<uint, MechaInfo> kv in battleInfo.BattleMechaInfoData.EnemyMechaInfoDict)
            {
                AddEnemyMecha(kv.Value);
            }
        }

        public void AddEnemyMecha(MechaInfo mechaInfo)
        {
            MechaInfoDict.Add(mechaInfo.GUID, mechaInfo);
            BattleInfo.AddEnemyMechaInfo(mechaInfo);
        }

        private void RemoveMechaInfo(MechaInfo mechaInfo)
        {
            MechaInfoDict.Remove(mechaInfo.GUID);
        }

        public MechaInfo FindMecha(uint guid)
        {
            MechaInfoDict.TryGetValue(guid, out MechaInfo mechaInfo);
            return mechaInfo;
        }

        public MechaComponentInfo FindMechaComponent(uint guid)
        {
            foreach (KeyValuePair<uint, MechaInfo> kv in MechaInfoDict)
            {
                if (kv.Value.MechaComponentInfoDict.TryGetValue(guid, out MechaComponentInfo mci))
                {
                    return mci;
                }
            }

            return null;
        }

        #region Proxy

        public delegate List<MechaComponentInfo> SearchRangeDelegate(Vector3 center, float radius, MechaType mechaType, ENUM_MultipleTargetTeam Team, int maxTargets, bool random);

        public SearchRangeDelegate SearchRangeHandler;

        #endregion
    }
}