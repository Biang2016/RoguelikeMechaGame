using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

namespace Client
{
    public class MechaComponentHitBox : MonoBehaviour
    {
        internal MechaComponentHitBoxRoot ParentHitBoxRoot;
        internal Mecha Mecha => ParentHitBoxRoot.MechaComponentBase.Mecha;
        internal BoxCollider BoxCollider;

        private void Awake()
        {
            BoxCollider = GetComponentInParent<BoxCollider>();
            ParentHitBoxRoot = GetComponentInParent<MechaComponentHitBoxRoot>();
        }

        private bool InBattle;

        public void SetInBattle(bool inBattle)
        {
            InBattle = inBattle;
        }

        public GridPos LocalGridPos;

        public void Initialize(GridPos localGP)
        {
            LocalGridPos = localGP;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (InBattle)
            {
                Projectile p = collision.gameObject.GetComponent<Projectile>();
                if (p && p.ProjectileInfo.MechaType != ParentHitBoxRoot.MechaComponentBase.MechaType)
                {
                    ParentHitBoxRoot.MechaComponentBase.MechaComponentInfo.Damage(p.ProjectileInfo.FinalDamage);
                    return;
                }
            }
        }
    }
}