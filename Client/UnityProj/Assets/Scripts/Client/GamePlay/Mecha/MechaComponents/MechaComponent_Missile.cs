using UnityEngine;
using System.Collections;
using GameCore;

namespace Client
{
    public class MechaComponent_Missile : MechaComponent_Controllable_Base
    {
        public Shooter Shooter;

        void Start()
        {
            Shooter.Initialize(new ShooterInfo(MechaType.Self, 0.1f, 50f, new ProjectileInfo(MechaType.Self, ProjectileType.Projectile_Butter, GameCore.ConfigManager.MissileSpeed, GameCore.ConfigManager.MissileDamage)));
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void ControlPerFrame()
        {
            if (Input.GetButton("Fire2"))
            {
                Shooter?.ContinuousShoot();
            }
            else
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    Shooter?.Shoot();
                }
            }
        }
    }
}