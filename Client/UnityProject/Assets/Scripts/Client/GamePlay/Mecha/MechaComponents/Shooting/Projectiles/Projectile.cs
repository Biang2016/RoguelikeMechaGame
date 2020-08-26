using BiangStudio.ObjectPool;
using GameCore;
using GameCore.AbilityDataDriven;
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
        private ProjectileColliderRoot ProjectileColliderRoot;

        [SerializeField]
        private Vector3 rotationOffset = new Vector3(0, 0, 0);

        private Rigidbody Rigidbody;
        private Collider Collider;
        private ParticleSystem ParticleSystem;

        internal ProjectileInfo ProjectileInfo;

        public override void PoolRecycle()
        {
            StopSelfEffect();
            ProjectileColliderRoot.OnRecycled();
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            Collider.enabled = false;
            ProjectileInfo = null;
            FlyRealtimeData.HitMechaComponentInfo = null;
            FlyRealtimeData.HitCollider = null;
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;
            base.PoolRecycle();
        }

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Collider = GetComponent<Collider>();
            ProjectileColliderRoot = GetComponentInChildren<ProjectileColliderRoot>();
            ParticleSystem = GetComponentInChildren<ParticleSystem>();
        }

        void Start()
        {
        }

        public void Initialize(ProjectileInfo projectileInfo)
        {
            ProjectileInfo = projectileInfo;
            ProjectileColliderRoot.Init(projectileInfo);
            CalculateOverrideParams();
        }

        #region OverrideParams

        private int Override_MaxRange;
        private int Override_MaxDuration;
        private int Override_Scale;
        private float Override_VelocityFactor;
        private bool Override_CanReflect;
        private int Override_ReflectTimes;

        private void CalculateOverrideParams()
        {
            Override_MaxRange = ProjectileInfo.ProjectileConfig.MaxRange;
            Override_MaxDuration = ProjectileInfo.ProjectileConfig.MaxDuration;
            Override_Scale = ProjectileInfo.ProjectileConfig.Scale;
            Override_VelocityFactor = 1f;
            Override_CanReflect = ProjectileInfo.ProjectileConfig.CanReflect;
            Override_ReflectTimes = ProjectileInfo.ProjectileConfig.ReflectTimes;
            switch (ProjectileInfo.ParentExecuteInfo.MechaComponentInfo.CurrentPowerUpgradeData)
            {
                case PowerUpgradeData_Gun pud_Gun:
                {
                    Override_MaxRange += Mathf.FloorToInt(ProjectileInfo.ProjectileConfig.MaxRange * pud_Gun.MaxRangeIncreasePercent / 100f);
                    Override_MaxDuration += Mathf.FloorToInt(ProjectileInfo.ProjectileConfig.MaxDuration * pud_Gun.MaxDurationIncreasePercent / 100f);
                    Override_Scale += Mathf.FloorToInt(ProjectileInfo.ProjectileConfig.Scale * pud_Gun.ScaleIncreasePercent / 100f);
                    Override_VelocityFactor += pud_Gun.VelocityIncreasePercent / 100f;
                    Override_CanReflect = pud_Gun.CanReflectOverride;
                    Override_ReflectTimes = pud_Gun.ReflectTimesOverride;
                    break;
                }
            }
        }

        #endregion

        public void Launch(Transform dummyPos)
        {
            Rigidbody.constraints = RigidbodyConstraints.None;
            Collider.enabled = true;
            transform.localScale = Vector3.one * (Override_Scale / 1000f);

            Vector3 initVelocity = new Vector3(ProjectileInfo.ProjectileConfig.Velocity.x, ProjectileInfo.ProjectileConfig.Velocity.y, ProjectileInfo.ProjectileConfig.VelocityCurve.Evaluate(0) * Override_VelocityFactor);
            FlyRealtimeData = new ProjectileInfo.FlyRealtimeData
            {
                FlyDistance = 0,
                FlyDuration = 0,
                Scale = transform.localScale,
                Position = transform.position,
                Velocity_Local = initVelocity,
                Velocity_Global = transform.TransformVector(initVelocity),
                Accelerate = ProjectileInfo.ProjectileConfig.Acceleration,
                HitCollider = null,
                RemainCollideTimes = Override_CanReflect ? Override_ReflectTimes : 9999,
            };

            PlayFlashEffect(dummyPos);
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

                // 尺寸
                FlyRealtimeData.Scale += Vector3.one * (ProjectileInfo.ProjectileConfig.ScaleIncrease / 1000f) * Time.deltaTime;
                transform.localScale = FlyRealtimeData.Scale;

                // 加速度 Y轴世界坐标，XZ轴局部坐标
                transform.forward = FlyRealtimeData.Velocity_Global.normalized;
                FlyRealtimeData.Velocity_Local += (Vector3) FlyRealtimeData.Accelerate * Time.fixedDeltaTime;
                FlyRealtimeData.Velocity_Local.z = ProjectileInfo.ProjectileConfig.VelocityCurve.Evaluate(FlyRealtimeData.FlyDuration) * Override_VelocityFactor;
                FlyRealtimeData.Velocity_Global = transform.TransformVector(FlyRealtimeData.Velocity_Local);
                FlyRealtimeData.Velocity_Global += new Vector3(0, -ProjectileInfo.ProjectileConfig.Gravity * Time.fixedDeltaTime, 0);

                transform.forward = FlyRealtimeData.Velocity_Global.normalized;
                FlyRealtimeData.Velocity_Local = transform.InverseTransformVector(FlyRealtimeData.Velocity_Global);

                // 速度
                Rigidbody.velocity = FlyRealtimeData.Velocity_Global;

                // 消亡检查
                bool recycleByDistance = Override_MaxRange > 0 && FlyRealtimeData.FlyDistance > Override_MaxRange;
                bool recycleByDuration = Override_MaxDuration > 0 && FlyRealtimeData.FlyDuration * 1000 > Override_MaxDuration;

                if (recycleByDistance || recycleByDuration)
                {
                    ClientGameManager.Instance.BattleMessenger.Broadcast((uint) ENUM_AbilityEvent.OnProjectileHitAndFinish, ProjectileInfo.ParentExecuteInfo, FlyRealtimeData);
                    ClientGameManager.Instance.BattleMessenger.Broadcast((uint) ENUM_AbilityEvent.OnProjectileFinish, ProjectileInfo.ParentExecuteInfo, FlyRealtimeData);
                    PoolRecycle();
                }
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!IsRecycled)
            {
                ContactPoint contact = collision.contacts[0];
                FlyRealtimeData.HitCollider = collision.collider;
                MechaComponent mc = collision.collider.GetComponentInParent<MechaComponent>();
                if (mc)
                {
                    FlyRealtimeData.HitMechaComponentInfo = mc.MechaComponentInfo;
                }

                bool recycle = false;
                if (Override_CanReflect)
                {
                    Vector3 reflectDir = FlyRealtimeData.Velocity_Global.normalized - 2 * Vector3.Dot(FlyRealtimeData.Velocity_Global.normalized, contact.normal) * contact.normal;
                    FlyRealtimeData.Velocity_Global = reflectDir * FlyRealtimeData.Velocity_Global.magnitude;
                    transform.forward = FlyRealtimeData.Velocity_Global.normalized;
                    Rigidbody.velocity = FlyRealtimeData.Velocity_Global;
                    FlyRealtimeData.Velocity_Local = transform.InverseTransformVector(FlyRealtimeData.Velocity_Global);

                    if (FlyRealtimeData.RemainCollideTimes <= 0)
                    {
                        recycle = true;
                    }
                    else
                    {
                        FlyRealtimeData.RemainCollideTimes--;
                    }
                }
                else
                {
                    recycle = true;
                }

                PlayHitEffect(contact.point, contact.normal);
                ClientGameManager.Instance.BattleMessenger.Broadcast((uint) ENUM_AbilityEvent.OnProjectileHitUnit, ProjectileInfo.ParentExecuteInfo, FlyRealtimeData);
                ClientGameManager.Instance.BattleMessenger.Broadcast((uint) ENUM_AbilityEvent.OnProjectileHitAndFinish, ProjectileInfo.ParentExecuteInfo, FlyRealtimeData);

                if (recycle) PoolRecycle();
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

        public void PlayFlashEffect(Transform dummyPos)
        {
            if (GameObjectPoolManager.Instance.ProjectileFlashDict.TryGetValue(ProjectileInfo.ProjectileType, out GameObjectPool flashPool))
            {
                ProjectileFlash flash = flashPool.AllocateGameObject<ProjectileFlash>(dummyPos);
                flash.transform.position = dummyPos.position;
                flash.transform.localScale = FlyRealtimeData.Scale;
                flash.transform.rotation = Quaternion.identity;
                flash.transform.forward = dummyPos.forward;
                flash.Play();
            }
        }

        public void PlayHitEffect(Vector3 position, Vector3 direction)
        {
            if (GameObjectPoolManager.Instance.ProjectileHitDict.ContainsKey(ProjectileInfo.ProjectileType))
            {
                ProjectileHit hit = GameObjectPoolManager.Instance.ProjectileHitDict[ProjectileInfo.ProjectileType].AllocateGameObject<ProjectileHit>(ClientProjectileManager.Instance.Root);
                hit.transform.position = position + direction * hitOffset;
                hit.transform.localScale = FlyRealtimeData.Scale;
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