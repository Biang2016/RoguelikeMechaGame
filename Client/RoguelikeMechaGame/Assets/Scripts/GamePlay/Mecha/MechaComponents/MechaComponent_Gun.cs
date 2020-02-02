using UnityEngine;
using System.Collections;

public class MechaComponent_Gun : MechaComponentBase
{
    public Shooter Shooter;

    void Start()
    {
        Shooter.Initialize(new ShooterInfo(MechaType.Self, 0.3f, 50f, new BulletInfo(MechaType.Self, ProjectileType.ArrowsFly)));
    }

    void Update()
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