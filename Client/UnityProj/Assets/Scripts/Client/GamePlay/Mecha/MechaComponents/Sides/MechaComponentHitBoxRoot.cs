using System;
using System.Collections.Generic;
using System.Linq;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GridBackpack;
using GameCore;
using UnityEngine;

namespace Client
{
    public class MechaComponentHitBoxRoot : ForbidLocalMoveRoot
    {
        internal MechaComponentBase MechaComponentBase;
        [NonSerialized]public List<MechaComponentHitBox> HitBoxes = new List<MechaComponentHitBox>();

        void Awake()
        {
            MechaComponentBase = GetComponentInParent<MechaComponentBase>();
            HitBoxes = GetComponentsInChildren<MechaComponentHitBox>().ToList();
            foreach (MechaComponentHitBox hitBox in HitBoxes)
            {
                hitBox.LocalGridPos = GridPos.GetGridPosByLocalTrans(hitBox.transform, ConfigManager.GridSize);
            }
        }

        public void SetInBattle(bool inBattle)
        {
            foreach (MechaComponentHitBox hitBox in HitBoxes)
            {
                hitBox.SetInBattle(inBattle);
            }
        }

        internal MechaComponentHitBox FindHitBox(Collider collider)
        {
            foreach (MechaComponentHitBox hitBox in HitBoxes)
            {
                if (hitBox.BoxCollider == collider)
                {
                    return hitBox;
                }
            }

            return null;
        }
    }
}