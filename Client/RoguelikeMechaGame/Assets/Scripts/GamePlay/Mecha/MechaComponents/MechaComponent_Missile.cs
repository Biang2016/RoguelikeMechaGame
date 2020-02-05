using UnityEngine;
using System.Collections;

public class MechaComponent_Missile : MechaComponent_Controllable_Base
{
    public Shooter Shooter;

    void Start()
    {
        Shooter.Initialize(new ShooterInfo(MechaType.Self, 0.1f, 50f, new ProjectileInfo(MechaType.Self, ProjectileType.Projectile_Butter)));
    }

    void Update()
    {
    }

    protected override void ControlPerFrame()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (Shooter)
            {
                Shooter.Shoot();
            }
        }

        if (Input.GetMouseButton(1))
        {
            Shooter.ContinuousShoot();
        }
    }
}