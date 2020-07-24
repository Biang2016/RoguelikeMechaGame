using System;
using BiangStudio.GameDataFormat;
using GameCore.AbilityDataDriven;
using UnityEngine;

namespace GameCore
{
    [Serializable]
    public class ProjectileInfo
    {
        public Action_EmitProjectile ParentAction;
        public ExecuteInfo ParentExecuteInfo;

        public Transform ChasingTarget;
        public Vector3 ChasingPosition;

        public MechaType MechaType => ParentExecuteInfo.MechaInfo.MechaType;
        public ProjectileConfig ProjectileConfig => ParentAction.ProjectileConfig;
        public ProjectileType ProjectileType => ProjectileConfig.ProjectileType;

        public ProjectileInfo(Action_EmitProjectile parentAction, ExecuteInfo executeInfo, Transform chasingTarget, Vector3 chasingPosition)
        {
            ParentAction = parentAction;
            ParentExecuteInfo = executeInfo;
            ChasingTarget = chasingTarget;
            ChasingPosition = chasingPosition;
        }

        public struct FlyRealtimeData
        {
            public float FlyDistance;
            public float FlyDuration;
            public Vector3 Position;
            public Vector3 Velocity_Local;
            public Vector3 Velocity_Global;
            public Vector3 Accelerate;
            public int RemainCollideTimes;

            // Need to release
            public Collider HitCollider;
            public MechaComponentInfo HitMechaComponentInfo;
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