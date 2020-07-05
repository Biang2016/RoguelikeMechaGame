using System;
using System.Collections.Generic;
using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine;
using UnityEngine.Events;

namespace BiangStudio.ShapedInventory
{
    public abstract class Inventory
    {
        public string InventoryName;
        public DragArea DragArea;

        public delegate bool KeyDownDelegate();

        public delegate void LogErrorDelegate(string log);

        public delegate MonoBehaviour InstantiatePrefabDelegate(Transform parent);

        public KeyDownDelegate RotateItemKeyDownHandler;

        /// <summary>
        /// This callback will be execute when the backpack is opened or closed
        /// </summary>
        public KeyDownDelegate ToggleDebugKeyDownHandler;

        public int GridSize { get; private set; }
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public bool UnlockedPartialGrids { get; private set; }

        private int unlockedGridCount = 0;

        public int UnlockedGridCount
        {
            get { return unlockedGridCount; }

            private set
            {
                if (unlockedGridCount != value)
                {
                    unlockedGridCount = value;
                    RefreshBackpackGrid();
                }
            }
        }

        public LogErrorDelegate LogErrorHandler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryName"></param>
        /// <param name="dragArea"></param>
        /// <param name="gridSize"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="unlockedPartialGrids"></param>
        /// <param name="unlockedGridCount"></param>
        /// <param name="rotateItemKeyDownHandler"></param>
        public Inventory(
            string inventoryName,
            DragArea dragArea,
            int gridSize,
            int rows,
            int columns,
            bool unlockedPartialGrids,
            int unlockedGridCount,
            KeyDownDelegate rotateItemKeyDownHandler
        )
        {
            InventoryName = inventoryName;
            DragArea = dragArea;
            RotateItemKeyDownHandler = rotateItemKeyDownHandler;
            GridSize = gridSize;
            Rows = rows;
            Columns = columns;
            UnlockedPartialGrids = unlockedPartialGrids;
            this.unlockedGridCount = unlockedGridCount;

            for (int x = 0; x < 10; x++)
            {
                for (int z = 0; z < 10; z++)
                {
                    InventoryGrid ig = new InventoryGrid();
                    ig.State = InventoryGrid.States.Unavailable;
                    InventoryGridMatrix[z, x] = ig;
                }
            }
        }

        public void LogError(string log)
        {
            LogErrorHandler?.Invoke($"BiangStudio.ShapedInventory Error: {log}");
        }

        public InventoryGrid[,] InventoryGridMatrix = new InventoryGrid[10, 10]; // column, row

        public List<InventoryItem> InventoryItems = new List<InventoryItem>();

        public void RefreshBackpackGrid()
        {
            int count = 0;
            for (int x = 0; x < 10; x++)
            {
                for (int z = 0; z < 10; z++)
                {
                    count++;
                    InventoryGridMatrix[z, x].State = (count > unlockedGridCount && UnlockedPartialGrids) ? InventoryGrid.States.Locked : InventoryGrid.States.Available;
                }
            }
        }

        public bool TryAddItem(InventoryItem ii)
        {
            bool canPlaceDirectly = CheckSpaceAvailable(ii.OccupiedGridPositions, GridPos.Zero);
            if (canPlaceDirectly)
            {
                AddItem(ii, ii.GridPos.orientation, ii.OccupiedGridPositions);
                return true;
            }

            bool placeFound = FindSpaceToPutItem(ii, out GridPosR.Orientation orientation, out List<GridPos> realOccupiedGPs);
            if (placeFound)
            {
                AddItem(ii, orientation, realOccupiedGPs);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool FindSpaceToPutItem(InventoryItem ii, out GridPosR.Orientation orientation, out List<GridPos> realOccupiedGPs)
        {
            orientation = GridPosR.Orientation.Up;
            foreach (string s in Enum.GetNames(typeof(GridPosR.Orientation)))
            {
                if (FindSpaceToPutRotatedItem(ii, orientation, out realOccupiedGPs)) return true;
            }

            realOccupiedGPs = new List<GridPos>();
            return false;
        }

        private bool FindSpaceToPutRotatedItem(InventoryItem ii, GridPosR.Orientation orientation, out List<GridPos> realOccupiedGPs)
        {
            GridRect space = ii.BoundingRect;

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
                    foreach (GridPos gp in ii.OccupiedGridPositions)
                    {
                        GridPos rot_gp = GridPos.RotateGridPos(gp, orientation);
                        int col = x + rot_gp.x;
                        int row = z + rot_gp.z;
                        if (col < 0 || col >= 10 || row < 0 || row >= 10)
                        {
                            canHold = false;
                            break;
                        }

                        if (!InventoryGridMatrix[col, row].Available)
                        {
                            canHold = false;
                            break;
                        }

                        realOccupiedGPs.Add(new GridPosR(col, row, GridPosR.Orientation.Up));
                    }

                    if (canHold)
                    {
                        ii.GridPos.x = x + space.position.x;
                        ii.GridPos.z = z + space.position.z;
                        ii.GridPos.orientation = orientation;
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

                if (!InventoryGridMatrix[addedGP.x, addedGP.z].Available)
                {
                    return false;
                }
            }

            return true;
        }

        public UnityAction<InventoryItem> OnAddItemSucAction;

        private void AddItem(InventoryItem ii, GridPosR.Orientation orientation, List<GridPos> realOccupiedGPs)
        {
            ii.OccupiedGridPositions = realOccupiedGPs;
            ii.GridPos.orientation = orientation;
            ii.RefreshSize();
            InventoryItems.Add(ii);
            OnAddItemSucAction?.Invoke(ii);
            foreach (GridPos gp in realOccupiedGPs)
            {
                InventoryGridMatrix[gp.x, gp.z].State = InventoryGrid.States.Unavailable;
            }
        }

        public void MoveItem(List<GridPos> oldOccupiedGPs, List<GridPos> newOccupiedGPs)
        {
            foreach (GridPos gp in oldOccupiedGPs)
            {
                InventoryGridMatrix[gp.x, gp.z].State = InventoryGrid.States.Available;
            }

            foreach (GridPos gp in newOccupiedGPs)
            {
                InventoryGridMatrix[gp.x, gp.z].State = InventoryGrid.States.Unavailable;
            }
        }

        public UnityAction<InventoryItem> OnRemoveItemSucAction;

        public void RemoveItem(InventoryItem ii)
        {
            if (InventoryItems.Contains(ii))
            {
                foreach (GridPos gp in ii.OccupiedGridPositions)
                {
                    InventoryGridMatrix[gp.x, gp.z].State = InventoryGrid.States.Available;
                }

                InventoryItems.Remove(ii);
                OnRemoveItemSucAction?.Invoke(ii);
            }
        }

        public void PickUpItem(InventoryItem ii)
        {
            if (InventoryItems.Contains(ii))
            {
                foreach (GridPos gp in ii.OccupiedGridPositions)
                {
                    InventoryGridMatrix[gp.x, gp.z].State = InventoryGrid.States.TempUnavailable;
                }
            }
        }
    }
}