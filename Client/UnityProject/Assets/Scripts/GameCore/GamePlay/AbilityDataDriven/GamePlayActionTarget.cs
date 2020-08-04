using System;
using BiangStudio.CloneVariant;
using Sirenix.OdinInspector;

namespace GameCore.AbilityDataDriven
{
    [LabelText("执行目标")]
    public abstract class GamePlayActionTarget : IClone<GamePlayActionTarget>
    {
        public GamePlayActionTarget Clone()
        {
            Type type = GetType();
            GamePlayActionTarget newActionTarget = (GamePlayActionTarget) Activator.CreateInstance(type);
            ChildClone(newActionTarget);
            return newActionTarget;
        }

        protected virtual void ChildClone(GamePlayActionTarget newActionTarget)
        {
        }
    }

    public class SingleActionTarget : GamePlayActionTarget
    {
        [LabelText("目标")]
        public ENUM_SingleTarget Target;

        protected override void ChildClone(GamePlayActionTarget newActionTarget)
        {
            base.ChildClone(newActionTarget);
            ((SingleActionTarget) newActionTarget).Target = Target;
        }
    }

    public class MultipleActionTarget : GamePlayActionTarget
    {
        [LabelText("搜索中心")]
        public ENUM_MultipleTargetCenter Center;

        [LabelText("范围半径")]
        [SuffixLabel("unit", true)]
        public int Radius;

        [LabelText("阵营筛选")]
        public ENUM_MultipleTargetTeam Team;

        [BoxGroup("单位类型")]
        [LabelText("选中")]
        public ENUM_MultipleTargetType Types;

        [BoxGroup("单位类型")]
        [LabelText("剔除")]
        public ENUM_MultipleTargetType ExcludeTypes;

        [BoxGroup("单位标签")]
        [LabelText("选中")]
        public ENUM_MultipleTargetFlag Flags;

        [BoxGroup("单位标签")]
        [LabelText("剔除")]
        public ENUM_MultipleTargetFlag ExcludeFlags;

        [LabelText("目标上限")]
        public int MaxTargets;

        [LabelText("随机选择")]
        public bool Random;

        protected override void ChildClone(GamePlayActionTarget newActionTarget)
        {
            base.ChildClone(newActionTarget);
            MultipleActionTarget actionTarget = ((MultipleActionTarget) newActionTarget);
            actionTarget.Center = Center;
            actionTarget.Radius = Radius;
            actionTarget.Team = Team;
            actionTarget.Types = Types;
            actionTarget.ExcludeTypes = ExcludeTypes;
            actionTarget.Flags = Flags;
            actionTarget.ExcludeFlags = ExcludeFlags;
            actionTarget.MaxTargets = MaxTargets;
            actionTarget.Random = Random;
        }
    }
}