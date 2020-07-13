using System;
using BiangStudio.CloneVariant;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace GameCore.AbilityDataDriven
{
    public abstract class Action : IClone<Action>
    {
        [HideInInspector]
        public string ActionName => GetType().ToString();

        public Action Clone()
        {
            Type type = GetType();
            Action newAction = (Action) Activator.CreateInstance(type);
            ChildClone(newAction);
            return newAction;
        }

        protected virtual void ChildClone(Action newAction)
        {
        }
    }

    [LabelText("行为_赋予技能")]
    public class Action_AddAbility : Action
    {
        public ActionTarget Target;

        [LabelText("技能名称")]
        public string AbilityName;

        protected override void ChildClone(Action newAction)
        {
            base.ChildClone(newAction);
            Action_AddAbility action = ((Action_AddAbility) newAction);
            action.Target = Target.Clone();
            action.AbilityName = AbilityName;
        }
    }

    public class Action_ActOnTargets : Action
    {
        public ActionTarget Target;

        public Action Action;

        protected override void ChildClone(Action newAction)
        {
            base.ChildClone(newAction);
            Action_ActOnTargets action = ((Action_ActOnTargets) newAction);
            action.Target = Target.Clone();
            action.Action = Action.Clone();
        }
    }

    [LabelText("行为_赋予Modifier")]
    public class Action_ApplyModifier : Action
    {
        public ActionTarget Target;

        [LabelText("Modifier名称")]
        public string ModifierName;

        protected override void ChildClone(Action newAction)
        {
            base.ChildClone(newAction);
            Action_ApplyModifier action = ((Action_ApplyModifier) newAction);
            action.Target = Target.Clone();
            action.ModifierName = ModifierName;
        }
    }

    public abstract class Action_EmitProjectile : Action
    {
        [LabelText("投掷物")]
        public ProjectileConfig ProjectileConfig;

        [LabelText("投掷物表现体")]
        public ProjectileType ProjectileType;

        protected override void ChildClone(Action newAction)
        {
            base.ChildClone(newAction);
            Action_EmitProjectile action = ((Action_EmitProjectile) newAction);
            action.ProjectileType = ProjectileType;
        }

        [HideInInspector]
        [NonSerialized]
        public UnityAction<ProjectileInfo.FlyRealtimeData> OnHit;

        [HideInInspector]
        [NonSerialized]
        public UnityAction<ProjectileInfo.FlyRealtimeData> OnMiss;
    }

    [LabelText("行为_释放投掷物_瞬时直线")]
    public class Action_EmitProjectile_ImmediateLine : Action_EmitProjectile
    {
        [LabelText("射程")]
        [SuffixLabel("unit", true)]
        public int Range;

        protected override void ChildClone(Action newAction)
        {
            base.ChildClone(newAction);
            Action_EmitProjectile_ImmediateLine action = ((Action_EmitProjectile_ImmediateLine) newAction);
            action.Range = Range;
        }
    }

    [LabelText("行为_释放投掷物_延时直线")]
    public class Action_EmitProjectile_DelayLine : Action_EmitProjectile
    {
        [LabelText("射程")]
        [SuffixLabel("unit", true)]
        public int Range;

        [LabelText("初速度(unit/s)")]
        public Vector3 Velocity;

        [LabelText("加速度(unit/s^2)")]
        public Vector3 Acceleration;

        protected override void ChildClone(Action newAction)
        {
            base.ChildClone(newAction);
            Action_EmitProjectile_DelayLine action = ((Action_EmitProjectile_DelayLine) newAction);
            action.Range = Range;
            action.Velocity = Velocity;
            action.Acceleration = Acceleration;
        }
    }

    [LabelText("行为_造成伤害")]
    public class Action_DealDamage : Action
    {
        public ActionTarget Target;

        [LabelText("伤害量")]
        public int Damage;

        protected override void ChildClone(Action newAction)
        {
            base.ChildClone(newAction);
            Action_DealDamage action = ((Action_DealDamage) newAction);
            action.Target = Target.Clone();
            action.Damage = Damage;
        }
    }
}