using BiangStudio.GamePlay.UI;
using BiangStudio.ObjectPool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public enum AttackerType
    {
        None = 0,
        LocalPlayer = 1,
        Enemy = 2,
        Team = 3,

        NoTip = 100,
    }

    public enum BattleTipType
    {
        None = 0,
        Resist = 1, //抵抗//这个无用的
        Dodge = 2, //躲闪
        SequenceAttack = 3, //暴击-
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

    public class UIBattleTip : PoolObject
    {
        private float disappearTick = 0;
        UIBattleTipInfo UIBattleTipInfo;

        public Image Icon;
        public TextMeshPro TextType;
        public TextMeshPro TextContent;
        public TextMeshPro TextElementContent;

        public Animator Animator;

        public AnimCurve3D AnimCurve3D;
        public Gradient ColorDuringLife;

        protected Vector3 default_IconLocalPos = Vector3.zero;
        protected Vector3 default_TextTypeLocalPos = Vector3.zero;
        protected Vector3 default_TextContextLocalPos = Vector3.zero;
        protected Vector3 default_TextElementContextLocalPos = Vector3.zero;

        protected Vector3 offsetPos = new Vector3();

        public override void PoolRecycle()
        {
            Reset();
            base.PoolRecycle();
        }

        void Awake()
        {
            if (TextType) default_TextTypeLocalPos = TextType.transform.localPosition;
            if (TextContent) default_TextContextLocalPos = TextContent.transform.localPosition;
            if (TextElementContent) default_TextElementContextLocalPos = TextElementContent.transform.localPosition;
            if (Icon) default_IconLocalPos = Icon.transform.localPosition;
        }

        void Update()
        {
            if (!IsRecycled)
            {
                disappearTick += Time.deltaTime;
                if (disappearTick > UIBattleTipInfo.DisappearTime)
                {
                    PoolRecycle();
                }
                else
                {
                    RefreshColor(disappearTick / UIBattleTipInfo.DisappearTime);
                }
            }
        }

        private void Reset()
        {
            Animator.speed = 1;
            UIBattleTipInfo = null;
            disappearTick = 0;
            if (TextType)
            {
                TextType.transform.localPosition = default_TextTypeLocalPos;
                TextType.color = new Color(0,0,0,0);
            }

            if (TextContent)
            {
                TextContent.transform.localPosition = default_TextContextLocalPos;
                TextContent.color = new Color(0,0,0,0);
            }

            if (TextElementContent)
            {
                TextElementContent.transform.localPosition = default_TextElementContextLocalPos;
                TextElementContent.color = new Color(0,0,0,0);
            }

            if (Icon)
            {
                Icon.transform.localPosition = default_IconLocalPos;
                Icon.color = new Color(0,0,0,0);
            }
        }

        public void Initialize(UIBattleTipInfo info)
        {
            UIBattleTipInfo = info;
            disappearTick = 0;

            transform.localScale = Vector3.one * info.Scale;

            if (info.RandomRange.magnitude > 0)
            {
                offsetPos.x += Random.Range(-UIBattleTipInfo.RandomRange.x, UIBattleTipInfo.RandomRange.x);
                offsetPos.y += Random.Range(-UIBattleTipInfo.RandomRange.y, UIBattleTipInfo.RandomRange.y);
            }

            transform.localPosition = UIBattleTipInfo.StartPos;
            transform.rotation = Quaternion.LookRotation(transform.position - CameraManager.Instance.MainCamera.transform.position);

            SetTextType(TextType);
            SetTextContext(TextContent, info.DiffHP);
            SetElementTextContext(TextElementContent, info.ElementHP);

            Animator.SetTrigger("Play");
            float duration_ori = ClientUtils.GetClipLength(Animator, "AttackNumberTip");
            Animator.speed = Animator.speed * duration_ori / info.DisappearTime;
        }

        private void SetTextType(TextMeshPro text)
        {
            text.text = "";
            text.color = ColorDuringLife.Evaluate(0);
            text.transform.localPosition = default_TextContextLocalPos + offsetPos;
        }

        private void SetTextContext(TextMeshPro text, long diffHP)
        {
            text.text = diffHP.ToString();
            text.color = ColorDuringLife.Evaluate(0);
            text.transform.localPosition = default_TextContextLocalPos + offsetPos;
        }

        private void SetElementTextContext(TextMeshPro text, long diffHP)
        {
            if (diffHP == 0)
            {
                text.gameObject.SetActive(false);
            }
            else
            {
                text.gameObject.SetActive(true);
                text.text = diffHP.ToString();
            }

            text.color = ColorDuringLife.Evaluate(0);
            text.transform.localPosition = default_TextElementContextLocalPos + offsetPos;
        }

        private void RefreshColor(float timePortion)
        {
            if (TextType)
            {
                TextType.color = ColorDuringLife.Evaluate(timePortion);
            }

            if (TextContent)
            {
                TextContent.color = ColorDuringLife.Evaluate(timePortion);
            }

            if (TextElementContent)
            {
                TextElementContent.color = ColorDuringLife.Evaluate(timePortion);
            }
        }
    }
}