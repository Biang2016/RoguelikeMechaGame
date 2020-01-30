using System;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    internal HitBoxRoot ParentHitBoxRoot;

    private void Awake()
    {
        ParentHitBoxRoot =GetComponentInParent<HitBoxRoot>();
    }

    private void OnCollisionEnter(Collision other)
    {
        Projectile p = other.gameObject.GetComponentInParent<Projectile>();
        if (p)
        {
            ParentHitBoxRoot.OnHit(p.Damage);
        }
    }
}