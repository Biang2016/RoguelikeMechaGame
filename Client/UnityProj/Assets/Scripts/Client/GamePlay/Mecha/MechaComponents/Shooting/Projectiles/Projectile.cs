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
                Velocity = ProjectileInfo.ParentAction.Velocity,
                Accelerate = ProjectileInfo.ParentAction.Acceleration,
                Range = ProjectileInfo.ParentAction.MaxRange,
                CurrentPosition = transform.position,
                HitCollider = null,
            };

            PlayFlashEffect(transform.position, transform.forward);
            PlaySelfEffect();
            PoolRecycle(ParticleSystem.main.duration);
        }

        private ProjectileInfo.FlyRealtimeData FlyRealtimeData = new ProjectileInfo.FlyRealtimeData();

        void FixedUpdate()
        {
            if (!IsRecycled)
            {
                Rigidbody.velocity = transform.TransformVector(FlyRealtimeData.Velocity);
                FlyRealtimeData.FlyDistance += FlyRealtimeData.Velocity.magnitude * Time.fixedDeltaTime;
                FlyRealtimeData.FlyDuration += Time.fixedDeltaTime;
                FlyRealtimeData.CurrentPosition = transform.position;
                FlyRealtimeData.Velocity += FlyRealtimeData.Accelerate * Time.fixedDeltaTime;

                if (ProjectileInfo.ParentAction.MaxRange > 0 && FlyRealtimeData.FlyDistance > ProjectileInfo.ParentAction.MaxRange)
                {
                    ProjectileInfo.ParentAction.OnMiss?.Invoke(FlyRealtimeData);
                    PoolRecycle();
                    return;
                }

                if (ProjectileInfo.ParentAction.MaxDuration > 0 && FlyRealtimeData.FlyDuration * 1000 > ProjectileInfo.ParentAction.MaxDuration)
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
                ProjectileInfo.ParentAction.OnHit?.Invoke(FlyRealtimeData);
                PlayHitEffect(contact.point, contact.normal);
                PoolRecycle();
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