using System.Collections.Generic;
using BiangStudio.GamePlay.UI;
using BiangStudio.Messenger;
using BiangStudio.Singleton;
using GameCore;
using UnityEngine;

namespace Client
{
    public class UIBattleTipManager : TSingleton<UIBattleTipManager>
    {
        private Messenger Messenger => ClientGameManager.Instance.BattleMessenger;
        private List<UIBattleTip> UIBattleTipList = new List<UIBattleTip>();

        public void Init()
        {
            RegisterEvent();
        }

        public void ShutDown()
        {
            foreach (UIBattleTip uiBattleTip in UIBattleTipList)
            {
                uiBattleTip.PoolRecycle();
            }

            UIBattleTipList.Clear();
            UnRegisterEvent();
        }

        private void RegisterEvent()
        {
            Messenger.AddListener<AttackData>((uint) ENUM_BattleEvent.Battle_MechaComponentAttackTip, HandleAttackTip);
            Messenger.AddListener<uint, BattleTipType>((uint) ENUM_BattleEvent.Battle_MechaComponentCommonTip, HandleCommonTip);
        }

        private void UnRegisterEvent()
        {
            Messenger.RemoveListener<AttackData>((uint) ENUM_BattleEvent.Battle_MechaComponentAttackTip, HandleAttackTip);
            Messenger.RemoveListener<uint, BattleTipType>((uint) ENUM_BattleEvent.Battle_MechaComponentCommonTip, HandleCommonTip);
        }

        private void HandleAttackTip(AttackData attackData)
        {
            UIBattleTipInfo info = new UIBattleTipInfo(
                0,
                attackData.BattleTipType,
                GetAttackerType(attackData.AttackerMCI.MechaInfo, attackData.HitterMCB.Mecha.MechaInfo, attackData.BattleTipType),
                attackData.DecHp,
                attackData.ElementHP,
                0.5f,
                attackData.ElementType,
                "",
                Color.red,
                attackData.HitterMCB.transform.position + Vector3.up * 3f,
                Vector2.zero,
                Vector2.zero,
                1.0f);
            CreateTip(info);
        }

        private void HandleCommonTip(uint mcbGUID, BattleTipType battleTipType)
        {
            if ((int) battleTipType >= (int) BattleTipType.FollowDummySeparate)
            {
                return;
            }

            AttackerType attackerType = AttackerType.None;

            MechaComponentBase mcb_owner = ClientBattleManager.Instance.FindMechaComponentBase(mcbGUID);
            if (ClientBattleManager.Instance.PlayerMecha != null && mcb_owner != null)
            {
                attackerType = GetAttackerType(mcb_owner.Mecha.MechaInfo, ClientBattleManager.Instance.PlayerMecha.MechaInfo, battleTipType);
            }

            if (attackerType == AttackerType.NoTip)
            {
                return;
            }

            UIBattleTipInfo info = new UIBattleTipInfo(
                0,
                battleTipType,
                attackerType,
                0,
                0,
                0.5f,
                0,
                "",
                Color.red,
                mcb_owner.transform.position + Vector3.up * 3f,
                Vector2.zero,
                Vector2.zero,
                1.0f);
            CreateTip(info);
        }

        private void CreateTip(UIBattleTipInfo info)
        {
            UIBattleTip tip = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.UIBattleTip].AllocateGameObject<UIBattleTip>(UIManager.Instance.UI3DRoot);
            tip.Initialize(info);
        }

        private AttackerType GetAttackerType(MechaInfo attacker, MechaInfo hitter, BattleTipType battleTipType)
        {
            //不走攻击类型判定
            if ((int) battleTipType > (int) BattleTipType.NoAttackSeparate)
            {
                return AttackerType.None;
            }

            if (attacker != null)
            {
                //主角
                if (attacker.IsPlayer)
                {
                    return AttackerType.LocalPlayer;
                }

                //同个阵营
                if (hitter != null && attacker.IsFriend(hitter))
                {
                    return AttackerType.NoTip;
                }

                //队友
                if (attacker.IsMainPlayerFriend())
                {
                    return AttackerType.Team;
                }

                //敌人
                if (hitter != null && attacker.IsOpponent(hitter))
                {
                    return AttackerType.Enemy;
                }
            }

            return AttackerType.None;
        }
    }
}