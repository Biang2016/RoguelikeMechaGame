﻿using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace GameCore
{
    [Serializable]
    public class BagInfo
    {
        public bool InfiniteComponents = false;

        public BagGridInfo[,] BagGridMatrix = new BagGridInfo[10, 10]; // column, row
        private List<BagItemInfo> bagItemInfos = new List<BagItemInfo>();

        private int bagGridNumber = 0;

        public int BagGridNumber
        {
            get { return bagGridNumber; }

            set
            {
                if (bagGridNumber != value)
                {
                    bagGridNumber = value;
                    RefreshBagGrid();
                }
            }
        }

        public BagInfo()
        {
            for (int x = 0; x < 10; x++)
            {
                for (int z = 0; z < 10; z++)
                {
                    BagGridInfo bgi = new BagGridInfo();
                    bgi.State = BagGridInfo.States.Unavailable;
                    BagGridMatrix[z, x] = bgi;
                }
            }
        }

        public void RefreshBagGrid()
        {
            int count = 0;
            for (int x = 0; x < 10; x++)
            {
                for (int z = 0; z < 10; z++)
                {
                    count++;
                    BagGridMatrix[z, x].State = count > bagGridNumber ? BagGridInfo.States.Locked : BagGridInfo.States.Available;
                }
            }
        }

        public bool TryAddItem(BagItemInfo bii)
        {
            bool canPlaceDirectly = CheckSpaceAvailable(bii.OccupiedGridPositions, GridPos.Zero);
            if (canPlaceDirectly)
            {
                AddItem(bii, bii.GridPos.orientation, bii.OccupiedGridPositions);
                return true;
            }

            bool placeFound = FindSpaceToPutItem(bii, out GridPos.Orientation orientation, out List<GridPos> realOccupiedGPs);
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

        private bool FindSpaceToPutItem(BagItemInfo bii, out GridPos.Orientation orientation, out List<GridPos> realOccupiedGPs)
        {
            orientation = GridPos.Orientation.Up;
            foreach (string s in Enum.GetNames(typeof(GridPos.Orientation)))
            {
                if (FindSpaceToPutRotatedItem(bii, orientation, out realOccupiedGPs)) return true;
            }

            realOccupiedGPs = new List<GridPos>();
            return false;
        }

        private bool FindSpaceToPutRotatedItem(BagItemInfo bii, GridPos.Orientation orientation, out List<GridPos> realOccupiedGPs)
        {
            GridRect space = bii.Size;

            bool heightWidthSwap = orientation == GridPos.Orientation.Right || orientation == GridPos.Orientation.Left;

            GridPos temp = new GridPos(space.x, space.z);
            GridPos temp_rot = GridPos.RotateGridPos(temp, orientation);
            int xStart_temp = temp_rot.x;
            int zStart_temp = temp_rot.z;

            realOccupiedGPs = new List<GridPos>();
            for (int z = 0 - zStart_temp; z <= 10 - (heightWidthSwap ? space.width : space.height) - zStart_temp; z++)
            {
                for (int x = 0 - xStart_temp; x <= 10 - (heightWidthSwap ? space.height : space.width) - xStart_temp; x++)
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

                        if (!BagGridMatrix[col, row].Available)
                        {
                            canHold = false;
                            break;
                        }

                        realOccupiedGPs.Add(new GridPos(col, row, GridPos.Orientation.Up));
                    }

                    if (canHold)
                    {
                        bii.GridPos.x = x + space.x;
                        bii.GridPos.z = z + space.z;
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

                if (!BagGridMatrix[addedGP.x, addedGP.z].Available)
                {
                    return false;
                }
            }

            return true;
        }

        public UnityAction<BagItemInfo> OnAddItemSucAction;

        private void AddItem(BagItemInfo bii, GridPos.Orientation orientation, List<GridPos> realOccupiedGPs)
        {
            bii.OccupiedGridPositions = realOccupiedGPs;
            bii.GridPos.orientation = orientation;
            bii.RefreshSize();
            bagItemInfos.Add(bii);
            OnAddItemSucAction?.Invoke(bii);
            foreach (GridPos gp in realOccupiedGPs)
            {
                BagGridMatrix[gp.x, gp.z].State = BagGridInfo.States.Unavailable;
            }
        }

        public void MoveItem(List<GridPos> oldOccupiedGPs, List<GridPos> newOccupiedGPs)
        {
            foreach (GridPos gp in oldOccupiedGPs)
            {
                Assert.AreEqual(BagGridMatrix[gp.x, gp.z].State, BagGridInfo.States.TempUnavailable);
                BagGridMatrix[gp.x, gp.z].State = BagGridInfo.States.Available;
            }

            foreach (GridPos gp in newOccupiedGPs)
            {
                Assert.AreEqual(BagGridMatrix[gp.x, gp.z].State, BagGridInfo.States.Available);
                BagGridMatrix[gp.x, gp.z].State = BagGridInfo.States.Unavailable;
            }
        }

        public UnityAction<BagItemInfo> OnRemoveItemSucAction;

        public void RemoveItem(BagItemInfo bii)
        {
            if (bagItemInfos.Contains(bii))
            {
                foreach (GridPos gp in bii.OccupiedGridPositions)
                {
                    BagGridMatrix[gp.x, gp.z].State = BagGridInfo.States.Available;
                }

                bagItemInfos.Remove(bii);
                OnRemoveItemSucAction?.Invoke(bii);
            }
        }

        public void PickUpItem(BagItemInfo bii)
        {
            if (bagItemInfos.Contains(bii))
            {
                foreach (GridPos gp in bii.OccupiedGridPositions)
                {
                    BagGridMatrix[gp.x, gp.z].State = BagGridInfo.States.TempUnavailable;
                }
            }
        }
    }
}