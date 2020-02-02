public class ShooterInfo
{
    public MechaType MechaType;
    public float FireInterval;
    public float MaxRange;
    public BulletInfo BulletInfo;

    public ShooterInfo(MechaType mechaType, float fireInterval, float maxRange, BulletInfo bulletInfo)
    {
        MechaType = mechaType;
        FireInterval = fireInterval;
        MaxRange = maxRange;
        BulletInfo = bulletInfo;
    }
}