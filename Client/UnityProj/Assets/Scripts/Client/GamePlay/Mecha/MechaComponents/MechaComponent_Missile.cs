using GameCore;
using UnityEngine;

namespace Client
{
    public class MechaComponent_Missile : MechaComponent_Controllable_Base
    {
        public Shooter Shooter;

        void Start()
        {
            Shooter.Initialize(new ShooterInfo(MechaType.Player, 0.1f, 50f, new ProjectileInfo(MechaType.Player, ProjectileType.Projectile_Butter, GameCore.ConfigManager.MissileSpeed, GameCore.ConfigManager.MissileDamage)));
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