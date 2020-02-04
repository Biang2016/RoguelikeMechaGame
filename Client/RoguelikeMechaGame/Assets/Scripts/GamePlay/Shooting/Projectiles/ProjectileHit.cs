using UnityEngine;

public class ProjectileHit : PoolObject
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