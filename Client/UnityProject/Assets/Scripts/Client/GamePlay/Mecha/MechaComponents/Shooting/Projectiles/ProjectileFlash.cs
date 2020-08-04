using BiangStudio.ObjectPool;
using UnityEngine;

namespace Client
{
    public class ProjectileFlash : PoolObject
    {
        private ParticleSystem ParticleSystem;

        public override void PoolRecycle()
        {
            Stop();
            base.PoolRecycle();
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;
        }

        void Awake()
        {
            ParticleSystem = GetComponentInChildren<ParticleSystem>();
        }

        void Update()
        {
            if (!IsRecycled && ParticleSystem.isStopped)
            {
                PoolRecycle();
            }
        }

        public void Play()
        {
            ParticleSystem.Play(true);
        }

        public void Stop()
        {
            ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}