using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class MechaComponentHitBox : MonoBehaviour
    {
        internal MechaComponentHitBoxRoot ParentHitBoxRoot;

        private void Awake()
        {
            ParentHitBoxRoot = GetComponentInParent<MechaComponentHitBoxRoot>();
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