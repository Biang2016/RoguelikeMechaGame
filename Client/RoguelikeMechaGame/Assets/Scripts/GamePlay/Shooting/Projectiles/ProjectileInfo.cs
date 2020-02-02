using UnityEngine;
using System.Collections;

public class ProjectileInfo
{
    public MechaType MechaType;
    public ProjectileType ProjectileType;

    public ProjectileInfo(MechaType mechaType, ProjectileType projectileType)
    {
        MechaType = mechaType;
        ProjectileType = projectileType;
    }
}