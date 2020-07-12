using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;


namespace GameCore.AbilityDataDriven
{
    public abstract class Action
    {
        [HideInInspector]
        public string ActionName => GetType().ToString();
    }

    [LabelText("行为_造成伤害")]
    public class Action_DealDamage : Action
    {
        public ActionTarget Target;

        [LabelText("伤害量")]
        public int Damage;
    }
    
    [LabelText("行为_赋予技能")]
    public class Action_AddAbility : Action
    {
        public ActionTarget Target;

        [LabelText("技能名称")]
        public string AbilityName;
    }

    public class Action_ActOnTargets : Action
    {
        public ActionTarget Target;

        public Action Action;
    }

    [LabelText("行为_赋予Modifier")]
    public class Action_ApplyModifier : Action
    {
        public ActionTarget Target;

        [LabelText("Modifier名称")]
        public string ModifierName;
    }

    public abstract class Action_EmitProjectile : Action
    {
        [LabelText("投掷物预制体路径")]
        public string ProjectilePrefabPath;
    }

    [LabelText("行为_释放投掷物_瞬时直线")]
    public class Action_EmitProjectile_ImmediateLine : Action_EmitProjectile
    {
        [LabelText("射程")]
        [SuffixLabel("unit", true)]
        public int Range;
    }

    [LabelText("行为_释放投掷物_延时直线")]
    public class Action_EmitProjectile_DelayLine : Action_EmitProjectile
    {
        [LabelText("射程")]
        [SuffixLabel("unit", true)]
        public int Range;

        [LabelText("初速度")]
        [SuffixLabel("unit/s", true)]
        public int Speed;

        [LabelText("加速度")]
        [SuffixLabel("unit/s2", true)]
        public int Acceleration;
    }
}