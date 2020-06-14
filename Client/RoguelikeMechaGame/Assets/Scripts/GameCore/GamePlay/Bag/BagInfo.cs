using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
                    int count = 0;
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            count++;
                            BagGridMatrix[j, i].State = count > value ? BagGridInfo.States.Locked : BagGridInfo.States.Available;
                        }
                    }

                    bagGridNumber = value;
                }
            }
        }

        public BagInfo(int bagGridNumber)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    BagGridInfo bgi = new BagGridInfo();
                    bgi.State = BagGridInfo.States.Unavailable;
                    BagGridMatrix[j, i] = bgi;
                }
            }

            BagGridNumber = bagGridNumber;
        }

        public bool TryAddItem(BagItemInfo bii)
        {
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

        public bool TryAddItem(BagItemInfo bii, GridPos.Orientation orientation, List<GridPos> realGridPoses)
        {
            bool placeFound = CheckSpaceAvailable(realGridPoses);
            if (placeFound)
            {
                AddItem(bii, orientation, realGridPoses);
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
            IntRect space = bii.Size;

            bool heightWidthSwap = orientation == GridPos.Orientation.Right || orientation == GridPos.Orientation.Left;

            GridPos temp = new GridPos(space.x, space.z);
            GridPos temp_rot = GridPos.RotateGridPos(temp, orientation);
            int xStart_temp = temp_rot.x;
            int zStart_temp = temp_rot.z;

            realOccupiedGPs = new List<GridPos>();
            for (int i = 0 - zStart_temp; i <= 10 - (heightWidthSwap ? space.width : space.height) - zStart_temp; i++)
            {
                for (int j = 0 - xStart_temp; j <= 10 - (heightWidthSwap ? space.height : space.width) - xStart_temp; j++)
                {
                    bool canHold = true;
                    foreach (GridPos gp in bii.OccupiedGridPositions)
                    {
                        GridPos rot_gp = GridPos.RotateGridPos(gp, orientation);
                        int col = j + rot_gp.x;
                        int row = i + rot_gp.z;
                        if (col < 0 || col >= 10 || row < 0 || col >= 10)
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
                        bii.GridPos.x = j;
                        bii.GridPos.z = i;
                        bii.GridPos.orientation = orientation;
                        return true;
                    }

                    realOccupiedGPs.Clear();
                }
            }

            return false;
        }

        public bool CheckSpaceAvailable(List<GridPos> realGridPoses)
        {
            foreach (GridPos gp in realGridPoses)
            {
                if (gp.x < 0 || gp.x >= 10 || gp.z < 0 || gp.z >= 10)
                {
                    return false;
                }

                if (!BagGridMatrix[gp.x, gp.z].Available)
                {
                    return false;
                }
            }

            return true;
        }

        public bool CheckSpaceLocked(List<GridPos> realGridPoses, GridPos offset)
        {
            foreach (GridPos gp in realGridPoses)
            {
                if (gp.x + offset.x < 0 || gp.x + offset.x >= 10 || gp.z + offset.z < 0 || gp.z + offset.z >= 10)
                {
                    return false;
                }

                if (BagGridMatrix[gp.x + offset.x, gp.z + offset.z].Locked)
                {
                    return false;
                }
            }

            return true;
        }

        public UnityAction<BagItemInfo> OnAddItemSucAction;

        private void AddItem(BagItemInfo bii, GridPos.Orientation orientation, List<GridPos> realOccupiedGPs)
        {
            GridPos gridPos = new GridPos(bii.GridPos.x, bii.GridPos.z, orientation);
            bii.OccupiedGridPositions = realOccupiedGPs;
            bii.GridPos = gridPos;
            bii.RefreshSize();
            bagItemInfos.Add(bii);
            OnAddItemSucAction?.Invoke(bii);
            foreach (GridPos gp in realOccupiedGPs)
            {
                BagGridMatrix[gp.x, gp.z].State = BagGridInfo.States.Unavailable;
            }
        }

        public UnityAction<BagItemInfo> OnRemoveItemSucAction;

        public void RemoveItem(BagItemInfo bii, bool temporary)
        {
            if (bagItemInfos.Contains(bii))
            {
                foreach (GridPos gp in bii.OccupiedGridPositions)
                {
                    BagGridMatrix[gp.x, gp.z].State = temporary ? BagGridInfo.States.TempUnavailable : BagGridInfo.States.Available;
                }

                if (!temporary)
                {
                    bagItemInfos.Remove(bii);
                    OnRemoveItemSucAction?.Invoke(bii);
                }
            }
        }
    }
}