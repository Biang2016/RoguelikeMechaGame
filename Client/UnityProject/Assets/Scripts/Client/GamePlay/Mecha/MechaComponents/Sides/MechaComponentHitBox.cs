using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

namespace Client
{
    public class MechaComponentHitBox : MonoBehaviour
    {
        internal MechaComponentGridRoot ParentGridRootRoot;

        internal Mecha Mecha => ParentGridRootRoot.MechaComponent.Mecha;

        internal BoxCollider BoxCollider;

        private void Awake()
        {
            BoxCollider = GetComponentInParent<BoxCollider>();
            ParentGridRootRoot = GetComponentInParent<MechaComponentGridRoot>();
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
                if (p && !p.IsRecycled && p.ProjectileInfo.MechaCamp != ParentGridRootRoot.MechaComponent.MechaCamp)
                {
                    //ParentGridRootRoot.MechaComponent.MechaComponentInfo.Damage(p.ProjectileInfo.FinalDamage);
                    return;
                }
            }
        }
    }
}