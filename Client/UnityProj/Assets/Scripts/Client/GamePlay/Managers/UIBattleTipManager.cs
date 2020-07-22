using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.GamePlay.UI;
using BiangStudio.Messenger;
using BiangStudio.Singleton;
using GameCore;
using UnityEngine;

namespace Client
{
    public struct AttackData
    {
        public MechaComponentBase AttackerMCB;
        public MechaComponentBase HitterMCB;
        public int DecHp;
        public BattleTipType BattleTipType;
        public int ElementType;
        public int ElementHP;
    }

    public class UIBattleTipInfo : IClone<UIBattleTipInfo>
    {
        public uint HitMCB_GUID;
        public BattleTipType BattleTipType;
        public AttackerType AttackerType;
        public int DiffHP;
        public int ElementHP;
        public float Scale;
        public bool InScreenCenter;
        public int ElementType;
        public string SpriteImagePath;
        public Color Color;
        public Vector3 StartPos;
        public Vector2 Offset;
        public Vector2 RandomRange;
        public float DisappearTime = 1.5f;

        public UIBattleTipInfo(uint hitMcbGuid, BattleTipType battleTipType, AttackerType attackerType, int diffHp, int elementHp, float scale, bool inScreenCenter, int elementType,
            string spriteImagePath, Color color, Vector3 startPos, Vector2 offset, Vector2 randomRange, float disappearTime)
        {
            HitMCB_GUID = hitMcbGuid;
            BattleTipType = battleTipType;
            AttackerType = attackerType;
            DiffHP = diffHp;
            ElementHP = elementHp;
            Scale = scale;
            InScreenCenter = inScreenCenter;
            ElementType = elementType;
            SpriteImagePath = spriteImagePath;
            Color = color;
            StartPos = startPos;
            Offset = offset;
            RandomRange = randomRange;
            DisappearTime = disappearTime;
        }

        public UIBattleTipInfo Clone()
        {
            return new UIBattleTipInfo(HitMCB_GUID, BattleTipType, AttackerType, DiffHP, ElementHP, Scale, InScreenCenter, ElementType, SpriteImagePath, Color, StartPos, Offset, RandomRange, DisappearTime);
        }

        public class UIBattleTipManager : TSingleton<UIBattleTipManager>
        {
            private Messenger Messenger => ClientGameManager.Instance.BattleMessenger;
            private List<UIBattleTip> UIBattleTipList = new List<UIBattleTip>();

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

            private ulong GetHitActorID(MechaComponentBase hitter)
            {
                if (hitter != null)
                {
                    return hitter.MechaComponentInfo.GUID;
                }

                return 0;
            }

            private void HandleAttackTip(AttackData attackData)
            {
                UIBattleTipInfo info = new UIBattleTipInfo(
                    0,
                    attackData.BattleTipType,
                    GetAttackerType(attackData.AttackerMCB.Mecha, attackData.HitterMCB.Mecha, attackData.BattleTipType),
                    attackData.DecHp,
                    attackData.ElementHP,
                    1,
                    false,
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
                    attackerType = GetAttackerType(mcb_owner.Mecha, ClientBattleManager.Instance.PlayerMecha, battleTipType);
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
                    1,
                    false,
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

            private AttackerType GetAttackerType(Mecha attacker, Mecha hitter, BattleTipType battleTipType)
            {
                //不走攻击类型判定
                if ((int) battleTipType > (int) BattleTipType.NoAttackSeparate)
                {
                    return AttackerType.None;
                }

                if (attacker != null)
                {
                    //主角
                    if (attacker.MechaInfo.IsPlayer)
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
}