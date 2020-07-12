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
    }
}