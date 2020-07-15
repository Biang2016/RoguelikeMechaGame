using BiangStudio.GameDataFormat;
using BiangStudio.ObjectPool;
using GameCore;
using UnityEngine;

namespace Client
{
    public class Projectile : PoolObject
    {
        [SerializeField]
        private float hitOffset = 0f;

        [SerializeField]
        private bool UseFirePointRotation;

        [SerializeField]
        private Vector3 rotationOffset = new Vector3(0, 0, 0);

        private Rigidbody Rigidbody;
        private Collider Collider;
        private ParticleSystem ParticleSystem;

        internal ProjectileInfo ProjectileInfo;

        public override void PoolRecycle()
        {
            StopSelfEffect();
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            Collider.enabled = false;
            ProjectileInfo = null;
            base.PoolRecycle();
        }

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Collider = GetComponent<Collider>();
            ParticleSystem = GetComponentInChildren<ParticleSystem>();
        }

        void Start()
        {
        }

        public void Initialize(ProjectileInfo projectileInfo)
        {
            ProjectileInfo = projectileInfo;
        }

        public void Launch()
        {
            Rigidbody.constraints = RigidbodyConstraints.None;
            Collider.enabled = true;
            FlyRealtimeData = new ProjectileInfo.FlyRealtimeData
            {
                FlyDistance = 0,
                FlyDuration = 0,
                Position = transform.position,
                Velocity_Local = ProjectileInfo.ProjectileConfig.Velocity,
                Velocity_Global = transform.TransformVector(ProjectileInfo.ProjectileConfig.Velocity),
                Accelerate = ProjectileInfo.ProjectileConfig.Acceleration,
                HitCollider = null,
                RemainCollideTimes = ProjectileInfo.ProjectileConfig.CollideTimes,
            };

            PlayFlashEffect(transform.position, transform.forward);
            PlaySelfEffect();
            PoolRecycle(ParticleSystem.main.duration);
        }

        private ProjectileInfo.FlyRealtimeData FlyRealtimeData;

        void FixedUpdate()
        {
            if (!IsRecycled)
            {
                // 统计
                FlyRealtimeData.FlyDistance += FlyRealtimeData.Velocity_Global.magnitude * Time.fixedDeltaTime;
                FlyRealtimeData.FlyDuration += Time.fixedDeltaTime;
                FlyRealtimeData.Position = transform.position;

                // 朝向

                // 加速度 Y轴世界坐标，XZ轴局部坐标
                transform.forward = FlyRealtimeData.Velocity_Global.normalized;
                FlyRealtimeData.Velocity_Local += Vector3.Scale(new Vector3(1, 0, 1), FlyRealtimeData.Accelerate) * Time.fixedDeltaTime;
                FlyRealtimeData.Velocity_Global = transform.TransformVector(FlyRealtimeData.Velocity_Local);
                FlyRealtimeData.Velocity_Global += new Vector3(0, FlyRealtimeData.Accelerate.y * Time.fixedDeltaTime, 0);

                transform.forward = FlyRealtimeData.Velocity_Global.normalized;
                FlyRealtimeData.Velocity_Local = transform.InverseTransformVector(FlyRealtimeData.Velocity_Global);

                // 速度
                Rigidbody.velocity = FlyRealtimeData.Velocity_Global;

                // 消亡检查
                if (ProjectileInfo.ProjectileConfig.MaxRange > 0 && FlyRealtimeData.FlyDistance > ProjectileInfo.ProjectileConfig.MaxRange)
                {
                    ProjectileInfo.ParentAction.OnMiss?.Invoke(FlyRealtimeData);
                    PoolRecycle();
                    return;
                }

                if (ProjectileInfo.ProjectileConfig.MaxDuration > 0 && FlyRealtimeData.FlyDuration * 1000 > ProjectileInfo.ProjectileConfig.MaxDuration)
                {
                    ProjectileInfo.ParentAction.OnMiss?.Invoke(FlyRealtimeData);
                    PoolRecycle();
                    return;
                }
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!IsRecycled)
            {
                ContactPoint contact = collision.contacts[0];
                FlyRealtimeData.HitCollider = collision.collider;
                MechaComponentBase mcb = collision.collider.GetComponentInParent<MechaComponentBase>();
                if (mcb && !ProjectileInfo.ProjectileConfig.IsCollideWithOwner && mcb.MechaInfo == ProjectileInfo.ParentMechaInfo)
                {
                    return;
                }
                else
                {
                    FlyRealtimeData.RemainCollideTimes--;
                    PlayHitEffect(contact.point, contact.normal);
                    Vector3 reflectDir = FlyRealtimeData.Velocity_Global.normalized - 2 * Vector3.Dot(FlyRealtimeData.Velocity_Global.normalized, contact.normal) * contact.normal;
                    FlyRealtimeData.Velocity_Global = reflectDir * FlyRealtimeData.Velocity_Global.magnitude;
                    transform.forward = FlyRealtimeData.Velocity_Global.normalized;
                    Rigidbody.velocity = FlyRealtimeData.Velocity_Global;
                    FlyRealtimeData.Velocity_Local = transform.InverseTransformVector(FlyRealtimeData.Velocity_Global);
                    if (FlyRealtimeData.RemainCollideTimes <= 0)
                    {
                        ProjectileInfo.ParentAction.OnHit?.Invoke(FlyRealtimeData);
                        PoolRecycle();
                    }
                }
            }
        }

        private void PlaySelfEffect()
        {
            ParticleSystem.Play(true);
        }

        private void StopSelfEffect()
        {
            ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        public void PlayFlashEffect(Vector3 position, Vector3 direction)
        {
            if (GameObjectPoolManager.Instance.ProjectileFlashDict.TryGetValue(ProjectileInfo.ProjectileType, out GameObjectPool flashPool))
            {
                ProjectileFlash flash = flashPool.AllocateGameObject<ProjectileFlash>(ProjectileManager.Instance.Root);
                flash.transform.position = position;
                flash.transform.rotation = Quaternion.identity;
                flash.transform.forward = direction;
                flash.Play();
            }
        }

        public void PlayHitEffect(Vector3 position, Vector3 direction)
        {
            if (GameObjectPoolManager.Instance.ProjectileHitDict.ContainsKey(ProjectileInfo.ProjectileType))
            {
                ProjectileHit hit = GameObjectPoolManager.Instance.ProjectileHitDict[ProjectileInfo.ProjectileType].AllocateGameObject<ProjectileHit>(ProjectileManager.Instance.Root);
                hit.transform.position = position + direction * hitOffset;
                hit.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
                if (UseFirePointRotation)
                {
                    hit.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0);
                }
                else if (rotationOffset != Vector3.zero)
                {
                    hit.transform.rotation = Quaternion.Euler(rotationOffset);
                }
                else
                {
                    hit.transform.LookAt(position + direction);
                }

                hit.Play();
            }
        }
    }
}