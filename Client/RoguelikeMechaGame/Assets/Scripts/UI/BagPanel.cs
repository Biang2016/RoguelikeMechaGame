using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BagPanel : BaseUIForm
{
    [SerializeField] private GridLayoutGroup ItemContainerGridLayout;
    [SerializeField] private Transform GridContainer;
    [SerializeField] private Transform ItemContainer;

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

    public bool TryAddItem(MechaComponentInfo mci, bool autoSpacing)
    {
        bool placeFound = FindSpaceToPutItem(mci, autoSpacing, out GridPos.Orientation orientation, out List<GridPos> realOccupiedGPs);
        if (placeFound)
        {
            AddItem(mci, orientation, realOccupiedGPs);
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool FindSpaceToPutItem(MechaComponentInfo mci, bool allowRotate, out GridPos.Orientation orientation, out List<GridPos> realOccupiedGPs)
    {
        realOccupiedGPs = new List<GridPos>();
        if (allowRotate)
        {
            orientation = GridPos.Orientation.Up;
            foreach (string s in Enum.GetNames(typeof(GridPos.Orientation)))
            {
                orientation = (GridPos.Orientation) Enum.Parse(typeof(GridPos.Orientation), s);
                if (FindSpaceToPutRotatedItem(mci, orientation, realOccupiedGPs)) return true;
            }

            return false;
        }
        else
        {
            orientation = GridPos.Orientation.Up;
            return FindSpaceToPutRotatedItem(mci, orientation, realOccupiedGPs);
        }
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

    private void AddItem(MechaComponentInfo mci, GridPos.Orientation orientation, List<GridPos> realOccupiedGPs)
    {
        IntRect realSpace = GetSizeFromGridPositions(realOccupiedGPs);
        GridPos GridPos = new GridPos(realSpace.x, realSpace.z, orientation);

        BagItem bi = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BagItem].AllocateGameObject<BagItem>(ItemContainer);
        bi.Initialize(mci, realSpace.width, realSpace.height, GridPos, realOccupiedGPs);
        bagItems.Add(bi);

        foreach (GridPos gp in realOccupiedGPs)
        {
            bagGridMatrix[gp.x, gp.z].State = BagGrid.States.Unavailable;
        }
    }

    public void RemoveItem(BagItem bagItem)
    {
        foreach (GridPos gp in bagItem.RealPositionsInBagPanel)
        {
            bagGridMatrix[gp.x, gp.z].State = BagGrid.States.Available;
        }

        bagItems.Remove(bagItem);
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