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

        public Image spriteIcon;
        public TextMeshProUGUI spriteTextType;
        public TextMeshProUGUI spriteTextContext;
        public Image spriteTextElementContext;

        public Animator animator;

        protected Vector3 contextLocalPos = Vector3.zero;
        protected Vector3 contextElementLocalPos = Vector3.zero;
        protected Vector3 typeLocalPos = Vector3.zero;
        protected Vector3 iconLocalPos = Vector3.zero;

        protected Color color = new Color();
        protected Vector3 offsetPos = new Vector3();

        public override void PoolRecycle()
        {
            base.PoolRecycle();
            UIBattleTipInfo = null;
        }

        void Awake()
        {
            if (spriteTextContext != null)
            {
                contextLocalPos = spriteTextContext.transform.localPosition;
            }

            if (spriteTextElementContext != null)
            {
                contextElementLocalPos = spriteTextElementContext.transform.localPosition;
            }

            if (spriteTextType != null)
            {
                typeLocalPos = spriteTextType.transform.localPosition;
            }

            if (spriteIcon != null)
            {
                iconLocalPos = spriteIcon.transform.localPosition;
            }
        }

        public void Initialize(UIBattleTipInfo info)
        {
            UIBattleTipInfo = info;
            disappearTick = 0;

            float scaleVal = info.Scale;
            transform.localScale.Set(scaleVal, scaleVal, scaleVal); 
        
            if (info.RandomRange.magnitude > 0)
            {
                RandomPos();
            }

            UpdatePos();
            //transform.localPosition = attackParam.startPos;

            int page = (int) attackParam.cfg.PageNum;
            bool changeClr = attackParam.acttackerType != (uint) AttackerType.LocalPlayer;

            bool hadType = UpdateSpriteByText(spriteTextType, attackParam.cfg.Note, ref typeLocalPos, page, changeClr);

            //if (attackParam.cfg.Note != "A" && attackParam.cfg.Note != "B" && attackParam.cfg.Note != "C")
            UpdateSpriteByText(spriteIcon, attackParam.cfg.PreNote, ref iconLocalPos, page, changeClr);

            SetContextSprite(spriteTextContext, attackParam.diffHP, hadType, page, changeClr);
            SetContextElementSprite(spriteTextElementContext, attackParam.elementHP, hadType, page, changeClr);

            UpdateSpriteByText(spriteWhiteTextType, attackParam.cfg.Note, ref typeLocalPos);

            //if (attackParam.cfg.Note != "A" && attackParam.cfg.Note != "B" && attackParam.cfg.Note != "C")
            UpdateSpriteByText(spriteWhiteIcon, attackParam.cfg.PreNote, ref iconLocalPos);

            SetContextSprite(spriteWhiteTextContext, attackParam.diffHP, hadType);
            SetContextElementSprite(spriteWhiteTextElementContext, attackParam.elementHP, hadType);

            ResetspriteImageIcon();

            PlayAnim();
        }

        void SetContextSprite(SpriteText3D sprite, long diffHP, bool hadType, int page = -1, bool chagneClr = true)
        {
            if (sprite != null)
            {
                if (page >= 0)
                    sprite.Page = page;
                sprite.text = diffHP.ToString();

                if (chagneClr)
                {
                    sprite.color = color;
                }
                else
                {
                    sprite.color = Color.white;
                }

                //spriteTextContext.transform.localPosition = (hadType ? typeLocalPos : contextLocalPos) + offsetPos;
                var pos = (hadType ? contextLocalPos : typeLocalPos) + offsetPos;
                sprite.transform.SetLocalPosition(pos.x, pos.y, pos.z);
            }
        }

        void SetContextElementSprite(SpriteText3D sprite, long diffHP, bool hadType, int page = -1, bool chagneClr = true)
        {
            if (sprite != null)
            {
                if (page >= 0)
                    sprite.Page = page;
                if (diffHP == 0)
                {
                    sprite.gameObject.CustomSetActive(false);
                }
                else
                {
                    sprite.gameObject.CustomSetActive(true);
                    sprite.text = diffHP.ToString();
                }

                if (chagneClr)
                {
                    sprite.color = color;
                }
                else
                {
                    sprite.color = Color.white;
                }

                var pos = (hadType ? contextElementLocalPos : typeLocalPos) + offsetPos;
                sprite.transform.SetLocalPosition(pos.x, pos.y, pos.z);
            }
        }

        int RandomSquare(int val)
        {
            return Random.Range(-val, val);
        }

        void RandomCircle(int radius, ref Vector3 lastOffset)
        {
            float quadRadius = attackParam.cfg.SquareX * 0.001f;
            float square = quadRadius;
            for (int i = 0; i < 100; ++i)
            {
                int val = Random.Range(-radius, radius);
                float diffX = lastOffset.x + val * 0.001f;
                if (diffX < offsetPos.x + square && diffX > offsetPos.x - square)
                {
                    float diffY = lastOffset.y + val * 0.001f;
                    if (diffY < offsetPos.y + square && diffY > offsetPos.y - square)
                    {
                        offsetPos.x = lastOffset.x + val * 0.001f;
                        offsetPos.y = lastOffset.y + val * 0.001f;
                        //Logger.Log("tip:RandomCircle:" + offsetPos.ToString());
                        return;
                    }
                }
            }

            offsetPos = lastOffset;
        }

        void RandomPos()
        {
            TipInfo tipInfo = UIBattleTipInfo.UIBattleTipManager.GetTipInfo(attackParam.hitIdent, attackParam.attackType);
            if (tipInfo == null)
            {
                int offset = RandomSquare((int) attackParam.cfg.SquareX);
                offsetPos.x += offset * 0.001f;
                offsetPos.y += offset * 0.001f;
                //Logger.Log("tip:RandomSquare:" + offsetPos.ToString());
                UIBattleTipInfo.UIBattleTipManager.AddTipInfo(attackParam.hitIdent, attackParam.attackType, ref offsetPos);
            }
            else
            {
                RandomCircle((int) attackParam.cfg.Radius, ref tipInfo.LastOffset);
                tipInfo.SetInfo(tipInfo.ident, tipInfo.attackType, ref offsetPos);
            }
        }

        void UpdatePos()
        {
            if (CameraManager.Instance.MainCamera != null && UIManager.Instance.UICamera != null)
            {
                if (UIBattleTipInfo != null && !UIBattleTipInfo.InScreenCenter)
                {
                    Vector3 pos = CameraManager.Instance.MainCamera.WorldToScreenPoint(UIBattleTipInfo.StartPos);
                    transform.localPosition = UIManager.Instance.UICamera.ScreenToWorldPoint(pos);
                }
                else
                {
                    transform.localPosition = Vector3.zero;
                }
            }
        }
    }
}