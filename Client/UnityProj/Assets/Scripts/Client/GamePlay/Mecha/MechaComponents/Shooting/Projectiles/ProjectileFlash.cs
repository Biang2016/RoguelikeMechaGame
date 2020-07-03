using BiangStudio.GamePlay;
using UnityEngine;

namespace Client
{
    public class ProjectileFlash : PoolObject
    {
        internal ParticleSystem ParticleSystem;

        void Awake()
        {
            ParticleSystem = GetComponent<ParticleSystem>();
            if (!ParticleSystem)
            {
                ParticleSystem = GetComponentInChildren<ParticleSystem>();
            }
        }

        public override void PoolRecycle()
        {
            base.PoolRecycle();
            ParticleSystem.Stop(true);
        }
    }
}