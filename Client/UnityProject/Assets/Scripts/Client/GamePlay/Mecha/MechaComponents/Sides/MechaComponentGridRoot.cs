using System;
using System.Collections.Generic;
using System.Linq;
using BiangStudio.GameDataFormat.Grid;
using GameCore;
using UnityEngine;

namespace Client
{
    [ExecuteInEditMode]
    public class MechaComponentGridRoot : ForbidLocalMoveRoot
    {
        internal MechaComponent MechaComponent;
        public List<MechaComponentGrid> mechaComponentGrids = new List<MechaComponentGrid>();

        [NonSerialized]
        public List<MechaComponentHitBox> HitBoxes = new List<MechaComponentHitBox>();

        void Awake()
        {
            mechaComponentGrids = GetComponentsInChildren<MechaComponentGrid>().ToList();
            MechaComponent = GetComponentInParent<MechaComponent>();
            HitBoxes = GetComponentsInChildren<MechaComponentHitBox>().ToList();
            foreach (MechaComponentHitBox hitBox in HitBoxes)
            {
                hitBox.LocalGridPos = GridPos.GetGridPosByLocalTransXZ(hitBox.transform, ConfigManager.GridSize);
            }
        }

        public void SetLayer(int layer)
        {
            foreach (MechaComponentHitBox hb in HitBoxes)
            {
                hb.gameObject.layer = layer;
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

#if UNITY_EDITOR
        public void RefreshConfig()
        {
            mechaComponentGrids = GetComponentsInChildren<MechaComponentGrid>().ToList();
            HitBoxes = GetComponentsInChildren<MechaComponentHitBox>().ToList();
        }

        public List<GridPos> GetOccupiedPositions()
        {
            List<GridPos> res = new List<GridPos>();
            foreach (MechaComponentGrid mcg in mechaComponentGrids)
            {
                res.Add(mcg.GetGridPos());
            }

            return res;
        }

        public List<GridPos> GetAllSlotPositions_Local()
        {
            List<GridPos> res = new List<GridPos>();
            foreach (MechaComponentGrid grid in mechaComponentGrids)
            {
                GridPos gp = grid.GetGridPos();
                grid.OnSlotEnumFlag_EditorChanged();
                if (grid.Slots[GridPosR.Orientation.Left].IsCandidate)
                {
                    res.Add(gp + new GridPos(-1, 0));
                }

                if (grid.Slots[GridPosR.Orientation.Right].IsCandidate)
                {
                    res.Add(gp + new GridPos(1, 0));
                }

                if (grid.Slots[GridPosR.Orientation.Up].IsCandidate)
                {
                    res.Add(gp + new GridPos(0, 1));
                }

                if (grid.Slots[GridPosR.Orientation.Down].IsCandidate)
                {
                    res.Add(gp + new GridPos(0, -1));
                }
            }

            return res;
        }
#endif

        public void SetSlotLightsShown(bool shown)
        {
            foreach (MechaComponentGrid mcg in mechaComponentGrids)
            {
                mcg.SetSlotLightsShown(shown);
            }
        }

        public void SetGridShown(bool shown)
        {
            foreach (MechaComponentGrid mcg in mechaComponentGrids)
            {
                mcg.SetGridShown(shown);
            }
        }

        public void SetConflictIndicatorShown(bool shown)
        {
            foreach (MechaComponentGrid grid in mechaComponentGrids)
            {
                grid.SetForbidIndicatorShown(shown);
            }
        }

        public void SetIsolatedIndicatorShown(bool shown)
        {
            foreach (MechaComponentGrid grid in mechaComponentGrids)
            {
                grid.SetIsolatedIndicatorShown(shown);
            }
        }

        public void SetGridConflicted(GridPos gridPos)
        {
            foreach (MechaComponentGrid grid in mechaComponentGrids)
            {
                GridPos gp = grid.GetGridPos();
                if (gp.x == gridPos.x && gp.z == gridPos.z)
                {
                    grid.IsConflicted = true;
                    grid.SetForbidIndicatorShown(true);
                }
            }
        }

        public void ResetAllGridConflict()
        {
            foreach (MechaComponentGrid grid in mechaComponentGrids)
            {
                grid.IsConflicted = false;
                grid.SetForbidIndicatorShown(false);
            }
        }
    }
}