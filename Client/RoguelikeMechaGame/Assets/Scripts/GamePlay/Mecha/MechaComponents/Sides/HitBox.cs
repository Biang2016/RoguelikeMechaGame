using UnityEngine;

public class HitBox : MonoBehaviour
{
    internal HitBoxRoot ParentHitBoxRoot;

    private void Awake()
    {
        ParentHitBoxRoot = GetComponentInParent<HitBoxRoot>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Projectile p = collision.gameObject.GetComponent<Projectile>();
        if (p && p.ProjectileInfo.MechaType != ParentHitBoxRoot.MechaComponentBase.MechaType)
        {
            ParentHitBoxRoot.MechaComponentBase.Damage(p.ProjectileInfo.FinalDamage);
        }
    }
}