public class ShooterInfo
{
    public MechaType MechaType;
    public float FireInterval;
    public float MaxRange;
    public ProjectileInfo ProjectileInfo;

    public ShooterInfo(MechaType mechaType, float fireInterval, float maxRange, ProjectileInfo projectileInfo)
    {
        MechaType = mechaType;
        FireInterval = fireInterval;
        MaxRange = maxRange;
        ProjectileInfo = projectileInfo;
    }
}