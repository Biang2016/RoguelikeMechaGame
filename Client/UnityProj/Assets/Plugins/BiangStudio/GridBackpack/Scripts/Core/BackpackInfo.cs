using System;
using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine.Events;

namespace BiangStudio.GridBackpack
{
    [Serializable]
    public class BackpackInfo
    {
        public BackpackGridInfo[,] BackpackGridMatrix = new BackpackGridInfo[10, 10]; // column, row

        public List<BackpackItemInfo> BackpackItemInfos = new List<BackpackItemInfo>();

        private int backpackGridNumber = 0;

        public int BackpackGridNumber
        {
            get { return backpackGridNumber; }

            set
            {
                if (backpackGridNumber != value)
                {
                    backpackGridNumber = value;
                    RefreshBackpackGrid();
                }
            }
        }

        public BackpackInfo(int backpackGridNumber)
        {
            this.backpackGridNumber = backpackGridNumber;
            for (int x = 0; x < 10; x++)
            {
                for (int z = 0; z < 10; z++)
                {
                    BackpackGridInfo bgi = new BackpackGridInfo();
                    bgi.State = BackpackGridInfo.States.Unavailable;
                    BackpackGridMatrix[z, x] = bgi;
                }
            }
        }

        public void RefreshBackpackGrid()
        {
            int count = 0;
            for (int x = 0; x < 10; x++)
            {
                for (int z = 0; z < 10; z++)
                {
                    count++;
                    BackpackGridMatrix[z, x].State = count > backpackGridNumber ? BackpackGridInfo.States.Locked : BackpackGridInfo.States.Available;
                }
            }
        }

        public bool TryAddItem(BackpackItemInfo bii)
        {
            bool canPlaceDirectly = CheckSpaceAvailable(bii.OccupiedGridPositions, GridPos.Zero);
            if (canPlaceDirectly)
            {
                AddItem(bii, bii.GridPos.orientation, bii.OccupiedGridPositions);
                return true;
            }

            bool placeFound = FindSpaceToPutItem(bii, out GridPosR.Orientation orientation, out List<GridPos> realOccupiedGPs);
            if (placeFound)
            {
                AddItem(bii, orientation, realOccupiedGPs);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool FindSpaceToPutItem(BackpackItemInfo bii, out GridPosR.Orientation orientation, out List<GridPos> realOccupiedGPs)
        {
            orientation = GridPosR.Orientation.Up;
            foreach (string s in Enum.GetNames(typeof(GridPosR.Orientation)))
            {
                if (FindSpaceToPutRotatedItem(bii, orientation, out realOccupiedGPs)) return true;
            }

            realOccupiedGPs = new List<GridPos>();
            return false;
        }

        private bool FindSpaceToPutRotatedItem(BackpackItemInfo bii, GridPosR.Orientation orientation, out List<GridPos> realOccupiedGPs)
        {
            GridRect space = bii.BoundingRect;

            bool heightWidthSwap = orientation == GridPosR.Orientation.Right || orientation == GridPosR.Orientation.Left;

            GridPos temp = new GridPos(space.position.x, space.position.z);
            GridPos temp_rot = GridPos.RotateGridPos(temp, orientation);
            int xStart_temp = temp_rot.x;
            int zStart_temp = temp_rot.z;

            realOccupiedGPs = new List<GridPos>();
            for (int z = 0 - zStart_temp; z <= 10 - (heightWidthSwap ? space.size.x : space.size.z) - zStart_temp; z++)
            {
                for (int x = 0 - xStart_temp; x <= 10 - (heightWidthSwap ? space.size.z : space.size.x) - xStart_temp; x++)
                {
                    bool canHold = true;
                    foreach (GridPos gp in bii.OccupiedGridPositions)
                    {
                        GridPos rot_gp = GridPos.RotateGridPos(gp, orientation);
                        int col = x + rot_gp.x;
                        int row = z + rot_gp.z;
                        if (col < 0 || col >= 10 || row < 0 || row >= 10)
                        {
                            canHold = false;
                            break;
                        }

                        if (!BackpackGridMatrix[col, row].Available)
                        {
                            canHold = false;
                            break;
                        }

                        realOccupiedGPs.Add(new GridPosR(col, row, GridPosR.Orientation.Up));
                    }

                    if (canHold)
                    {
                        bii.GridPos.x = x + space.position.x;
                        bii.GridPos.z = z + space.position.z;
                        bii.GridPos.orientation = orientation;
                        return true;
                    }

                    realOccupiedGPs.Clear();
                }
            }

            return false;
        }

        public bool CheckSpaceAvailable(List<GridPos> realGridPoses, GridPos offset)
        {
            foreach (GridPos gp in realGridPoses)
            {
                GridPos addedGP = gp + offset;

                if (addedGP.x < 0 || addedGP.x >= 10 || addedGP.z < 0 || addedGP.z >= 10)
                {
                    return false;
                }

                if (!BackpackGridMatrix[addedGP.x, addedGP.z].Available)
                {
                    return false;
                }
            }

            return true;
        }

        public UnityAction<BackpackItemInfo> OnAddItemSucAction;

        private void AddItem(BackpackItemInfo bii, GridPosR.Orientation orientation, List<GridPos> realOccupiedGPs)
        {
            bii.OccupiedGridPositions = realOccupiedGPs;
            bii.GridPos.orientation = orientation;
            bii.RefreshSize();
            BackpackItemInfos.Add(bii);
            OnAddItemSucAction?.Invoke(bii);
            foreach (GridPos gp in realOccupiedGPs)
            {
                BackpackGridMatrix[gp.x, gp.z].State = BackpackGridInfo.States.Unavailable;
            }
        }

        public void MoveItem(List<GridPos> oldOccupiedGPs, List<GridPos> newOccupiedGPs)
        {
            foreach (GridPos gp in oldOccupiedGPs)
            {
                BackpackGridMatrix[gp.x, gp.z].State = BackpackGridInfo.States.Available;
            }

            foreach (GridPos gp in newOccupiedGPs)
            {
                BackpackGridMatrix[gp.x, gp.z].State = BackpackGridInfo.States.Unavailable;
            }
        }

        public UnityAction<BackpackItemInfo> OnRemoveItemSucAction;

        public void RemoveItem(BackpackItemInfo bii)
        {
            if (BackpackItemInfos.Contains(bii))
            {
                foreach (GridPos gp in bii.OccupiedGridPositions)
                {
                    BackpackGridMatrix[gp.x, gp.z].State = BackpackGridInfo.States.Available;
                }

                BackpackItemInfos.Remove(bii);
                OnRemoveItemSucAction?.Invoke(bii);
            }
        }

        public void PickUpItem(BackpackItemInfo bii)
        {
            if (BackpackItemInfos.Contains(bii))
            {
                foreach (GridPos gp in bii.OccupiedGridPositions)
                {
                    BackpackGridMatrix[gp.x, gp.z].State = BackpackGridInfo.States.TempUnavailable;
                }
            }
        }
    }
}