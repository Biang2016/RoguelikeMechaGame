using System;
using GameCore;
using Sirenix.OdinInspector;

namespace Client
{
    [Serializable]
    public class ShooterInfo
    {
        [LabelText("触发按键")] public ButtonState TriggerButtonState;
        internal MechaType MechaType;
        [LabelText("开火间隔")] public float FireInterval;
        [LabelText("飞行距离")] public float MaxRange;
        [LabelText("投掷物信息")] public ProjectileInfo ProjectileInfo;

        public ShooterInfo(ButtonState triggerButtonState, MechaType mechaType, float fireInterval, float maxRange, ProjectileInfo projectileInfo)
        {
            TriggerButtonState = triggerButtonState;
            MechaType = mechaType;
            FireInterval = fireInterval;
            MaxRange = maxRange;
            ProjectileInfo = projectileInfo;
        }
    }
}