using UnityEngine;
using System.Collections;

public class ProjectileInfo
{
    public MechaType MechaType;
    public ProjectileType ProjectileType;
    public float Speed;

    public ProjectileInfo(MechaType mechaType, ProjectileType projectileType,float speed)
    {
        MechaType = mechaType;
        ProjectileType = projectileType;
        Speed = speed;
    }
}