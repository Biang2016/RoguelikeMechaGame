using System.Collections.Generic;
using UnityEngine;

namespace Client
{
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
    }
}