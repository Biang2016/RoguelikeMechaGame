using UnityEngine;

namespace Client
{
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

            if (Input.GetKeyUp(KeyCode.C))
            {
                ShooterInfo.ProjectileInfo.ProjectileType = (ProjectileType) (((int) ShooterInfo.ProjectileInfo.ProjectileType + 1) % 25 + 1);
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
            ProjectileManager.Instance.ShootProjectile(ShooterInfo.ProjectileInfo, FirePoint.position, FirePoint.forward);
        }
    }
}