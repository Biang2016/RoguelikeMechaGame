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

        public TransformHelper TransformHelper = new TransformHelper();

        internal ProjectileInfo ProjectileInfo;
        private Fix64 curSpeed;
        private Fix64 accelerate;
        private Fix64 range;

        public override void PoolRecycle()
        {
            Stop();
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            Collider.enabled = false;
            curSpeed = Fix64.Zero;
            accelerate = Fix64.Zero;
            range = Fix64.Zero;
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
            curSpeed = (Fix64) ProjectileInfo.ParentAction.Speed;
            accelerate = (Fix64) ProjectileInfo.ParentAction.Acceleration;
            range = (Fix64) ProjectileInfo.ParentAction.Range;
            if (GameObjectPoolManager.Instance.ProjectileFlashDict.TryGetValue(ProjectileInfo.ProjectileType, out GameObjectPool flashPool))
            {
                ProjectileFlash flash = flashPool.AllocateGameObject<ProjectileFlash>(ProjectileManager.Instance.Root);
                flash.transform.position = transform.position;
                flash.transform.rotation = Quaternion.identity;
                flash.transform.forward = gameObject.transform.forward;
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
            ContactPoint contact = collision.contacts[0];

            if (GameObjectPoolManager.Instance.ProjectileHitDict.ContainsKey(ProjectileInfo.ProjectileType))
            {
                ProjectileHit hit = GameObjectPoolManager.Instance.ProjectileHitDict[ProjectileInfo.ProjectileType].AllocateGameObject<ProjectileHit>(ProjectileManager.Instance.Root);
                hit.transform.position = contact.point + contact.normal * hitOffset;
                hit.transform.rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
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
                    hit.transform.LookAt(contact.point + contact.normal);
                }

                hit.Play();
            }

            PoolRecycle();
        }

        public void Update()
        {
            if (!IsRecycled)
            {
                Rigidbody.velocity = transform.forward * (float) curSpeed;
                curSpeed += accelerate * Time.deltaTime;
            }
        }
    }
}