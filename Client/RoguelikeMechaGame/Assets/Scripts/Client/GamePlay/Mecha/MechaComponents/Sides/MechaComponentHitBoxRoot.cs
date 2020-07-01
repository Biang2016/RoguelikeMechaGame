using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Client
{
    public class MechaComponentHitBoxRoot : ForbidLocalMoveRoot
    {
        internal MechaComponentBase MechaComponentBase;
        internal List<MechaComponentHitBox> HitBoxes = new List<MechaComponentHitBox>();

        void Awake()
        {
            MechaComponentBase = GetComponentInParent<MechaComponentBase>();
            HitBoxes = GetComponentsInChildren<MechaComponentHitBox>().ToList();
        }

        public void SetInBattle(bool inBattle)
        {
            foreach (MechaComponentHitBox hitBox in HitBoxes)
            {
                hitBox.SetInBattle(inBattle);
            }
        }
    }
}