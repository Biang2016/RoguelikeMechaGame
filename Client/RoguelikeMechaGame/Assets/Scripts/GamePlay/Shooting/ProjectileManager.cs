using UnityEngine;
using System.Collections;

public class ProjectileManager : MonoSingleton<ProjectileManager>
{
    public GameObject ShootProjectile(ProjectileType projectileType, Vector3 from, Vector3 dir)
    {
        GameObject prefab = PrefabManager.Instance.GetPrefab("Projectile_" + projectileType);
        GameObject go = Instantiate(prefab);
        go.transform.position = from;
        go.transform.LookAt(from + dir);
        return go;
    }
}

public enum ProjectileType
{
    Leaves = 1,
    BloodBlade = 2,
    WhiteLightening = 3,
    Fire = 4,
    SnowFlake = 5,
    PurpleSmoke = 6,
    WhiteFlash = 7,
    PurpleGravBoom = 8,
    InterlacedRays = 9,
    GreenPoisonous = 10,
    BubbleBlade = 11,
    CyanSlight = 12,
    YellowLightening = 13,
    WaterBall = 14,
    FlyCutter = 15,
    SpiralDrill = 16,
    LoveHeart = 17,
    BlueArrowSmoke = 18,
    YellowLighteningHotBall = 19,
    EvilBigGravBall = 20,
    FastGreenBoom = 21,
    TwinkleLittleWhite = 22,
    Mushroom = 23,
    Butter = 24,
    ArrowsFly = 25,
}