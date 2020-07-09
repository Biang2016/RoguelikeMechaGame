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

        public delegate GridPosR CoordinateTransformationDelegate(GridPosR gp);

        public CoordinateTransformationDelegate CoordinateTransformationHandler_FromPosToMatrixIndex { get; private set; }
        public CoordinateTransformationDelegate CoordinateTransformationHandler_FromMatrixIndexToPos { get; private set; }
        public CoordinateTransformationDelegate CoordinateTransformationHandler_FromPosToMatrixIndex_Diff { get; private set; }
        public CoordinateTransformationDelegate CoordinateTransformationHandler_FromMatrixIndexToPos_Diff { get; private set; }

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
        /// Create a new inventory
        /// </summary>
        /// <param name="inventoryName">The inventory name</param>
        /// <param name="dragArea"></param>
        /// <param name="gridSize">the gridSize of inventory</param>
        /// <param name="rows">how many rows in total</param>
        /// <param name="columns">how many columns in total</param>
        /// <param name="unlockedPartialGrids">is there any grid locked at the beginning</param>
        /// <param name="unlockedGridCount">how many grids are locked at the beginning</param>
        /// <param name="rotateItemKeyDownHandler">this delegate should return a bool whether the rotate item key is pressed down</param>
        /// <param name="coordinateTransformationHandler_FromPosToMatrixIndex"></param>
        /// <param name="coordinateTransformationHandler_FromMatrixIndexToPos"></param>
        /// <param name="coordinateTransformationHandler_FromPosToMatrixIndex_Diff"></param>
        /// <param name="coordinateTransformationHandler_FromMatrixIndexToPos_Diff"></param>
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
            CoordinateTransformationDelegate coordinateTransformationHandler_FromMatrixIndexToPos,
            CoordinateTransformationDelegate coordinateTransformationHandler_FromPosToMatrixIndex_Diff,
            CoordinateTransformationDelegate coordinateTransformationHandler_FromMatrixIndexToPos_Diff
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
            CoordinateTransformationHandler_FromPosToMatrixIndex_Diff = coordinateTransformationHandler_FromPosToMatrixIndex_Diff;
            CoordinateTransformationHandler_FromMatrixIndexToPos_Diff = coordinateTransformationHandler_FromMatrixIndexToPos_Diff;

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
            bool canPlaceDirectly = CheckSpaceAvailable(item.OccupiedGridPositions_Matrix, GridPos.Zero);
            if (canPlaceDirectly)
            {
                AddItem(item);
                return true;
            }

            bool placeFound = FindSpaceToPutItem(item);
            if (placeFound)
            {
                AddItem(item);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool FindSpaceToPutItem(InventoryItem item)
        {
            foreach (GridPosR.Orientation o in Enum.GetValues(typeof(GridPosR.Orientation)))
            {
                if (FindSpaceToPutRotatedItem(item, o)) return true;
            }

            return false;
        }

        private bool FindSpaceToPutRotatedItem(InventoryItem item, GridPosR.Orientation orientation)
        {
            GridPosR.Orientation oriRot = item.GridPos_Matrix.orientation;
            item.GridPos_Matrix.orientation = GridPosR.Orientation.Up;

            GridRect space = item.BoundingRect;

            bool heightWidthSwap = orientation == GridPosR.Orientation.Right || orientation == GridPosR.Orientation.Left;

            GridPos temp_rot = GridPos.RotateGridPos(space.position, orientation);
            GridPos start = temp_rot - (GridPos) item.GridPos_Matrix;

            for (int z = 0 - start.z; z < Rows - (heightWidthSwap ? space.z_max : space.x_max); z++)
            {
                for (int x = 0 - start.x; x < Columns - (heightWidthSwap ? space.x_max : space.z_max); x++)
                {
                    bool canHold = true;
                    foreach (GridPos gp_matrix in item.OccupiedGridPositions_Matrix)
                    {
                        GridPos rot_gp = GridPos.RotateGridPos(gp_matrix, orientation);
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
                    }

                    if (canHold)
                    {
                        item.GridPos_Matrix.x = x;
                        item.GridPos_Matrix.z = z;
                        item.GridPos_Matrix.orientation = orientation;
                        return true;
                    }
                }
            }

            item.GridPos_Matrix.orientation = oriRot;
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

        private void AddItem(InventoryItem item)
        {
            InventoryItems.Add(item);
            OnAddItemSucAction?.Invoke(item);
            foreach (GridPos gp in item.OccupiedGridPositions_Matrix)
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
                foreach (GridPos gp_matrix in item.OccupiedGridPositions_Matrix)
                {
                    InventoryGridMatrix[gp_matrix.x, gp_matrix.z].State = InventoryGrid.States.Available;
                }

                InventoryItems.Remove(item);
                OnRemoveItemSucAction?.Invoke(item);
            }
        }

        public void PickUpItem(InventoryItem item)
        {
            if (InventoryItems.Contains(item))
            {
                foreach (GridPos gp_matrix in item.OccupiedGridPositions_Matrix)
                {
                    InventoryGridMatrix[gp_matrix.x, gp_matrix.z].State = InventoryGrid.States.TempUnavailable;
                }
            }
        }

        public void PutDownItem(InventoryItem item)
        {
            if (InventoryItems.Contains(item))
            {
                foreach (GridPos gp_matrix in item.OccupiedGridPositions_Matrix)
                {
                    InventoryGridMatrix[gp_matrix.x, gp_matrix.z].State = InventoryGrid.States.Unavailable;
                }
            }
        }

        public void RefreshConflictAndIsolation()
        {
            RefreshConflictAndIsolation(out List<InventoryItem> conflictItems, out List<InventoryItem> isolatedItems);
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
                foreach (GridPos gp_matrix in item.OccupiedGridPositions_Matrix)
                {
                    if (gp_matrix.x < 0 || gp_matrix.x >= Rows
                                        || gp_matrix.z < 0 || gp_matrix.z >= Columns)
                    {
                        hasConflict = true;
                        conflictGridPositions.Add(gp_matrix);
                    }
                    else
                    {
                        if (InventoryItemMatrix[gp_matrix.x, gp_matrix.z] != null)
                        {
                            hasConflict = true;
                            conflictGridPositions.Add(gp_matrix);
                        }
                        else
                        {
                            InventoryItemMatrix[gp_matrix.x, gp_matrix.z] = item;
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
                foreach (GridPos gp_matrix in item.OccupiedGridPositions_Matrix)
                {
                    connectedMatrix[gp_matrix.x, gp_matrix.z] = 1;
                }
            }

            Queue<GridPos> connectedQueue = new Queue<GridPos>();

            foreach (GridPos coreGP in coreGPs)
            {
                connectedMatrix[coreGP.x, coreGP.z] = 2;
                connectedQueue.Enqueue(coreGP);
            }

            void connectPos(int col, int row)
            {
                if (row < 0 || row >= Rows || col < 0 || col >= Columns)
                {
                    return;
                }
                else
                {
                    if (connectedMatrix[col, row] == 1)
                    {
                        connectedQueue.Enqueue(new GridPos(col, row));
                        connectedMatrix[col, row] = 2;
                    }
                }
            }

            while (connectedQueue.Count > 0)
            {
                GridPos gp = connectedQueue.Dequeue();
                connectPos(gp.x + 1, gp.z);
                connectPos(gp.x - 1, gp.z);
                connectPos(gp.x, gp.z - 1);
                connectPos(gp.x, gp.z + 1);
            }

            for (int col = 0; col < Columns; col++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    if (connectedMatrix[col, row] == 1)
                    {
                        isolatedGridPositions.Add((new GridPos(col, row)));
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
            InventoryItem item = InventoryItemMatrix[gp_matrix.x, gp_matrix.z];
            if (item != null)
            {
                GridPos gp_local_noRotate = gp_matrix - (GridPos) item.GridPos_Matrix;
                GridPos gp_local_rotate = GridPos.RotateGridPos(gp_local_noRotate, (GridPosR.Orientation) ((4 - (int) item.GridPos_Matrix.orientation) % 4));
                item.OnConflictedHandler?.Invoke(gp_local_rotate);
            }
        }

        private void AddIsolatedComponentIndicator(GridPos gp_matrix)
        {
            InventoryItem item = InventoryItemMatrix[gp_matrix.x, gp_matrix.z];
            item?.OnIsolatedHandler?.Invoke(true);
        }

        public void MoveAllItemTogether(GridPos delta_local_GP)
        {
            GridPos delta_matrix = CoordinateTransformationHandler_FromPosToMatrixIndex_Diff(delta_local_GP);
            foreach (InventoryItem item in InventoryItems)
            {
                foreach (GridPos gp_matrix in item.OccupiedGridPositions_Matrix)
                {
                    GridPos newGP = gp_matrix + delta_matrix;
                    if (newGP.x >= Columns || newGP.x < 0)
                    {
                        MoveAllItemTogether(new GridPos(0, delta_matrix.z));
                        return;
                    }

                    if (newGP.z >= Rows || newGP.z < 0)
                    {
                        MoveAllItemTogether(new GridPos(delta_matrix.x, 0));
                        return;
                    }
                }
            }

            foreach (InventoryItem item in InventoryItems)
            {
                GridPosR newGridPos_Matrix = item.GridPos_Matrix + delta_matrix;
                item.SetGridPosition(newGridPos_Matrix);
            }

            RefreshConflictAndIsolation();
        }
    }
}