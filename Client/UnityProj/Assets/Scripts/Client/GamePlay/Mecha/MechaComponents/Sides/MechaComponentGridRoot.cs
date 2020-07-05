﻿using System.Collections.Generic;
using System.Linq;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

namespace Client
{
    [ExecuteInEditMode]
    public class MechaComponentGridRoot : ForbidLocalMoveRoot
    {
        private List<MechaComponentGrid> mechaComponentGrids = new List<MechaComponentGrid>();

#if UNITY_EDITOR
        public List<GridPos> GetOccupiedPositions()
        {
            List<GridPos> res = new List<GridPos>();
            if (mechaComponentGrids.Count == 0)
            {
                mechaComponentGrids = GetComponentsInChildren<MechaComponentGrid>().ToList();
            }

            foreach (MechaComponentGrid mcg in mechaComponentGrids)
            {
                res.Add(mcg.GetGridPos());
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
                }
            }
        }

        public void ResetAllGridConflict()
        {
            foreach (MechaComponentGrid grid in mechaComponentGrids)
            {
                grid.IsConflicted = false;
            }
        }
    }
}