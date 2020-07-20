using BiangStudio.CloneVariant;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore.AbilityDataDriven
{
    public class ProjectileConfig : IClone<ProjectileConfig>
    {
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

        [LabelText("初速度(unit/s)")]
        public Vector3 Velocity;

        [LabelText("加速度(unit/s^2)")]
        public Vector3 Acceleration;

        [LabelText("最大碰撞次数")]
        public int CollideTimes = 1;

        [LabelText("和施法者碰撞")]
        public bool IsCollideWithOwner = false;

        public ProjectileConfig Clone()
        {
            return new ProjectileConfig
            {
                ProjectileName = ProjectileName,
                ProjectileType = ProjectileType,
                DummyPos = DummyPos,
                MaxRange = MaxRange,
                MaxDuration = MaxDuration,
                Velocity = Velocity,
                Acceleration = Acceleration,
                IsCollideWithOwner = IsCollideWithOwner,
                CollideTimes = CollideTimes,
            };
        }
    }
}