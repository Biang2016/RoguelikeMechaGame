﻿using System;
using BiangStudio.GameDataFormat;

namespace GameCore
{
    [Serializable]
    public class ProjectileInfo
    {
        public AbilityDataDriven.Action_EmitProjectile_DelayLine ParentAction;
        public MechaType MechaType;
        public ProjectileType ProjectileType;

        public TransformInfo TransformInfo;

        public ProjectileInfo(AbilityDataDriven.Action_EmitProjectile_DelayLine parentAction, MechaType mechaType, ProjectileType projectileType)
        {
            ParentAction = parentAction;
            MechaType = mechaType;
            ProjectileType = projectileType;
        }
    }

    public enum ProjectileType
    {
        Projectile_Leaves,
        Projectile_BloodBlade,
        Projectile_WhiteLightening,
        Projectile_Fire,
        Projectile_SnowFlake,
        Projectile_PurpleSmoke,
        Projectile_WhiteFlash,
        Projectile_PurpleGravBoom,
        Projectile_InterlacedRays,
        Projectile_GreenPoisonous,
        Projectile_BubbleBlade,
        Projectile_CyanSlight,
        Projectile_YellowLightening,
        Projectile_WaterBall,
        Projectile_FlyCutter,
        Projectile_SpiralDrill,
        Projectile_LoveHeart,
        Projectile_BlueArrowSmoke,
        Projectile_YellowLighteningHotBall,
        Projectile_EvilBigGravBall,
        Projectile_FastGreenBoom,
        Projectile_TwinkleLittleWhite,
        Projectile_Mushroom,
        Projectile_Butter,
        Projectile_ArrowsFly,
    }
}