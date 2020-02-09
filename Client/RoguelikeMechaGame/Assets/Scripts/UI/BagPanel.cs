using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BagPanel : BaseUIForm
{
    [SerializeField] private GridLayoutGroup ItemContainerGridLayout;
    [SerializeField] private Transform GridContainer;
    public Transform ItemContainer;

    private BagGrid[,] bagGridMatrix = new BagGrid[10, 10]; // column, row
    private List<BagItem> bagItems = new List<BagItem>();

    void Awake()
    {
        UIType.InitUIType(
            isClearStack: false,
            isESCClose: true,
            isClickElsewhereClose: false,
            uiForms_Type: UIFormTypes.Normal,
            uiForms_ShowMode: UIFormShowModes.Normal,
            uiForm_LucencyType: UIFormLucencyTypes.Penetrable);
        BagManager.Instance.BagItemGridSize = Mathf.RoundToInt(ItemContainerGridLayout.cellSize.x);

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                BagGrid big = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BagGrid].AllocateGameObject<BagGrid>(GridContainer);
                big.State = BagGrid.States.Unavailable;
                bagGridMatrix[j, i] = big;
            }
        }
    }

    void Update()
    {
    }

    void Reset()
    {
        foreach (BagItem bi in bagItems)
        {
            bi.PoolRecycle();
        }

        bagItems.Clear();
    }

    public void UnlockBagGridTo(int gridNumber)
    {
        int count = 0;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                count++;
                bagGridMatrix[j, i].State = count > gridNumber ? BagGrid.States.Locked : BagGrid.States.Available;
            }
        }
    }

    public bool TryAddItem(MechaComponentInfo mci, out BagItem bagItem)
    {
        bool placeFound = FindSpaceToPutItem(mci, out GridPos.Orientation orientation, out List<GridPos> realOccupiedGPs);
        if (placeFound)
        {
            AddItem(mci, orientation, realOccupiedGPs, out bagItem);
            return true;
        }
        else
        {
            bagItem = null;
            return false;
        }
    }

    public bool TryAddItem(MechaComponentInfo mci, GridPos.Orientation orientation, List<GridPos> realGridPoses, out BagItem bagItem)
    {
        bool placeFound = CheckSpaceAvailable(realGridPoses);
        if (placeFound)
        {
            AddItem(mci, orientation, realGridPoses, out bagItem);
            return true;
        }
        else
        {
            bagItem = null;
            return false;
        }
    }

    private bool FindSpaceToPutItem(MechaComponentInfo mci, out GridPos.Orientation orientation, out List<GridPos> realOccupiedGPs)
    {
        realOccupiedGPs = new List<GridPos>();
        orientation = GridPos.Orientation.Up;
        foreach (string s in Enum.GetNames(typeof(GridPos.Orientation)))
        {
            orientation = (GridPos.Orientation) Enum.Parse(typeof(GridPos.Orientation), s);
            if (FindSpaceToPutRotatedItem(mci, orientation, realOccupiedGPs)) return true;
        }

        return false;
    }

    private bool FindSpaceToPutRotatedItem(MechaComponentInfo mci, GridPos.Orientation orientation, List<GridPos> realOccupiedGPs)
    {
        List<GridPos> occupiedGridPositions = BagManager.Instance.MechaComponentOccupiedGridPosDict[mci.MechaComponentType];
        IntRect space = GetSizeFromGridPositions(occupiedGridPositions);

        bool heightWidthSwap = orientation == GridPos.Orientation.Right || orientation == GridPos.Orientation.Left;

        GridPos temp = new GridPos(space.x, space.z);
        GridPos temp_rot = GridPos.RotateGridPos(temp, orientation);
        int xStart_temp = temp_rot.x;
        int zStart_temp = temp_rot.z;

        for (int i = 0 - zStart_temp; i <= 10 - (heightWidthSwap ? space.width : space.height) - zStart_temp; i++)
        {
            for (int j = 0 - xStart_temp; j <= 10 - (heightWidthSwap ? space.height : space.width) - xStart_temp; j++)
            {
                bool canHold = true;
                foreach (GridPos gp in occupiedGridPositions)
                {
                    GridPos rot_gp = GridPos.RotateGridPos(gp, orientation);
                    int col = j + rot_gp.x;
                    int row = i + rot_gp.z;
                    if (col < 0 || col >= 10 || row < 0 || col >= 10)
                    {
                        canHold = false;
                        break;
                    }

                    if (!bagGridMatrix[col, row].Available)
                    {
                        canHold = false;
                        break;
                    }

                    realOccupiedGPs.Add(new GridPos(col, row, GridPos.Orientation.Up));
                }

                if (canHold)
                {
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

            if (!bagGridMatrix[gp.x, gp.z].Available)
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

            if (bagGridMatrix[gp.x + offset.x, gp.z + offset.z].Locked)
            {
                return false;
            }
        }

        return true;
    }

    private void AddItem(MechaComponentInfo mci, GridPos.Orientation orientation, List<GridPos> realOccupiedGPs, out BagItem bagItem)
    {
        IntRect realSpace = GetSizeFromGridPositions(realOccupiedGPs);
        GridPos GridPos = new GridPos(realSpace.x, realSpace.z, orientation);

        bagItem = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BagItem].AllocateGameObject<BagItem>(ItemContainer);
        bagItem.Initialize(mci, realSpace.width, realSpace.height, GridPos, realOccupiedGPs, false);
        bagItems.Add(bagItem);

        foreach (GridPos gp in realOccupiedGPs)
        {
            bagGridMatrix[gp.x, gp.z].State = BagGrid.States.Unavailable;
        }
    }

    public void RemoveItem(BagItem bagItem, bool temporary)
    {
        if (bagItems.Contains(bagItem))
        {
            foreach (GridPos gp in bagItem.RealPositionsInBagPanel_BeforeMove)
            {
                bagGridMatrix[gp.x, gp.z].State = temporary ? BagGrid.States.TempUnavailable : BagGrid.States.Available;
            }

            if (!temporary)
            {
                bagItems.Remove(bagItem);
                bagItem.PoolRecycle();
            }
        }
    }

    private IntRect GetSizeFromGridPositions(List<GridPos> occupiedGridPositions)
    {
        int X_min = 999;
        int X_max = -999;
        int Z_min = 999;
        int Z_max = -999;
        foreach (GridPos gp in occupiedGridPositions)
        {
            if (gp.x < X_min)
            {
                X_min = gp.x;
            }

            if (gp.x > X_max)
            {
                X_max = gp.x;
            }

            if (gp.z < Z_min)
            {
                Z_min = gp.z;
            }

            if (gp.z > Z_max)
            {
                Z_max = gp.z;
            }
        }

        IntRect res = new IntRect(X_min, Z_min, X_max - X_min + 1, Z_max - Z_min + 1);
        return res;
    }

    public void OnMouseEnterBag()
    {
        DragManager.Instance.IsMouseInsideBag = true;
    }

    public void OnMouseLeaveBag()
    {
        DragManager.Instance.IsMouseInsideBag = false;
    }
}