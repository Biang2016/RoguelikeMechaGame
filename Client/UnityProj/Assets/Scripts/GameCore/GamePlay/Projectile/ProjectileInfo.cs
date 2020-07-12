using System;
using BiangStudio.GameDataFormat;

namespace GameCore
{
    [Serializable]
    public class ProjectileInfo
    {
        public AbilityDataDriven.Action_EmitProjectile_DelayLine ParentAction;
        public MechaType MechaType;
        public ProjectileType ProjectileType;

        public ProjectileInfo(AbilityDataDriven.Action_EmitProjectile_DelayLine parentAction, MechaType mechaType, ProjectileType projectileType)
        {
            ParentAction = parentAction;
            MechaType = mechaType;
            ProjectileType = projectileType;
        }
    }

    public enum ProjectileType
    {
        Projectile_Leaves = 1,
        Projectile_BloodBlade = 2,
        Projectile_WhiteLightening = 3,
        Projectile_Fire = 4,
        Projectile_SnowFlake = 5,
        Projectile_PurpleSmoke = 6,
        Projectile_WhiteFlash = 7,
        Projectile_PurpleGravBoom = 8,
        Projectile_InterlacedRays = 9,
        Projectile_GreenPoisonous = 10,
        Projectile_BubbleBlade = 11,
        Projectile_CyanSlight = 12,
        Projectile_YellowLightening = 13,
        Projectile_WaterBall = 14,
        Projectile_FlyCutter = 15,
        Projectile_SpiralDrill = 16,
        Projectile_LoveHeart = 17,
        Projectile_BlueArrowSmoke = 18,
        Projectile_YellowLighteningHotBall = 19,
        Projectile_EvilBigGravBall = 20,
        Projectile_FastGreenBoom = 21,
        Projectile_TwinkleLittleWhite = 22,
        Projectile_Mushroom = 23,
        Projectile_Butter = 24,
        Projectile_ArrowsFly = 25,
    }
}