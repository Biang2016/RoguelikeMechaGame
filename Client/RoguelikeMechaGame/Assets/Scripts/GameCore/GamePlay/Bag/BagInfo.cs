using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace GameCore
{
    [Serializable]
    [LabelText("背包信息")]
    public class BagInfo
    {
        public BagGridInfo[,] BagGridMatrix = new BagGridInfo[10, 10]; // column, row

        [ShowInInspector]
        public List<BagItemInfo> BagItemInfos = new List<BagItemInfo>();

        private int bagGridNumber = 0;

        [ShowInInspector]
        [PropertyOrder(-1)]
        [LabelText("背包容量")]
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

        private bool FindSpaceToPutItem(BagItemInfo bii, out GridPosR.Orientation orientation, out List<GridPos> realOccupiedGPs)
        {
            orientation = GridPosR.Orientation.Up;
            foreach (string s in Enum.GetNames(typeof(GridPosR.Orientation)))
            {
                if (FindSpaceToPutRotatedItem(bii, orientation, out realOccupiedGPs)) return true;
            }

            realOccupiedGPs = new List<GridPos>();
            return false;
        }

        private bool FindSpaceToPutRotatedItem(BagItemInfo bii, GridPosR.Orientation orientation, out List<GridPos> realOccupiedGPs)
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

                        if (!BagGridMatrix[col, row].Available)
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

                if (!BagGridMatrix[addedGP.x, addedGP.z].Available)
                {
                    return false;
                }
            }

            return true;
        }

        public UnityAction<BagItemInfo> OnAddItemSucAction;

        private void AddItem(BagItemInfo bii, GridPosR.Orientation orientation, List<GridPos> realOccupiedGPs)
        {
            bii.OccupiedGridPositions = realOccupiedGPs;
            bii.GridPos.orientation = orientation;
            bii.RefreshSize();
            BagItemInfos.Add(bii);
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
                BagGridMatrix[gp.x, gp.z].State = BagGridInfo.States.Available;
            }

            foreach (GridPos gp in newOccupiedGPs)
            {
                BagGridMatrix[gp.x, gp.z].State = BagGridInfo.States.Unavailable;
            }
        }

        public UnityAction<BagItemInfo> OnRemoveItemSucAction;

        public void RemoveItem(BagItemInfo bii)
        {
            if (BagItemInfos.Contains(bii))
            {
                foreach (GridPos gp in bii.OccupiedGridPositions)
                {
                    BagGridMatrix[gp.x, gp.z].State = BagGridInfo.States.Available;
                }

                BagItemInfos.Remove(bii);
                OnRemoveItemSucAction?.Invoke(bii);
            }
        }

        public void PickUpItem(BagItemInfo bii)
        {
            if (BagItemInfos.Contains(bii))
            {
                foreach (GridPos gp in bii.OccupiedGridPositions)
                {
                    BagGridMatrix[gp.x, gp.z].State = BagGridInfo.States.TempUnavailable;
                }
            }
        }
    }
}