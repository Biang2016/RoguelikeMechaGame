using System.Collections.Generic;
using BiangStudio.GameDataFormat;
using UnityEngine;

namespace Client
{
    public class Blade : MonoBehaviour
    {
        public BladeInfo BladeInfo;

        public void Initialize(BladeInfo bladeInfo)
        {
            BladeInfo = bladeInfo;
        }

        private Fix64 bladeAttackTick = Fix64.Zero;

        List<MechaComponentHitBox> HittingHitBoxes = new List<MechaComponentHitBox>();

        void Update()
        {
            if (GameStateManager.Instance.GetState() == GameState.Fighting)
            {
                bladeAttackTick += Time.deltaTime;
                if (bladeAttackTick > BladeInfo.FinalInterval)
                {
                    bladeAttackTick = Fix64.Zero;
                    foreach (MechaComponentHitBox hb in HittingHitBoxes)
                    {
                        hb.ParentGridRootRoot.MechaComponentBase.MechaComponentInfo.Damage(BladeInfo.FinalDamage);
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider c)
        {
            if (GameStateManager.Instance.GetState() == GameState.Fighting)
            {
                MechaComponentHitBox hb = c.gameObject.GetComponent<MechaComponentHitBox>();
                if (hb && hb.Mecha.MechaInfo.MechaType != BladeInfo.MechaType)
                {
                    HittingHitBoxes.Add(hb);
                    return;
                }
            }
        }

        private void OnTriggerExit(Collider c)
        {
            if (GameStateManager.Instance.GetState() == GameState.Fighting)
            {
                MechaComponentHitBox hb = c.gameObject.GetComponent<MechaComponentHitBox>();
                if (hb && hb.ParentGridRootRoot.MechaComponentBase.MechaComponentInfo.CheckAlive() && hb.Mecha.MechaInfo.MechaType != BladeInfo.MechaType)
                {
                    HittingHitBoxes.Remove(hb);
                    return;
                }
            }
        }
    }
}