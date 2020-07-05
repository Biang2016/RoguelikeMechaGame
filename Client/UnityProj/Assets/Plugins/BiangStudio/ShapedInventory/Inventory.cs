﻿using System;
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

        public delegate GridPos CoordinateTransformationDelegate(GridPos gp);

        private CoordinateTransformationDelegate CoordinateTransformationHandler_FromPosToMatrixIndex;
        private CoordinateTransformationDelegate CoordinateTransformationHandler_FromMatrixIndexToPos;

        public delegate MonoBehaviour InstantiatePrefabDelegate(Transform parent);

        /// <summary>
        /// This handler should return a signal whether rotate the backpack(e.g. return Input.GetKeyDown(KeyCode.R);)
        /// </summary>
        public KeyDownDelegate RotateItemKeyDownHandler;

        /// <summary>
        /// This handler should return a signal whether toggles the backpack debug mode(e.g. return Input.GetKeyDown(KeyCode.U);) 
        /// </summary>
        public KeyDownDelegate ToggleDebugKeyDownHandler;

        public InventoryGrid[,] InventoryGridMatrix; // column, row
        public InventoryItem[,] InventoryItemMatrix; // column, row

        public List<InventoryItem> InventoryItems = new List<InventoryItem>();

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
                    RefreshInventoryGrids();
                }
            }
        }

        public LogErrorDelegate LogErrorHandler;

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="inventoryName"></param>
        /// <param name="dragArea"></param>
        /// <param name="gridSize"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="unlockedPartialGrids"></param>
        /// <param name="unlockedGridCount"></param>
        /// <param name="rotateItemKeyDownHandler"></param>
        /// <param name="coordinateTransformationHandler_FromPosToMatrixIndex"></param>
        /// <param name="coordinateTransformationHandler_FromMatrixIndexToPos"></param>
        public Inventory(
            string inventoryName,
            DragArea dragArea,
            int gridSize,
            int rows,
            int columns,
            bool unlockedPartialGrids,
            int unlockedGridCount,
            KeyDownDelegate rotateItemKeyDownHandler,
            CoordinateTransformationDelegate coordinateTransformationHandler_FromPosToMatrixIndex,
            CoordinateTransformationDelegate coordinateTransformationHandler_FromMatrixIndexToPos
        )
        {
            InventoryName = inventoryName;
            DragArea = dragArea;
            GridSize = gridSize;
            Rows = rows;
            Columns = columns;
            UnlockedPartialGrids = unlockedPartialGrids;
            this.unlockedGridCount = unlockedGridCount;

            RotateItemKeyDownHandler = rotateItemKeyDownHandler;
            CoordinateTransformationHandler_FromPosToMatrixIndex = coordinateTransformationHandler_FromPosToMatrixIndex;
            CoordinateTransformationHandler_FromMatrixIndexToPos = coordinateTransformationHandler_FromMatrixIndexToPos;

            InventoryGridMatrix = new InventoryGrid[Columns, Rows];
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    InventoryGrid ig = new InventoryGrid();
                    ig.State = InventoryGrid.States.Unavailable;
                    InventoryGridMatrix[col, row] = ig;
                }
            }

            InventoryItemMatrix = new InventoryItem[Columns, Rows];
        }

        public void LogError(string log)
        {
            LogErrorHandler?.Invoke($"BiangStudio.ShapedInventory Error: {log}");
        }

        public void RefreshInventoryGrids()
        {
            int count = 0;
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    count++;
                    InventoryGridMatrix[col, row].State = (count > unlockedGridCount && UnlockedPartialGrids) ? InventoryGrid.States.Locked : InventoryGrid.States.Available;
                }
            }
        }

        public bool TryAddItem(InventoryItem item)
        {
            bool canPlaceDirectly = CheckSpaceAvailable(item.OccupiedGridPositions, GridPos.Zero);
            if (canPlaceDirectly)
            {
                AddItem(item, item.GridPos.orientation, item.OccupiedGridPositions);
                return true;
            }

            bool placeFound = FindSpaceToPutItem(item, out GridPosR.Orientation orientation, out List<GridPos> realOccupiedGPs);
            if (placeFound)
            {
                AddItem(item, orientation, realOccupiedGPs);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool FindSpaceToPutItem(InventoryItem item, out GridPosR.Orientation orientation, out List<GridPos> realOccupiedGPs)
        {
            orientation = GridPosR.Orientation.Up;
            foreach (string s in Enum.GetNames(typeof(GridPosR.Orientation)))
            {
                if (FindSpaceToPutRotatedItem(item, orientation, out realOccupiedGPs)) return true;
            }

            realOccupiedGPs = new List<GridPos>();
            return false;
        }

        private bool FindSpaceToPutRotatedItem(InventoryItem item, GridPosR.Orientation orientation, out List<GridPos> realOccupiedGPs)
        {
            GridRect space = item.BoundingRect;

            bool heightWidthSwap = orientation == GridPosR.Orientation.Right || orientation == GridPosR.Orientation.Left;

            GridPos temp = new GridPos(space.position.x, space.position.z);
            GridPos temp_rot = GridPos.RotateGridPos(temp, orientation);
            int xStart_temp = temp_rot.x;
            int zStart_temp = temp_rot.z;

            realOccupiedGPs = new List<GridPos>();
            for (int z = 0 - zStart_temp; z <= Columns - (heightWidthSwap ? space.size.x : space.size.z) - zStart_temp; z++)
            {
                for (int x = 0 - xStart_temp; x <= Rows - (heightWidthSwap ? space.size.z : space.size.x) - xStart_temp; x++)
                {
                    bool canHold = true;
                    foreach (GridPos gp in item.OccupiedGridPositions)
                    {
                        GridPos rot_gp = GridPos.RotateGridPos(gp, orientation);
                        int col = x + rot_gp.x;
                        int row = z + rot_gp.z;
                        if (col < 0 || col >= Columns || row < 0 || row >= Rows)
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
                        item.GridPos.x = x + space.position.x;
                        item.GridPos.z = z + space.position.z;
                        item.GridPos.orientation = orientation;
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

                if (addedGP.x < 0 || addedGP.x >= Rows || addedGP.z < 0 || addedGP.z >= Columns)
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

        private void AddItem(InventoryItem item, GridPosR.Orientation orientation, List<GridPos> realOccupiedGPs)
        {
            item.OccupiedGridPositions = realOccupiedGPs;
            item.GridPos.orientation = orientation;
            item.RefreshSize();
            InventoryItems.Add(item);
            OnAddItemSucAction?.Invoke(item);
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

        public void RemoveItem(InventoryItem item)
        {
            if (InventoryItems.Contains(item))
            {
                foreach (GridPos gp in item.OccupiedGridPositions)
                {
                    InventoryGridMatrix[gp.x, gp.z].State = InventoryGrid.States.Available;
                }

                InventoryItems.Remove(item);
                OnRemoveItemSucAction?.Invoke(item);
            }
        }

        public void PickUpItem(InventoryItem item)
        {
            if (InventoryItems.Contains(item))
            {
                foreach (GridPos gp in item.OccupiedGridPositions)
                {
                    InventoryGridMatrix[gp.x, gp.z].State = InventoryGrid.States.TempUnavailable;
                }
            }
        }

        public void RefreshConflictAndIsolation(out List<InventoryItem> conflictItems, out List<InventoryItem> isolatedItems)
        {
            foreach (InventoryItem item in InventoryItems)
            {
                item.OnIsolatedHandler?.Invoke(false);
                item.OnResetConflictHandler?.Invoke();
            }

            List<GridPos> coreGPs = new List<GridPos>();
            List<InventoryItem> notConflictItems = new List<InventoryItem>();

            // Find conflict items
            List<GridPos> conflictGridPositions = new List<GridPos>();
            conflictItems = new List<InventoryItem>();

            for (int col = 0; col < Columns; col++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    InventoryItemMatrix[col, row] = null;
                }
            }

            foreach (InventoryItem item in InventoryItems)
            {
                bool isRootItem = item.AmIRootItemInIsolationCalculationHandler != null && item.AmIRootItemInIsolationCalculationHandler.Invoke();
                bool hasConflict = false;
                foreach (GridPos gp in item.OccupiedGridPositions)
                {
                    GridPos gp_matrix = CoordinateTransformationHandler_FromPosToMatrixIndex.Invoke(gp);

                    if (gp_matrix.x < 0 || gp_matrix.x >= Rows
                                        || gp_matrix.z < 0 || gp_matrix.z >= Columns)
                    {
                        hasConflict = true;
                        conflictGridPositions.Add(gp_matrix);
                    }
                    else
                    {
                        if (InventoryItemMatrix[gp_matrix.z, gp_matrix.x] != null)
                        {
                            hasConflict = true;
                            conflictGridPositions.Add(gp_matrix);
                        }
                        else
                        {
                            InventoryItemMatrix[gp_matrix.z, gp_matrix.x] = item;
                            if (isRootItem) coreGPs.Add(gp_matrix);
                        }
                    }
                }

                if (hasConflict)
                {
                    conflictItems.Add(item);
                }
                else
                {
                    notConflictItems.Add(item);
                }
            }

            foreach (GridPos gp in conflictGridPositions)
            {
                AddForbidComponentIndicator(gp);
            }

            // Find isolated components
            List<GridPos> isolatedGridPositions = new List<GridPos>();
            isolatedItems = new List<InventoryItem>();

            int[,] connectedMatrix = new int[Columns, Rows];

            foreach (InventoryItem item in notConflictItems)
            {
                foreach (GridPos gp in item.OccupiedGridPositions)
                {
                    GridPos gp_matrix = CoordinateTransformationHandler_FromPosToMatrixIndex.Invoke(gp);
                    connectedMatrix[gp_matrix.z, gp_matrix.x] = 1;
                }
            }

            Queue<GridPos> connectedQueue = new Queue<GridPos>();

            foreach (GridPos coreGP in coreGPs)
            {
                connectedMatrix[coreGP.z, coreGP.x] = 2;
                connectedQueue.Enqueue(coreGP);
            }

            void connectPos(int column, int row)
            {
                if (row < 0 || row >= Rows || column < 0 || column >= Columns)
                {
                    return;
                }
                else
                {
                    if (connectedMatrix[column, row] == 1)
                    {
                        connectedQueue.Enqueue(new GridPos(row, column));
                        connectedMatrix[column, row] = 2;
                    }
                }
            }

            while (connectedQueue.Count > 0)
            {
                GridPos gp = connectedQueue.Dequeue();
                connectPos(gp.z + 1, gp.x);
                connectPos(gp.z - 1, gp.x);
                connectPos(gp.z, gp.x - 1);
                connectPos(gp.z, gp.x + 1);
            }

            for (int col = 0; col < Columns; col++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    if (connectedMatrix[col, row] == 1)
                    {
                        isolatedGridPositions.Add((new GridPos(row, col)));
                        InventoryItem isolatedItem = InventoryItemMatrix[col, row];
                        if (!isolatedItems.Contains(isolatedItem))
                        {
                            isolatedItems.Add(isolatedItem);
                        }
                    }
                }
            }

            foreach (GridPos gp in isolatedGridPositions)
            {
                AddIsolatedComponentIndicator(gp);
            }
        }

        private void AddForbidComponentIndicator(GridPos gp_matrix)
        {
            InventoryItem item = InventoryItemMatrix[gp_matrix.z, gp_matrix.x];
            if (item != null)
            {
                GridPos gp = CoordinateTransformationHandler_FromMatrixIndexToPos.Invoke(gp_matrix);
                GridPos gp_local_noRotate = gp - (GridPos) item.GridPos;
                GridPos gp_local_rotate = GridPos.RotateGridPos(gp_local_noRotate, (GridPosR.Orientation) ((4 - (int) item.GridPos.orientation) % 4));
                item.OnConflictedHandler.Invoke(gp_local_rotate);
            }
        }

        private void AddIsolatedComponentIndicator(GridPos gp_matrix)
        {
            InventoryItem item = InventoryItemMatrix[gp_matrix.z, gp_matrix.x];
            item?.OnIsolatedHandler(true);
        }
    }
}