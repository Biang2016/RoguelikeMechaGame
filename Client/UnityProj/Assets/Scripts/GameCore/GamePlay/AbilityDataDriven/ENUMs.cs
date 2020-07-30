using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore.AbilityDataDriven
{
    [Flags]
    public enum ENUM_AbilityBehavior
    {
        /// <summary>
        /// Can be owned by a unit but can't be cast and won't show up on the HUD.
        /// </summary>
        [LabelText("隐式被动")]
        ABILITY_BEHAVIOR_PASSIVE_IMPLICIT = 1 << 0,

        /// <summary>
        /// Cannot be cast like above but this one shows up on the ability HUD.
        /// </summary>
        [LabelText("显式被动")]
        ABILITY_BEHAVIOR_PASSIVE_EXPLICIT = 1 << 1,

        /// <summary>
        /// Doesn't need a target to be cast, ability fires off as soon as the button is pressed.
        /// </summary>
        [LabelText("无需目标单位")]
        ABILITY_BEHAVIOR_NO_TARGET = 1 << 2,

        /// <summary>
        /// Needs a target to be cast on.
        /// </summary>
        [LabelText("需要目标单位")]
        ABILITY_BEHAVIOR_UNIT_TARGET = 1 << 3,

        /// <summary>
        /// Can be cast anywhere the mouse cursor is (if a unit is clicked it will just be cast where the unit was standing).
        /// </summary>
        [LabelText("指定目标点")]
        ABILITY_BEHAVIOR_POINT = 1 << 4,

        /// <summary>
        /// Has a direction from the hero, such as miranas arrow or pudge's hook.
        /// </summary>
        [LabelText("需指定释放方向")]
        ABILITY_BEHAVIOR_DIRECTIONAL = 1 << 5,

        /// <summary>
        /// Draws a radius where the ability will have effect. Kinda like POINT but with a an area of effect display.
        /// </summary>
        [LabelText("AOE")]
        ABILITY_BEHAVIOR_AOE = 1 << 6,

        /// <summary>
        /// Probably can be cast or have a casting scheme but cannot be learned (these are usually abilities that are temporary like techie's bomb detonate).
        /// </summary>
        [LabelText("不可学习")]
        ABILITY_BEHAVIOR_NOT_LEARNABLE = 1 << 7,

        /// <summary>
        /// Channeled ability. If the user moves or is silenced the ability is interrupted.
        /// </summary>
        [LabelText("吟唱技能")]
        ABILITY_BEHAVIOR_CHANNELLED = 1 << 8,

        /// <summary>
        /// Ability is tied up to an item.
        /// </summary>
        [LabelText("物品技能")]
        ABILITY_BEHAVIOR_ITEM = 1 << 9,

        /// <summary>
        /// Can be toggled.
        /// </summary>
        [LabelText("可开关")]
        ABILITY_BEHAVIOR_TOGGLE = 1 << 10,

        /// <summary>
        /// Can be cast automatically.
        /// </summary>
        [LabelText("自动释放√")]
        ABILITY_BEHAVIOR_AUTOCAST = 1 << 11,

        /// <summary>
        /// Ability has no reticle assist.
        /// </summary>
        [LabelText("无十字线辅助标准")]
        ABILITY_BEHAVIOR_NOASSIST = 1 << 12,

        /// <summary>
        /// Ability is an aura.  Not really used other than to tag the ability as such.
        /// </summary>
        [LabelText("光环效果")]
        ABILITY_BEHAVIOR_AURA = 1 << 13,

        /// <summary>
        /// Is an attack and cannot hit attack-immune targets.
        /// </summary>
        [LabelText("攻击技能")]
        ABILITY_BEHAVIOR_ATTACK = 1 << 14,

        /// <summary>
        /// Forbid movement during ability phase.
        /// </summary>
        [LabelText("阻断自身移动")]
        ABILITY_BEHAVIOR_FORBID_MOVEMENT = 1 << 15,

        /// <summary>
        /// Ability ignores backswing pseudoqueue.
        /// </summary>
        [LabelText("打断前序技能后摇")]
        ABILITY_BEHAVIOR_IGNORE_BACKSWING = 1 << 16,

        /// <summary>
        /// Can be used instantly without going into the action queue.
        /// </summary>
        [LabelText("瞬发，不进动作序列")]
        ABILITY_BEHAVIOR_IMMEDIATE = 1 << 17,

        /// <summary>
        /// Cannot be used when rooted
        /// </summary>
        [LabelText("受禁锢时无法释放")]
        ABILITY_BEHAVIOR_ROOT_DISABLES = 1 << 18,

        /// <summary>
        /// Ability is allowed when commands are restricted
        /// </summary>
        [LabelText("受封印时仍可释放")]
        ABILITY_BEHAVIOR_UNRESTRICTED = 1 << 19,

        /// <summary>
        /// Can be executed while stunned. Only applicable to toggled abilities.
        /// </summary>
        [LabelText("受眩晕时仍可释放")]
        ABILITY_BEHAVIOR_IGNORE_PSEUDO_QUEUE = 1 << 20,

        /// <summary>
        /// Does not alert enemies when target-cast on them.
        /// </summary>
        [LabelText("瞄准时不触发敌人警觉")]
        ABILITY_BEHAVIOR_DONT_ALERT_TARGET = 1 << 21,
    }

    public enum ENUM_ProjectileDummyPosition
    {
        [LabelText("无√")]
        None = 0,

        [LabelText("射击点√")]
        ShooterDummyPos,
    }

    /// <summary>
    /// Event for abilities. Only triggered by the caster of ability.
    /// </summary>
    public enum ENUM_AbilityEvent
    {
        [HideInInspector]
        ENUM_AbilityEvent_MIN = 5000,

        [LabelText("技能阶段开始时")]
        OnAbilityPhaseStart, // Triggers when the ability is cast (before the unit turns toward the target)

        [LabelText("技能开始时√")]
        OnAbilityStart,

        [LabelText("攻击时")]
        OnAttack,

        [LabelText("技能吟唱被打断时")]
        OnChannelInterrupted,

        [LabelText("技能吟唱成功时")]
        OnChannelSucceeded,

        [LabelText("技能吟唱结束时")]
        OnChannelFinish,

        [LabelText("创建时")]
        OnCreated,

        [LabelText("装备时")]
        OnEquip,

        [LabelText("受治疗时")]
        OnHealReceived,

        [LabelText("获得生命值时")]
        OnHealthGained,

        [LabelText("击杀单位时")]
        OnKilled,

        [LabelText("获得能量时")]
        OnPowerGained,

        [LabelText("宿主死亡时")]
        OnOwnerDied,

        [LabelText("宿主出生时")]
        OnOwnerSpawned,

        [LabelText("投掷物被阻挡时")]
        OnProjectileDodge,

        [LabelText("投掷物飞行结束时√")]
        OnProjectileFinish,

        [LabelText("投掷物击中单位时√")]
        OnProjectileHitUnit, //Adding the KV pair "DeleteOnHit" "0" in this block will cause the projectile to not disappear when it hits a unit.

        [LabelText("复活时")]
        OnRespawn,

        [LabelText("复活时")]
        OnSpellStart,

        [LabelText("消耗能量时")]
        OnSpentPower,

        [LabelText("状态改变时")]
        OnStateChanged,

        [LabelText("技能关闭时")]
        OnToggleOff,

        [LabelText("技能开启时")]
        OnToggleOn,

        [LabelText("单位移动时")]
        OnUnitMoved,

        [LabelText("升级时")]
        OnUpgrade,

        ENUM_AbilityEvent_MAX = 6000,
    }

    public enum ENUM_SingleTarget
    {
        [LabelText("施法者√")]
        CASTER,

        [LabelText("施法对象")]
        TARGET,

        [LabelText("目标点")]
        POINT,

        [LabelText("目标单位√")]
        UNIT,

        [LabelText("攻击者")]
        ATTACKER,
    }

    public enum ENUM_MultipleTargetCenter
    {
        [LabelText("施法者")]
        CASTER,

        [LabelText("施法对象")]
        TARGET,

        [LabelText("目标点")]
        POINT,

        [LabelText("目标单位")]
        UNIT,

        [LabelText("攻击者")]
        ATTACKER,

        [LabelText("投掷物√")]
        PROJECTILE,
    }

    public enum ENUM_MultipleTargetTeam
    {
        [LabelText("无√")]
        UNIT_TARGET_TEAM_NONE,

        [LabelText("全部√")]
        UNIT_TARGET_TEAM_BOTH,

        [LabelText("敌方√")]
        UNIT_TARGET_TEAM_ENEMY,

        [LabelText("友方√")]
        UNIT_TARGET_TEAM_FRIENDLY,
    }

    [Flags]
    public enum ENUM_MultipleTargetType
    {
        [LabelText("基本")]
        UNIT_TARGET_BASIC = 1 << 0,

        [LabelText("精英")]
        UNIT_TARGET_ELITE = 1 << 1,

        [LabelText("BOSS")]
        UNIT_TARGET_BOSS = 1 << 2,

        [LabelText("投掷物")]
        UNIT_TARGET_PROJECTILE = 1 << 3,

        [LabelText("建筑")]
        UNIT_TARGET_BUILDING = 1 << 4,
    }

    [Flags]
    public enum ENUM_MultipleTargetFlag
    {
        UNIT_TARGET_FLAG_CHECK_DISABLE_HELP,
        UNIT_TARGET_FLAG_DEAD,
        UNIT_TARGET_FLAG_FOW_VISIBLE,
        UNIT_TARGET_FLAG_INVULNERABLE,
        UNIT_TARGET_FLAG_MAGIC_IMMUNE_ENEMIES,
        UNIT_TARGET_FLAG_POWER_ONLY,
        UNIT_TARGET_FLAG_MELEE_ONLY,
        UNIT_TARGET_FLAG_NO_INVIS,
        UNIT_TARGET_FLAG_NONE,
        UNIT_TARGET_FLAG_NOT_ANCIENTS,
        UNIT_TARGET_FLAG_NOT_ATTACK_IMMUNE,
        UNIT_TARGET_FLAG_NOT_CREEP_HERO,
        UNIT_TARGET_FLAG_NOT_DOMINATED,
        UNIT_TARGET_FLAG_NOT_ILLUSIONS,
        UNIT_TARGET_FLAG_NOT_MAGIC_IMMUNE_ALLIES,
        UNIT_TARGET_FLAG_NOT_NIGHTMARED,
        UNIT_TARGET_FLAG_NOT_SUMMONED,
        UNIT_TARGET_FLAG_OUT_OF_WORLD,
        UNIT_TARGET_FLAG_PLAYER_CONTROLLED,
        UNIT_TARGET_FLAG_RANGED_ONLY,
    }

    [Flags]
    public enum ENUM_ModifierAttribute
    {
        [LabelText("支持叠加")]
        MODIFIER_ATTRIBUTE_STACK = 1 << 0,

        [LabelText("无视虚弱")]
        MODIFIER_ATTRIBUTE_IGNORE_INVULNERABLE = 1 << 1,

        [LabelText("常驻")]
        MODIFIER_ATTRIBUTE_PERMANENT = 1 << 2,
    }

    public enum ENUM_EffectAttachType
    {
        FOLLOW_ORIGIN,
        FOLLOW_OVERHEAD,
        START_AT_CUSTOM_ORIGIN,
        WORLD_ORIGIN
    }

    public enum ENUM_ModifierProperty
    {
        MODIFIER_PROPERTY_ABSOLUTE_NO_DAMAGE_MAGICAL,
        MODIFIER_PROPERTY_ABSOLUTE_NO_DAMAGE_PHYSICAL,
        MODIFIER_PROPERTY_ABSOLUTE_NO_DAMAGE_PURE,
        MODIFIER_PROPERTY_ABSORB_SPELL,
        MODIFIER_PROPERTY_ATTACK_RANGE_BONUS,
        MODIFIER_PROPERTY_ATTACKSPEED_BONUS_CONSTANT,
        MODIFIER_PROPERTY_ATTACKSPEED_BONUS_CONSTANT_POWER_TREADS,
        MODIFIER_PROPERTY_ATTACKSPEED_BONUS_CONSTANT_SECONDARY,
        MODIFIER_PROPERTY_AVOID_CONSTANT,
        MODIFIER_PROPERTY_AVOID_SPELL,
        MODIFIER_PROPERTY_BASEATTACK_BONUSDAMAGE,
        MODIFIER_PROPERTY_BASE_ATTACK_TIME_CONSTANT,
        MODIFIER_PROPERTY_BASEDAMAGEOUTGOING_PERCENTAGE,
        MODIFIER_PROPERTY_BASE_POWER_REGEN,
        MODIFIER_PROPERTY_BONUS_DAY_VISION,
        MODIFIER_PROPERTY_BONUS_NIGHT_VISION,
        MODIFIER_PROPERTY_BONUS_VISION_PERCENTAGE,
        MODIFIER_PROPERTY_CHANGE_ABILITY_VALUE,
        MODIFIER_PROPERTY_COOLDOWN_REDUCTION_CONSTANT,
        MODIFIER_PROPERTY_DAMAGEOUTGOING_PERCENTAGE,
        MODIFIER_PROPERTY_DAMAGEOUTGOING_PERCENTAGE_ILLUSION,
        MODIFIER_PROPERTY_DEATHGOLDCOST,
        MODIFIER_PROPERTY_DISABLE_AUTOATTACK,
        MODIFIER_PROPERTY_DISABLE_HEALING,
        MODIFIER_PROPERTY_DISABLE_TURNING, //unhandled                                                    
        MODIFIER_PROPERTY_EVASION_CONSTANT,
        MODIFIER_PROPERTY_EXTRA_HEALTH_BONUS,
        MODIFIER_PROPERTY_EXTRA_POWER_BONUS,
        MODIFIER_PROPERTY_EXTRA_STRENGTH_BONUS,
        MODIFIER_PROPERTY_FORCE_DRAW_MINIMAP,
        MODIFIER_PROPERTY_HEALTH_BONUS,
        MODIFIER_PROPERTY_HEALTH_REGEN_CONSTANT,
        MODIFIER_PROPERTY_HEALTH_REGEN_PERCENTAGE,
        MODIFIER_PROPERTY_IGNORE_CAST_ANGLE,
        MODIFIER_PROPERTY_INCOMING_DAMAGE_PERCENTAGE,
        MODIFIER_PROPERTY_INCOMING_PHYSICAL_DAMAGE_PERCENTAGE,
        MODIFIER_PROPERTY_INCOMING_SPELL_DAMAGE_CONSTANT,
        MODIFIER_PROPERTY_INVISIBILITY_LEVEL,
        MODIFIER_PROPERTY_IS_ILLUSION,
        MODIFIER_PROPERTY_IS_SCEPTER,
        MODIFIER_PROPERTY_LIFETIME_FRACTION,
        MODIFIER_PROPERTY_MAGICAL_RESISTANCE_BONUS,
        MODIFIER_PROPERTY_MAGICAL_RESISTANCE_DECREPIFY_UNIQUE,
        MODIFIER_PROPERTY_MAGICAL_RESISTANCE_ITEM_UNIQUE,
        MODIFIER_PROPERTY_POWER_BONUS,
        MODIFIER_PROPERTY_POWER_REGEN_CONSTANT,
        MODIFIER_PROPERTY_POWER_REGEN_CONSTANT_UNIQUE,
        MODIFIER_PROPERTY_POWER_REGEN_PERCENTAGE,
        MODIFIER_PROPERTY_POWER_REGEN_TOTAL_PERCENTAGE,
        MODIFIER_PROPERTY_MIN_HEALTH,
        MODIFIER_PROPERTY_MISS_PERCENTAGE,
        MODIFIER_PROPERTY_MODEL_CHANGE,
        MODIFIER_PROPERTY_MODEL_SCALE, // unhandled                                                           
        MODIFIER_PROPERTY_MOVESPEED_ABSOLUTE,
        MODIFIER_PROPERTY_MOVESPEED_BASE_OVERRIDE,
        MODIFIER_PROPERTY_MOVESPEED_BONUS_CONSTANT,
        MODIFIER_PROPERTY_MOVESPEED_BONUS_PERCENTAGE,
        MODIFIER_PROPERTY_MOVESPEED_BONUS_PERCENTAGE_UNIQUE,
        MODIFIER_PROPERTY_MOVESPEED_BONUS_UNIQUE,
        MODIFIER_PROPERTY_MOVESPEED_LIMIT, // unhandled                                                       
        MODIFIER_PROPERTY_MOVESPEED_MAX, // unhandled                                                         
        MODIFIER_PROPERTY_OVERRIDE_ANIMATION,
        MODIFIER_PROPERTY_OVERRIDE_ANIMATION_RATE,
        MODIFIER_PROPERTY_OVERRIDE_ANIMATION_WEIGHT,
        MODIFIER_PROPERTY_OVERRIDE_ATTACK_MAGICAL,
        MODIFIER_PROPERTY_PERSISTENT_INVISIBILITY,
        MODIFIER_PROPERTY_PHYSICAL_ARMOR_BONUS,
        MODIFIER_PROPERTY_PHYSICAL_ARMOR_BONUS_ILLUSIONS,
        MODIFIER_PROPERTY_PHYSICAL_ARMOR_BONUS_UNIQUE,
        MODIFIER_PROPERTY_PHYSICAL_ARMOR_BONUS_UNIQUE_ACTIVE,
        MODIFIER_PROPERTY_PHYSICAL_CONSTANT_BLOCK,
        MODIFIER_PROPERTY_POST_ATTACK,
        MODIFIER_PROPERTY_PREATTACK_BONUS_DAMAGE,
        MODIFIER_PROPERTY_PREATTACK_BONUS_DAMAGE_POST_CRIT,
        MODIFIER_PROPERTY_PREATTACK_CRITICALSTRIKE,
        MODIFIER_PROPERTY_PROCATTACK_BONUS_DAMAGE_COMPOSITE,
        MODIFIER_PROPERTY_PROCATTACK_BONUS_DAMAGE_MAGICAL,
        MODIFIER_PROPERTY_PROCATTACK_BONUS_DAMAGE_PHYSICAL,
        MODIFIER_PROPERTY_PROCATTACK_BONUS_DAMAGE_PURE,
        MODIFIER_PROPERTY_PROCATTACK_FEEDBACK,
        MODIFIER_PROPERTY_PROVIDES_FOW_POSITION,
        MODIFIER_PROPERTY_REINCARNATION,
        MODIFIER_PROPERTY_RESPAWNTIME,
        MODIFIER_PROPERTY_RESPAWNTIME_PERCENTAGE,
        MODIFIER_PROPERTY_RESPAWNTIME_STACKING,
        MODIFIER_PROPERTY_STATS_AGILITY_BONUS,
        MODIFIER_PROPERTY_STATS_INTELLECT_BONUS,
        MODIFIER_PROPERTY_STATS_STRENGTH_BONUS,
        MODIFIER_PROPERTY_TOOLTIP,
        MODIFIER_PROPERTY_TOTAL_CONSTANT_BLOCK,
        MODIFIER_PROPERTY_TOTAL_CONSTANT_BLOCK_UNAVOIDABLE_PRE_ARMOR,
        MODIFIER_PROPERTY_TOTALDAMAGEOUTGOING_PERCENTAGE, // unhandled                                  
        MODIFIER_PROPERTY_TRANSLATE_ACTIVITY_MODIFIERS,
        MODIFIER_PROPERTY_TRANSLATE_ATTACK_SOUND,
        MODIFIER_PROPERTY_TURN_RATE_PERCENTAGE,
    }

    public enum ENUM_ModifierStates
    {
        MODIFIER_STATE_ATTACK_IMMUNE,
        MODIFIER_STATE_BLIND,
        MODIFIER_STATE_BLOCK_DISABLED,
        MODIFIER_STATE_CANNOT_MISS,
        MODIFIER_STATE_COMMAND_RESTRICTED,
        MODIFIER_STATE_DISARMED,
        MODIFIER_STATE_DOMINATED,
        MODIFIER_STATE_EVADE_DISABLED,
        MODIFIER_STATE_FLYING,
        MODIFIER_STATE_FLYING_FOR_PATHING_PURPOSES_ONLY,
        MODIFIER_STATE_FROZEN,
        MODIFIER_STATE_HEXED,
        MODIFIER_STATE_INVISIBLE,
        MODIFIER_STATE_INVULNERABLE,
        MODIFIER_STATE_LOW_ATTACK_PRIORITY,
        MODIFIER_STATE_MAGIC_IMMUNE,
        MODIFIER_STATE_MUTED,
        MODIFIER_STATE_NIGHTMARED,
        MODIFIER_STATE_NO_HEALTH_BAR,
        MODIFIER_STATE_NO_TEAM_MOVE_TO,
        MODIFIER_STATE_NO_TEAM_SELECT,
        MODIFIER_STATE_NOT_ON_MINIMAP,
        MODIFIER_STATE_NOT_ON_MINIMAP_FOR_ENEMIES,
        MODIFIER_STATE_NO_UNIT_COLLISION,
        MODIFIER_STATE_OUT_OF_GAME,
        MODIFIER_STATE_PASSIVES_DISABLED,
        MODIFIER_STATE_PROVIDES_VISION,
        MODIFIER_STATE_ROOTED,
        MODIFIER_STATE_SILENCED,
        MODIFIER_STATE_SOFT_DISARMED,
        MODIFIER_STATE_SPECIALLY_DENIABLE,
        MODIFIER_STATE_STUNNED,
        MODIFIER_STATE_UNSELECTABLE,
    }

    public enum ENUM_ModifierEvent
    {
        ENUM_ModifierEvent_MIN = 6001,

        [LabelText("技能释放时")]
        OnAbilityExecuted,

        [LabelText("受击时")]
        OnAttacked, //The unit this modifier is attached to has been attacked.

        [LabelText("攻击命中时")]
        OnAttackLanded, //The unit this modifier is attached to has landed an attack on a target. "%attack_damage" is set to the damage value before mitigation. Autoattack damage is dealt after this block executes.

        /// <summary>
        /// The unit this modifier is attached to has started to attack a target (when the attack animation begins, not when the autoattack projectile is created).
        /// </summary>
        [LabelText("攻击起手时")]
        OnAttackStart,

        /// <summary>
        /// The modifier has been created.
        /// </summary>
        [LabelText("Modifier创建时")]
        OnCreated,

        /// <summary>
        /// The unit this modifier is attached to has dealt damage. "%attack_damage" is set to the damage value after mitigation.
        /// </summary>
        [LabelText("造成伤害时")]
        OnDealDamage,

        /// <summary>
        /// The unit this modifier is attached to has died.
        /// </summary>
        [LabelText("宿主死亡时")]
        OnDeath,

        /// <summary>
        /// The modifier has been removed.
        /// </summary>
        [LabelText("Modifier移除时")]
        OnDestroy,

        /// <summary>
        /// The interval ticker ticks.
        /// </summary>
        [LabelText("计时器触发时")]
        OnIntervalTicker,

        /// <summary>
        /// The unit this modifier is attached to has killed other unit.
        /// </summary>
        [LabelText("击杀敌方时")]
        OnKill,

        /// <summary>
        /// The unit this modifier is attached to has taken damage. "%attack_damage" is set to the damage value after mitigation.
        /// </summary>
        [LabelText("受到伤害时")]
        OnTakeDamage,

        ENUM_ModifierEvent_MAX = 7000,
    }

    public enum ENUM_ProjectileType
    {
    }
}