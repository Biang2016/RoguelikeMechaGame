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

        public UIBattleTipInfo(uint hitMcbGuid, BattleTipType battleTipType, AttackerType attackerType, int diffHp, int elementHp, float scale, bool inScreenCenter, int elementType,
            string spriteImagePath, Color color, Vector3 startPos, Vector2 offset, Vector2 randomRange)
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
        }

        public UIBattleTipInfo Clone()
        {
            return new UIBattleTipInfo(HitMCB_GUID, BattleTipType, AttackerType, DiffHP, ElementHP, Scale, InScreenCenter, ElementType, SpriteImagePath, Color, StartPos, Offset, RandomRange);
        }

        public struct SAttackData
        {
            public MechaComponentBase MechaComponentBase;
            public long decHp;
            public UIBattleTipType attackType;
            public MechaComponentBase attackerMCB;
            public int elementType;
            public long elementHP;
        }

        public class UIBattleTipParam
        {
            public ulong hitIdent;
            public Vector3 startPos;
            public UIBattleTipType attackType;
            public long diffHP;
            public long elementHP;
            public float offsetX;
            public float offsetY;
            public uint attackerType;
            public float scale;
            public bool inScreenCenter;
            public int elementType;
            public string spriteImagePath;

            public void Setup(Vector3 pos, ulong id, UIBattleTipType _attackType, long _diff, uint _attackerType = 0, float _scale = 1.0f, int _elementType = 0, string _imagePath = "",
                long _elementHP = 0)
            {
                startPos = pos;
                hitIdent = id;
                attackType = _attackType;
                diffHP = _diff;
                scale = _scale;
                attackerType = _attackerType;
                offsetX = 0.0f;
                offsetY = 0.0f;
                inScreenCenter = (pos.magnitude < 0.001f);
                elementType = _elementType;
                spriteImagePath = _imagePath;
                elementHP = _elementHP;
            }

            public void CopyData(UIBattleTipParam param)
            {
                param.startPos = startPos;
                param.attackType = attackType;
                param.diffHP = diffHP;
                param.offsetX = offsetX;
                param.offsetY = offsetY;
                param.scale = scale;
                param.attackerType = attackerType;
                param.hitIdent = hitIdent;
                param.inScreenCenter = inScreenCenter;
                param.spriteImagePath = spriteImagePath;
                param.elementHP = elementHP;
            }
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
                    Vector2.zero);
                CreateTip(info);
            }

            private void HandleCommonTip(uint mcbGUID, BattleTipType battleTipType)
            {
                AddTip(sAttackData.MechaComponentBase, sAttackData.decHp, sAttackData.attackType, sAttackData.attackerMCB, sAttackData.elementType, null, sAttackData.elementHP);
            }

            private void HandleCommonTip(uint mcbGUID, UIBattleTipType type)
            {
                if ((int) type >= (int) UIBattleTipType.FollowDummySeparate)
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
                    Vector2.zero);
                CreateTip(info);
            }

            private void CreateTip(UIBattleTipInfo info)
            {
                UIBattleTip tip = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.UIBattleTip].AllocateGameObject<UIBattleTip>(UIManager.Instance.UI3DRoot);
                tip.
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