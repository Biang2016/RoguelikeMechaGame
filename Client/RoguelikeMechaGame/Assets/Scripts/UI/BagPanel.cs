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

    private BagGrid[,] BagGridMatrix = new BagGrid[10, 10]; // column, row
    private List<BagItem> BagItems = new List<BagItem>();

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
                big.Available = true;
                big.Locked = true;
                BagGridMatrix[j, i] = big;
            }
        }
    }

    void Update()
    {
    }

    void Reset()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                BagGridMatrix[j, i].Available = true;
                BagGridMatrix[j, i].Locked = true;
            }
        }

        foreach (BagItem bi in BagItems)
        {
            bi.PoolRecycle();
        }

        BagItems.Clear();
    }

    public void UnlockBagGridTo(int gridNumber)
    {
        int count = 0;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                count++;
                BagGridMatrix[j, i].Locked = count > gridNumber;
            }
        }
    }

    public bool AddItem(MechaComponentInfo mci, List<GridPos> occupiedGridPositions)
    {
        GetSizeFromGridPositions(occupiedGridPositions, out int width, out int height, out int xStart, out int zStart);
        bool placeFound = FindSpaceToPutItem(occupiedGridPositions, width, height, xStart, zStart, out GridPos.Orientation orientation, out List<GridPos> realOccupiedGPs);
        if (placeFound)
        {
            GetSizeFromGridPositions(realOccupiedGPs, out int _width, out int _height, out int _xStart, out int _zStart);
            GridPos GridPos = new GridPos(_xStart, _zStart, orientation);
            BagItem bi = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BagItem].AllocateGameObject<BagItem>(ItemContainer);
            bi.Initialize(mci, width, height, GridPos, realOccupiedGPs);
            BagItems.Add(bi);

            foreach (GridPos gp in realOccupiedGPs)
            {
                BagGridMatrix[gp.x, gp.z].Available = false;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public void RemoveItem(BagItem bagItem)
    {
        foreach (GridPos gp in bagItem.RealPosInBagPanel)
        {
            BagGridMatrix[gp.x, gp.z].Available = true;
        }

        BagItems.Remove(bagItem);
    }

    private bool FindSpaceToPutItem(List<GridPos> occupiedGridPositions, int width, int height, int xStart, int zStart, out GridPos.Orientation orientation, out List<GridPos> realOccupiedGPs)
    {
        realOccupiedGPs = new List<GridPos>();
        orientation = GridPos.Orientation.Up;
        foreach (string s in Enum.GetNames(typeof(GridPos.Orientation)))
        {
            orientation = (GridPos.Orientation) Enum.Parse(typeof(GridPos.Orientation), s);
            bool heightWidthSwap = orientation == GridPos.Orientation.Right || orientation == GridPos.Orientation.Left;

            GridPos temp = new GridPos(xStart, zStart);
            GridPos temp_rot = GridPos.RotateGridPos(temp, orientation);
            int xStart_temp = temp_rot.x;
            int zStart_temp = temp_rot.z;

            for (int i = 0 - zStart_temp; i <= 10 - (heightWidthSwap ? width : height) - zStart_temp; i++)
            {
                for (int j = 0 - xStart_temp; j <= 10 - (heightWidthSwap ? height : width) - xStart_temp; j++)
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

                        if (!BagGridMatrix[col, row].Available)
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
        }

        return false;
    }

    private void GetSizeFromGridPositions(List<GridPos> occupiedGridPositions, out int width, out int height, out int xStart, out int zStart)
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

        width = X_max - X_min + 1;
        height = Z_max - Z_min + 1;
        xStart = X_min;
        zStart = Z_min;
    }

    public void OnMouseEnterBag()
    {
        BagManager.Instance.IsMouseInsideBag = true;
    }

    public void OnMouseLeaveBag()
    {
        BagManager.Instance.IsMouseInsideBag = false;
    }
}