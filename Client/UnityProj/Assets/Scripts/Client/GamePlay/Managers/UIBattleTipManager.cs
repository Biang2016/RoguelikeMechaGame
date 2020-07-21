using System.Collections.Generic;
using BiangStudio.GamePlay.UI;
using BiangStudio.Messenger;
using BiangStudio.Singleton;
using GameCore;
using UnityEngine;

namespace Client
{
    public class TipInfo
    {
        const float step = 1.0f;
        public ulong ident;
        public UIBattleTipType attackType;
        public float startTime;
        public Vector3 lastOffset;
        public bool isValid = false;

        public void Clear()
        {
            isValid = false;
        }

        public bool IsEqual(ulong id, UIBattleTipType type)
        {
            if (isValid)
            {
                return (ident == id) && (type == attackType);
            }

            return false;
        }

        public void SetInfo(ulong id, UIBattleTipType type, ref Vector3 pos)
        {
            ident = id;
            attackType = type;
            lastOffset = pos;
            isValid = true;
            startTime = Time.time;
        }

        public void Tick(float time)
        {
            if (isValid && time > startTime + step)
            {
                isValid = false;
            }
        }
    }

    public enum UIBattleTipType
    {
        None = 0,
        Resist = 1, //抵抗//这个无用的
        Dodge = 2, //躲闪
        CriticalAttack = 3, //暴击-
        Damage = 4, //普通伤害-
        Rampage = 5, //暴走 //这个无用的-
        MeleekillScore = 6, //近身击杀得分//这个无用的-
        MpTip = 7, //得到能量//这个无用的-
        AddHp = 8, //加血 //这个无用的-

        AddLife = 9, //生命 +/- xxx
        Stun = 10, //晕眩
        Shield = 11, //护盾
        Hiding = 12, //隐身
        SpeedUp = 13, //加速
        SlowDown = 14, //减速
        Defense = 15, //防御 +/- xxx
        Attack = 16, //攻击 +/- xxx
        Invincible = 17, //无敌
        Frozen = 18, // 冰冻
        Poison = 19, //中毒
        Firing = 20, //灼烧
        SecKill = 21, //秒杀----
        Bullet = 22, //弹药 +/- xxx
        ExtraDamage = 24, //额外伤害 +/- xxx
        PickWeapon = 25,
        Parry = 26, //格挡

        RaceDamage = 27, //种族伤害, 加深伤害

        GetBuff = 28, //获得Buff
        GetBestBuff = 29, //获得强力BUFF
        UpLevel = 31, //升级，大幅提升武器伤害

        InvincibleSpirit = 32, //不屈意志！
        Overloading = 33, //超载！
        AirAttack = 34, //空袭！
        BoomShield = 35, //防护盾！
        Thud = 36, //震击
        Quickness = 37, //急速
        WarmBlood = 38, //热血
        NerveGas = 39, //神经毒气
        Sober = 40, //清醒
        Discipline = 41, //惩戒
        EleInterference = 42, //电磁干扰
        FastHatch = 43, //快速填充
        LevelIncrease = 44, //等级提升
        LeaderAppeared = 45, //首领现身
        LeaderKilled = 46, //首领已被击杀
        WeaponOutbreak = 47, //武器爆发
        BeFound = 48, //被发现, 【通用机制】被敌兵发现时增加飘字“被发现”
        MpDamage = 49, //【通用机制】MP盾特殊飘字 MP伤害

        ParticalCannon = 51, //粒子炮
        DevilBomb = 52, //恶魔炸弹
        IonArc = 52, //离子电弧
        AbsolutenessDomain = 53, //绝对领域
        UW8AddGold = 56, //【水下8关】【单局】【敌兵掉落】敌兵掉落表现
        PlaySoul = 57, //播放枪魂
        ExpNum = 61, // Exp + xx

        NoDamage = 66, //1.10英雄天赋（跨版本）：固定值伤害减伤BUFF storyID:61774891 BUFF持续时间内，每次受到的伤害减少固定值例如1000（伤害最低减少到0），减少到0的时候免疫此次伤害并飘字“免疫”提示玩家

        SafeZone = 75, // 吃鸡毒圈

        NoAttackSeparate = 200,
        AddScore = 201, //得分
        SwitchWeapon = 202, //切换武器时飘字
        GetOverlap = 203, //获取叉乘
        BulletTimeAttckTip = 204, //子弹时间暴击提示
        DesignTest = 205,

        ScreenCenterSeparate = 300, //不取挂点，直接获取屏幕中心
        BulletTimeTip = 301, //子弹时间屏幕中心得示
        ScreenCenterTest2 = 302,
        ScreenCenterTest3 = 303,

        FollowDummySeparate = 400, //取挂点，且一直随着挂点移动
        BulletTimeTutorialTip = 401, //新手引导用的子弹时间
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

        public void Setup(Vector3 pos, ulong id, UIBattleTipType _attackType, long _diff, uint _attackerType = 0, float _scale = 1.0f, int _elementType = 0, string _imagePath = "", long _elementHP = 0)
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

        public bool IsEqual(UIBattleTipParam param)
        {
            return (param.attackerType == attackerType) && (param.attackType == attackType);
        }
    }

    public class UIBattleTipManager : TSingleton<UIBattleTipManager>
    {
        private Messenger Messenger => ClientGameManager.Instance.BattleMessenger;
        private const int maxTipNum = 50;
        private List<UIBattleTip> TipList = new List<UIBattleTip>(maxTipNum);
        private List<UIBattleTipParam> ParamList = new List<UIBattleTipParam>();
        private List<TipInfo> TipInfoList = new List<TipInfo>();

        private float curTime = 0.0f;

        public void Setup()
        {
            RegisterEvent();
            curTime = Time.time + 2.0f;
        }

        public void Shutdown()
        {
            TipInfoList.Clear();
            ClearTipList(TipList);
            UnRegisterEvent();
        }

        private void RegisterEvent()
        {
            Messenger.AddListener<SAttackData>((uint) ENUM_BattleEvent.Battle_MechaComponentAttackTip, HandleAttackTip);
            Messenger.AddListener<MechaComponentBase, int>((uint) ENUM_BattleEvent.Battle_AddScoreTip, HandleAddScore);
            Messenger.AddListener<MechaComponentBase, int>((uint) ENUM_BattleEvent.Battle_SkillTip, HandleAgeConfigTip);
            Messenger.AddListener<uint, UIBattleTipType>((uint) ENUM_BattleEvent.Battle_CommonTip, HandleCommonTip);
        }

        private void UnRegisterEvent()
        {
            Messenger.RemoveListener<SAttackData>((uint) ENUM_BattleEvent.Battle_MechaComponentAttackTip, HandleAttackTip);
            Messenger.RemoveListener<MechaComponentBase, int>((uint) ENUM_BattleEvent.Battle_AddScoreTip, HandleAddScore);
            Messenger.RemoveListener<MechaComponentBase, int>((uint) ENUM_BattleEvent.Battle_SkillTip, HandleAgeConfigTip);
            Messenger.RemoveListener<uint, UIBattleTipType>((uint) ENUM_BattleEvent.Battle_CommonTip, HandleCommonTip);
        }

        private ulong GetHitActorID(MechaComponentBase hitter)
        {
            if (hitter != null)
            {
                return hitter.MechaComponentInfo.GUID;
            }

            return 0;
        }

        private void HandleAddScore(MechaComponentBase mcb, int diffHP)
        {
            Debug.Log($"HandleAddScore：score:{diffHP}");
            AddTip(mcb, diffHP, UIBattleTipType.AddScore, null);
        }

        private void HandleAgeConfigTip(MechaComponentBase mcb, int type)
        {
            AddTip(mcb, 0, (UIBattleTipType) type, null);
        }

        private void HandleAttackTip(SAttackData sAttackData)
        {
            AddTip(sAttackData.MechaComponentBase, sAttackData.decHp, sAttackData.attackType, sAttackData.attackerMCB, sAttackData.elementType, null, sAttackData.elementHP);
        }

        private void HandleCommonTip(uint mcbGUID, UIBattleTipType type)
        {
            if ((int) type >= (int) UIBattleTipType.FollowDummySeparate)
            {
                return;
            }

            uint attackerType = (uint) EAttackerType.None;

            MechaComponentBase mcb_attacker = ClientBattleManager.Instance.FindMechaComponentBase(mcbGUID);
            if (ClientBattleManager.Instance.PlayerMecha != null && mcb_attacker != null)
            {
                attackerType = GetAttackerType(mcb_attacker, ClientBattleManager.Instance.PlayerMecha, type);
            }

            if (attackerType == (uint) EAttackerType.NoTip)
            {
                return;
            }

            UIBattleTipParam param = CreateParam();
            param.Setup(GetPos(mcb_attacker, type), GetHitActorID(mcb_attacker), type, 0, attackerType, 1, 0);
            CreateTip(param);
        }

        private void ClearTipList(List<UIBattleTip> list)
        {
            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (list[i] != null)
                {
                    list[i].Shutdown();
                    GameObject.Destroy(list[i].gameObject);
                }
            }

            list.Clear();
        }

        private Vector3 GetPos(MechaComponentBase mcb, UIBattleTipType attackType)
        {
            if ((int) attackType >= (int) UIBattleTipType.ScreenCenterSeparate)
            {
                return Vector3.zero;
            }

            if (mcb != null)
            {
                return mcb.transform.position + Vector3.up * 3;
            }

            return Vector3.zero;
        }

        public void Tick(float deltaTime)
        {
            if (curTime < Time.time)
            {
                curTime = Time.time + 2;
                //  AddTip(PlayerCamp.Instance.Player,2,AttackType.Damage);
            }

            for (int i = TipList.Count - 1; i >= 0; --i)
            {
                if (TipList[i] != null && TipList[i].Tick(deltaTime))
                {
                    int lastIndex = TipList.Count - 1;
                    ReplaceLast(TipList, i);
                    RecycledTipList.Add(TipList[lastIndex]);
                    TipList.RemoveAt(lastIndex);
                }
            }

            TickTipInfo();
        }

        private void ReplaceLast<T>(List<T> _listTip, int index) where T : class
        {
            int lastIndex = _listTip.Count - 1;
            if (lastIndex != 0 && index != lastIndex)
            {
                var item = _listTip[index];
                _listTip[index] = _listTip[lastIndex];
                _listTip[lastIndex] = item;
            }
        }

        public void AddTip(MechaComponentBase mcb, long diffHp, UIBattleTipType attackType, MechaComponentBase attacker, int elementType = 0, string image = "", long elementHP = 0)
        {
            if ((int) attackType >= (int) UIBattleTipType.FollowDummySeparate)
            {
                return;
            }

            uint attackerType = GetAttackerType(attacker, mcb, attackType);
            if (attackerType == (uint) EAttackerType.NoTip)
            {
                return;
            }

            UIBattleTipParam param = CreateParam();
            param.Setup(GetPos(mcb, attackType), GetHitActorID(mcb), attackType, diffHp, attackerType, GetScale(attacker, mcb), elementType, image, elementHP);
            CreateTip(param);
        }

        //攻击者类型
        private uint GetAttackerType(MechaComponentBase attacker, MechaComponentBase hitter, UIBattleTipType attackType)
        {
            if (hitter != null && hitter.ActorDataComponent != null && hitter.ActorDataComponent.ActorConfig != null)
            {
                if (hitter.ActorDataComponent.ActorConfig.TipShowType == 0)
                    return (uint) EAttackerType.NoTip;
            }

            //不走攻击类型判定
            if ((int) attackType > (int) UIBattleTipType.NoAttackSeparate)
            {
                return (uint) EAttackerType.None;
            }

            if (attacker != null)
            {
                //主角
                if (attacker.IsLocalPlayer())
                {
                    return (uint) EAttackerType.LocalPlayer;
                }

                //同个阵营
                if (hitter != null && attacker.IsFriend(hitter))
                {
                    return (uint) EAttackerType.NoTip;
                }

                //队友
                if (attacker.IsMainPlayerFriend())
                {
                    return (uint) EAttackerType.Team;
                }

                //敌人
                if (hitter != null && attacker.IsOpponent(hitter))
                {
                    return (uint) EAttackerType.Enemy;
                }

                //if (attacker.PlayerAsEnemy)
                //    return (uint)EAttackerType.Enemy;
            }

            else
            {
                //这种可能是自杀
                if (hitter != null && hitter.HpHelper != null && hitter.HpHelper.CurHp == 0)
                {
                    return (uint) EAttackerType.NoTip;
                }
            }

            return (uint) EAttackerType.None;
        }

        private float GetScale(MechaComponentBase attacker, MechaComponentBase hitter)
        {
            if (attacker != null && attacker.MechaInfo.IsPlayer)
                return 1.0f;
            if (hitter != null && hitter.MechaInfo.IsPlayer)
                return 1.0f;
            return 0.8f;
        }

        private UIBattleTipParam CreateParam()
        {
            if (ParamList.Count > 0)
            {
                UIBattleTipParam param = ParamList[ParamList.Count - 1];
                ParamList.RemoveAt(ParamList.Count - 1);
                return param;
            }

            return new UIBattleTipParam();
        }

        private UIBattleTip CreateTip(UIBattleTipParam param)
        {
            pbUIBattleTipCfg cfg = ConfigManager.Instance.GetConfigWithKeys<pbUIBattleTipCfg>(ConfigFileList.UIBattleTipCfg, (uint) param.attackType, param.acttackerType, 0, (uint) param.elementType);
            if (cfg != null && cfg.Path != null)
            {
                param.cfg = cfg;
                UIBattleTip tip = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.UIBattleTip].AllocateGameObject<UIBattleTip>(UIManager.Instance.UI3DRoot);
                if (tip != null)
                {
                    tip.Setup(param, this);
                    TipList.Add(tip);
                    return tip;
                }

                LoaderWrapHelper.Instance.InstantiateParam<UIBattleTipParam>(cfg.Path, param).Completed += LoadAttackTipCallBack;

                if (op == null)
                {
                    Debug.LogError("UIBattleTipManager 加载UIAttack资源失败!");
                    return;
                }

                if (op.Result != null)
                {
                    var tipGo = op.Result;
                    if (tipGo != null)
                    {
                        tipGo.transform.parent = UIManager.Instance.UI3DRoot;
                        tipGo.transform.localPosition = Vector3.zero;
                        var tip = tipGo.GetComponent<UIBattleTip>();
                        if (tip != null)
                        {
                            tip.Setup(op.Param, this);
                            TipList.Add(tip);
                            ParamList.Add(op.Param);
                        }
                    }
                }

                else
                {
                    op.Release();
                    Debug.LogError("UIBattleTipManager 加载UIAttack资源失败!");
                    return;
                }

                op.Release();
            }

            return null;
        }

        public TipInfo GetTipInfo(ulong ident, UIBattleTipType attackType)
        {
            for (int i = 0; i < TipInfoList.Count; ++i)
            {
                if (TipInfoList[i] != null && TipInfoList[i].IsEqual(ident, attackType))
                {
                    return TipInfoList[i];
                }
            }

            return null;
        }

        public void AddTipInfo(ulong id, UIBattleTipType attackType, ref Vector3 offsetPos)
        {
            for (int i = 0; i < TipInfoList.Count; ++i)
            {
                if (TipInfoList[i] != null && !TipInfoList[i].isValid)
                {
                    TipInfoList[i].SetInfo(id, attackType, ref offsetPos);
                    return;
                }
            }

            TipInfo tipInfo = new TipInfo();
            tipInfo.SetInfo(id, attackType, ref offsetPos);
            TipInfoList.Add(tipInfo);
        }

        public void TickTipInfo()
        {
            float curTime = Time.time;
            for (int i = 0; i < TipInfoList.Count; ++i)
            {
                if (TipInfoList[i] != null)
                {
                    TipInfoList[i].Tick(curTime);
                }
            }
        }
    }
}