using System.Collections;
using System.Collections.Generic;
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

        private float bladeAttackTick = 0;

        List<MechaComponentHitBox> HittingHitBoxes = new List<MechaComponentHitBox>();

        void Update()
        {
            if (GameManager.Instance.GetState() == GameState.Fighting)
            {
                bladeAttackTick += Time.deltaTime;
                if (bladeAttackTick > BladeInfo.FinalInterval)
                {
                    bladeAttackTick = 0;
                    foreach (MechaComponentHitBox hb in HittingHitBoxes)
                    {
                        hb.ParentHitBoxRoot.MechaComponentBase.Damage(BladeInfo.FinalDamage);
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider c)
        {
            if (GameManager.Instance.GetState() == GameState.Fighting)
            {
                MechaComponentHitBox hb = c.gameObject.GetComponent<MechaComponentHitBox>();
                if (hb && hb.ParentHitBoxRoot.MechaComponentBase.MechaType != BladeInfo.MechaType)
                {
                    HittingHitBoxes.Add(hb);
                    return;
                }
            }
        }

        private void OnTriggerExit(Collider c)
        {
            if (GameManager.Instance.GetState() == GameState.Fighting)
            {
                MechaComponentHitBox hb = c.gameObject.GetComponent<MechaComponentHitBox>();
                if (hb && hb.ParentHitBoxRoot.MechaComponentBase.CheckAlive() && hb.ParentHitBoxRoot.MechaComponentBase.MechaType != BladeInfo.MechaType)
                {
                    HittingHitBoxes.Remove(hb);
                    return;
                }
            }
        }
    }
}