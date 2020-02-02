using UnityEngine;
using System.Collections;

public class MechaComponent_Missile : MechaComponentBase
{
    public Shooter Shooter;

    void Start()
    {
        Shooter.Initialize(new ShooterInfo(MechaType.Self, 0.1f, 50f, new BulletInfo(MechaType.Self, ProjectileType.BubbleBlade)));
    }

    void Update()
    {
        if (GameManager.Instance.GetState() == GameState.Fighting)
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
}