using UnityEngine;

namespace Client
{
    public class MechaComponent_Gun : MechaComponent_Controllable_Base
    {
        public Shooter Shooter;

        protected override void Child_Initialize()
        {
            base.Child_Initialize();
            if (Mecha) Shooter.Initialize(new ShooterInfo(Mecha.MechaInfo.MechaType, 0.1f, 50f, new ProjectileInfo(Mecha.MechaInfo.MechaType, ProjectileType.Projectile_ArrowsFly, GameCore.ConfigManager.GunSpeed, GameCore.ConfigManager.GunDamage)));
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

        public void AddModifier(Modifier modifier)
        {
            Shooter.ShooterInfo.ProjectileInfo.Modifiers_Damage.Add(modifier);
        }

        public void RemoveModifier(Modifier modifier)
        {
            Shooter.ShooterInfo.ProjectileInfo.Modifiers_Damage.Remove(modifier);
        }
    }
}