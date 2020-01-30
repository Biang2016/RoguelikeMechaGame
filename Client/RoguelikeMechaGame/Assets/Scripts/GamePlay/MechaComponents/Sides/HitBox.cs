using System;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    private HitBoxRoot ParentHitBoxRoot;

    private void Awake()
    {
        ParentHitBoxRoot = transform.parent.GetComponent<HitBoxRoot>();
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