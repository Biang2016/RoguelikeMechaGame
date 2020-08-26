using BiangStudio.CloneVariant;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore.AbilityDataDriven
{
    public class ProjectileConfig : IClone<ProjectileConfig>
    {
        [ReadOnly]
        [LabelText("投掷物名称")]
        public string ProjectileName;

        [LabelText("投掷物表现体")]
        public ProjectileType ProjectileType;

        [LabelText("投掷物发射点")]
        public ENUM_ProjectileDummyPosition DummyPos;

        [LabelText("射程(0为无限)")]
        [SuffixLabel("unit", true)]
        public int MaxRange;

        [LabelText("生存最大时间(0为无限)")]
        [SuffixLabel("ms", true)]
        public int MaxDuration;

        [LabelText("初始尺寸(/1000)")]
        public int Scale;

        [LabelText("尺寸增幅(/1000/s)")]
        public int ScaleIncrease;

        [LabelText("轴向速度曲线")]
        public AnimationCurve VelocityCurve;

        [LabelText("切向初速度(unit/s)")]
        public Vector2 Velocity;

        [LabelText("切向加速度(unit/s^2)")]
        public Vector2 Acceleration;

        [LabelText("重力加速度(unit/s^2)")]
        public int Gravity;

        [LabelText("物理检测")]
        public ENUM_MultipleTargetTeam CollisionFilter;

        [LabelText("穿透而非碰撞")]
        public ENUM_MultipleTargetTeam PenetrateFilter;

        [LabelText("反弹能力")]
        public bool CanReflect;

        [LabelText("最大反弹次数")]
        public int ReflectTimes = 1;

        public ProjectileConfig Clone()
        {
            return new ProjectileConfig
            {
                ProjectileName = ProjectileName,
                ProjectileType = ProjectileType,
                DummyPos = DummyPos,
                MaxRange = MaxRange,
                MaxDuration = MaxDuration,
                Scale = Scale,
                ScaleIncrease = ScaleIncrease,
                VelocityCurve = VelocityCurve,
                Velocity = Velocity,
                Acceleration = Acceleration,
                Gravity = Gravity,
                CollisionFilter = CollisionFilter,
                PenetrateFilter = PenetrateFilter,
                CanReflect = CanReflect,
                ReflectTimes = ReflectTimes,
            };
        }
    }
}