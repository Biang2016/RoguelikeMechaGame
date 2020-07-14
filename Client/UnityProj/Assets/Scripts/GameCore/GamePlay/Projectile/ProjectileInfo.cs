﻿using System;
using BiangStudio.GameDataFormat;
using GameCore.AbilityDataDriven;
using UnityEngine;

namespace GameCore
{
    [Serializable]
    public class ProjectileInfo
    {
        public Action_EmitProjectile_DelayLine ParentAction;
        public MechaComponentInfo ParentMechaComponentInfo;
        public MechaInfo ParentMechaInfo;

        public Transform ChasingTarget;
        public Vector3 ChasingPosition;

        public MechaType MechaType => ParentMechaInfo.MechaType;
        public ProjectileConfig ProjectileConfig => ParentAction.ProjectileConfig;
        public ProjectileType ProjectileType => ProjectileConfig.ProjectileType;

        public ProjectileInfo(Action_EmitProjectile_DelayLine parentAction, MechaComponentInfo parentMechaComponentInfo, MechaInfo parentMechaInfo,
            Transform chasingTarget, Vector3 chasingPosition)
        {
            ParentAction = parentAction;
            ParentMechaComponentInfo = parentMechaComponentInfo;
            ParentMechaInfo = parentMechaInfo;
            ChasingTarget = chasingTarget;
            ChasingPosition = chasingPosition;
        }

        public struct FlyRealtimeData
        {
            public float FlyDistance;
            public float FlyDuration;
            public int Range;
            public Vector3 Velocity;
            public Vector3 Accelerate;
            public Vector3 CurrentPosition;
            public Collider HitCollider;
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