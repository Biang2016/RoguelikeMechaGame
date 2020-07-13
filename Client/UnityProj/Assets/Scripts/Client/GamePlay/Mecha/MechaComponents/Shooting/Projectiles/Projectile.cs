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
        private Vector3 curSpeed;
        private Vector3 accelerate;
        private float range;

        public override void PoolRecycle()
        {
            Stop();
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            Collider.enabled = false;
            curSpeed = Vector3.zero;
            accelerate = Vector3.zero;
            range = 0;
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
            FlyRealtimeData = new ProjectileInfo.FlyRealtimeData();
            Rigidbody.constraints = RigidbodyConstraints.None;
            Collider.enabled = true;
            curSpeed = ProjectileInfo.ParentAction.Velocity;
            accelerate = ProjectileInfo.ParentAction.Acceleration;
            range = ProjectileInfo.ParentAction.Range;
            if (GameObjectPoolManager.Instance.ProjectileFlashDict.TryGetValue(ProjectileInfo.ProjectileType, out GameObjectPool flashPool))
            {
                ProjectileFlash flash = flashPool.AllocateGameObject<ProjectileFlash>(ProjectileManager.Instance.Root);
                flash.transform.position = transform.position;
                flash.transform.rotation = Quaternion.identity;
                flash.transform.forward = transform.forward;
                flash.Play();
            }

            Play();
            PoolRecycle(ParticleSystem.main.duration);
        }

        private void Play()
        {
            ParticleSystem.Play(true);
        }

        private void Stop()
        {
            ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (ProjectileInfo != null)
            {
                ContactPoint contact = collision.contacts[0];
                ProjectileInfo.ParentAction.OnHit?.Invoke(FlyRealtimeData);
                PlayHitEffect(contact.point, contact.normal);
                PoolRecycle();
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

        private ProjectileInfo.FlyRealtimeData FlyRealtimeData = new ProjectileInfo.FlyRealtimeData();

        public void Update()
        {
            if (!IsRecycled)
            {
                Rigidbody.velocity = transform.TransformVector(curSpeed);
                curSpeed += accelerate * Time.deltaTime;
                FlyRealtimeData.Velocity = Rigidbody.velocity;
                FlyRealtimeData.Accelerate = accelerate;
            }
        }
    }
}