using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private Transform FirePoint;

    public ShooterInfo ShooterInfo;

    public void Initialize(ShooterInfo shooterInfo)
    {
        ShooterInfo = shooterInfo;
    }

    private float fireCountdown = 0f;

    void Update()
    {
        if (GameManager.Instance.GetState() == GameState.Fighting)
        {
            fireCountdown -= Time.deltaTime;
        }
    }

    public void Shoot()
    {
        FireByFirePointDirection();
    }

    public void ContinuousShoot()
    {
        if (fireCountdown <= 0f)
        {
            FireByFirePointDirection();
            fireCountdown = 0;
            fireCountdown += ShooterInfo.FireInterval;
        }
    }

    private void FireByFirePointDirection()
    {
        ProjectileManager.Instance.ShootProjectile(ProjectileType.EvilBigGravBall, FirePoint.position, FirePoint.forward);
    }
}