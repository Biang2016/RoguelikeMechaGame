using System.Collections.Generic;
using BiangStudio.GamePlay.UI;
using BiangStudio.Messenger;
using BiangStudio.Singleton;
using GameCore;
using UnityEngine;

namespace Client
{
    public class UIBattleTipManager : TSingletonBaseManager<UIBattleTipManager>
    {
        private Messenger Messenger => ClientGameManager.Instance.BattleMessenger;
        public List<UIBattleTip> UIBattleTipList = new List<UIBattleTip>();

        public bool EnableUIBattleTip = true;

        public void Init()
        {
            RegisterEvent();
        }

        public override void Update()
        {
            base.Update();
            if (ControlManager.Instance.Battle_ToggleBattleTip.Up)
            {
                EnableUIBattleTip = !EnableUIBattleTip;
            }
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
            if (!EnableUIBattleTip) return;
            UIBattleTipInfo info = new UIBattleTipInfo(
                0,
                attackData.BattleTipType,
                GetAttackerType(attackData.AttackerMCI.MechaInfo, attackData.HitterMCB.Mecha.MechaInfo, attackData.BattleTipType),
                attackData.DecHp,
                attackData.ElementHP,
                0.13f,
                attackData.ElementType,
                "",
                attackData.HitterMCB.transform.position + Vector3.up * 1f,
                Vector2.zero,
                Vector2.one,
                0.5f);
            CreateTip(info);
        }

        private void HandleCommonTip(uint mcGUID, BattleTipType battleTipType)
        {
            if (!EnableUIBattleTip) return;
            if ((int) battleTipType >= (int) BattleTipType.FollowDummySeparate)
            {
                return;
            }

            AttackerType attackerType = AttackerType.None;

            MechaComponent mc_owner = ClientBattleManager.Instance.FindMechaComponent(mcGUID);
            if (ClientBattleManager.Instance.PlayerMecha != null && mc_owner != null)
            {
                attackerType = GetAttackerType(mc_owner.Mecha.MechaInfo, ClientBattleManager.Instance.PlayerMecha.MechaInfo, battleTipType);
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
                0.13f,
                0,
                "",
                mc_owner.transform.position + Vector3.up * 1f,
                Vector2.zero,
                Vector2.one,
                0.5f);
            CreateTip(info);
        }

        private void CreateTip(UIBattleTipInfo info)
        {
            int maxSortingOrder = 0;
            foreach (UIBattleTip uiBattleTip in UIBattleTipList)
            {
                if (uiBattleTip.SortingOrder > maxSortingOrder)
                {
                    maxSortingOrder = uiBattleTip.SortingOrder;
                }
            }

            BattleTipPrefabType btType = BattleTipPrefabType.SelfAttack;

            if (info.AttackerType == AttackerType.LocalPlayer)
            {
                if (info.BattleTipType == BattleTipType.CriticalAttack)
                {
                    btType = BattleTipPrefabType.SelfCriticalAttack;
                }
                else if (info.BattleTipType == BattleTipType.Attack)
                {
                    btType = BattleTipPrefabType.SelfAttack;
                }
            }
            else if (info.AttackerType == AttackerType.LocalPlayerSelfDamage)
            {
                if (info.BattleTipType == BattleTipType.Attack)
                {
                    btType = BattleTipPrefabType.SelfDamage;
                }
            }

            UIBattleTip tip = GameObjectPoolManager.Instance.BattleUIDict[btType].AllocateGameObject<UIBattleTip>(UIManager.Instance.UI3DRoot);
            tip.Initialize(info, maxSortingOrder + 1);
            UIBattleTipList.Add(tip);
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
                    if (hitter.IsPlayer)
                    {
                        return AttackerType.LocalPlayerSelfDamage;
                    }
                    else
                    {
                        return AttackerType.LocalPlayer;
                    }
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