using System.Collections.Generic;
using BiangStudio.Messenger;
using UnityEngine.Events;

namespace GameCore
{
    public class BattleInfo
    {
        public MechaInfo PlayerMechaInfo;
        public SortedDictionary<int, MechaInfo> EnemyMechaInfoDict = new SortedDictionary<int, MechaInfo>();

        public UnityAction<MechaInfo> OnAddEnemyMechaInfoSuc;

        public Messenger BattleMessenger = new Messenger();

        public BattleInfo(MechaInfo playerMechaInfo)
        {
            PlayerMechaInfo = playerMechaInfo;
        }

        public void SetPlayerMecha(MechaInfo mechaInfo)
        {
            PlayerMechaInfo = mechaInfo;
        }

        public void AddEnemyMechaInfo(MechaInfo mechaInfo)
        {
            mechaInfo.OnRemoveMechaInfoSuc += RemoveEnemyMechaInfo;
            EnemyMechaInfoDict.Add(mechaInfo.GUID, mechaInfo);
            OnAddEnemyMechaInfoSuc?.Invoke(mechaInfo);
        }

        private void RemoveEnemyMechaInfo(MechaInfo mechaInfo)
        {
            EnemyMechaInfoDict.Remove(mechaInfo.GUID);
        }
    }
}