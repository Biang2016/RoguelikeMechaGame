using System;
using GameCore;

namespace Client
{
    [Serializable]
    public class ShooterInfo
    {
        public ButtonState TriggerButtonState;
        public MechaType MechaType;
        public float FireInterval;
        public float MaxRange;
        public ProjectileInfo ProjectileInfo;

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