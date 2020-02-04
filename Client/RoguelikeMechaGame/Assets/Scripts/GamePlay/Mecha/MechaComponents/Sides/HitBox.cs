using System;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    internal HitBoxRoot ParentHitBoxRoot;

    private void Awake()
    {
        ParentHitBoxRoot =GetComponentInParent<HitBoxRoot>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Projectile p = collision.gameObject.GetComponent<Projectile>();
        if (p)
        {
            ParentHitBoxRoot.MechaComponentBase.Damage(5);
        }
    }
}