using System;
using Sirenix.OdinInspector;

namespace GameCore.AbilityDataDriven
{
    [LabelText("执行目标")]
    public abstract class ActionTarget
    {
    }

    public class SingleActionTarget : ActionTarget
    {
        [LabelText("目标")]
        public ENUM_SingleTarget Target;
    }

    public class MultipleActionTarget : ActionTarget
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
    }
}