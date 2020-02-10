using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    internal HitBoxRoot ParentHitBoxRoot;

    private void Awake()
    {
        ParentHitBoxRoot = GetComponentInParent<HitBoxRoot>();
    }

    private bool InBattle;

    public void SetInBattle(bool inBattle)
    {
        InBattle = inBattle;
        if (!inBattle)
        {
            StayingBlades.Clear();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (InBattle)
        {
            Projectile p = collision.gameObject.GetComponent<Projectile>();
            if (p && p.ProjectileInfo.MechaType != ParentHitBoxRoot.MechaComponentBase.MechaType)
            {
                ParentHitBoxRoot.MechaComponentBase.Damage(p.ProjectileInfo.FinalDamage);
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider c)
    {
        if (InBattle)
        {
            Blade b = c.gameObject.GetComponent<Blade>();
            if (b && b.BladeInfo.MechaType != ParentHitBoxRoot.MechaComponentBase.MechaType)
            {
                if (!StayingBlades.Contains(b))
                {
                    StayingBlades.Add(b);
                }

                return;
            }
        }
    }

    private List<Blade> StayingBlades = new List<Blade>();

    private float bladeHitTick = 0;
    private float bladeHitInterval = 0.5f;

    void Update()
    {
        if (InBattle)
        {
            bladeHitTick += Time.deltaTime;
            if (bladeHitTick > bladeHitInterval)
            {
                bladeHitTick = 0;
                foreach (Blade b in StayingBlades)
                {
                    ParentHitBoxRoot.MechaComponentBase.Damage(b.BladeInfo.FinalDamage);
                }
            }
        }
    }

    private void OnTriggerExit(Collider c)
    {
        if (InBattle)
        {
            Blade b = c.gameObject.GetComponent<Blade>();
            if (b && b.BladeInfo.MechaType != ParentHitBoxRoot.MechaComponentBase.MechaType)
            {
                StayingBlades.Remove(b);
                return;
            }
        }
    }
}