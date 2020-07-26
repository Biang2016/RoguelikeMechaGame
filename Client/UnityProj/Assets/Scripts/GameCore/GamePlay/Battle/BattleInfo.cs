using System.Collections.Generic;
using UnityEngine.Events;

namespace GameCore
{
    public class BattleInfo
    {
        public BattleMechaInfoData BattleMechaInfoData = new BattleMechaInfoData();

        public UnityAction<MechaInfo> OnAddMechaInfoSuc;
        public UnityAction<MechaInfo> OnRemoveMechaInfoSuc;

        public BattleInfo(MechaInfo playerMechaInfo)
        {
            BattleMechaInfoData.PlayerMechaInfo = playerMechaInfo;
        }

        public void SetPlayerMecha(MechaInfo mechaInfo)
        {
            BattleMechaInfoData.PlayerMechaInfo = mechaInfo;
            OnAddMechaInfoSuc?.Invoke(mechaInfo);
        }

        public void AddEnemyMechaInfo(MechaInfo mechaInfo)
        {
            mechaInfo.OnRemoveMechaInfoSuc += RemoveEnemyMechaInfo;
            BattleMechaInfoData.EnemyMechaInfoDict.Add(mechaInfo.GUID, mechaInfo);
            OnAddMechaInfoSuc?.Invoke(mechaInfo);
        }

        public void RemoveEnemyMechaInfo(MechaInfo mechaInfo)
        {
            BattleMechaInfoData.EnemyMechaInfoDict.Remove(mechaInfo.GUID);
            OnRemoveMechaInfoSuc?.Invoke(mechaInfo);
        }

        public void Clear()
        {
            BattleMechaInfoData.PlayerMechaInfo?.RemoveMechaInfo();
            BattleMechaInfoData.PlayerMechaInfo = null;
            foreach (KeyValuePair<uint, MechaInfo> kv in BattleMechaInfoData.EnemyMechaInfoDict)
            {
                kv.Value.RemoveMechaInfo();
            }

            BattleMechaInfoData.EnemyMechaInfoDict.Clear();
        }
    }
}