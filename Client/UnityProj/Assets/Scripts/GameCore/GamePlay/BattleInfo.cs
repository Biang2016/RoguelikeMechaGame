using System.Collections.Generic;
using BiangStudio.Messenger;
using UnityEngine.Events;

namespace GameCore
{
    public class BattleInfo
    {
        public BattleMechaInfoData BattleMechaInfoData = new BattleMechaInfoData();

        public UnityAction<MechaInfo> OnAddEnemyMechaInfoSuc;

        public Messenger BattleMessenger = new Messenger();

        public BattleInfo(MechaInfo playerMechaInfo)
        {
            BattleMechaInfoData.PlayerMechaInfo = playerMechaInfo;
        }

        public void SetPlayerMecha(MechaInfo mechaInfo)
        {
            BattleMechaInfoData.PlayerMechaInfo = mechaInfo;
        }

        public void AddEnemyMechaInfo(MechaInfo mechaInfo)
        {
            mechaInfo.OnRemoveMechaInfoSuc += RemoveEnemyMechaInfo;
            BattleMechaInfoData.EnemyMechaInfoDict.Add(mechaInfo.GUID, mechaInfo);
            OnAddEnemyMechaInfoSuc?.Invoke(mechaInfo);
        }

        private void RemoveEnemyMechaInfo(MechaInfo mechaInfo)
        {
            BattleMechaInfoData.EnemyMechaInfoDict.Remove(mechaInfo.GUID);
        }
    }

    public class BattleMechaInfoData
    {
        public MechaInfo PlayerMechaInfo;
        public SortedDictionary<uint, MechaInfo> EnemyMechaInfoDict = new SortedDictionary<uint, MechaInfo>();
    }
}