using System;
using BiangStudio.DragHover;
using BiangStudio.GridBackpack;
using BiangStudio.ShapedInventory;
using UnityEngine;
using UnityEngine.Events;

public class Backpack : Inventory
{
    public BackpackPanel BackpackPanel;

    private KeyDownDelegate ToggleBackpackKeyDownHandler;

    private InstantiatePrefabDelegate InstantiateBackpackGridHandler;
    private InstantiatePrefabDelegate InstantiateBackpackItemHandler;
    private InstantiatePrefabDelegate InstantiateBackpackItemGridHitBoxHandler;

    /// <summary>
    /// This callback will be execute when the backpack is opened or closed
    /// </summary>
    public UnityAction<bool> ToggleBackpackCallback;

    /// <summary>
    /// This callback will be execute when the backpack debug mode is enable or disable
    /// </summary>
    public UnityAction<bool> ToggleDebugCallback;

    /// <summary>
    /// This callback will be execute when the backpack item is dragged out of the backpack.
    /// </summary>
    public UnityAction<BackpackItem> DragItemOutBackpackCallback;

    private bool isOpen = false;

    private bool IsOpen
    {
        get { return isOpen; }

        set
        {
            if (isOpen != value)
            {
                ToggleBackpackCallback?.Invoke(value);
                isOpen = value;
            }
        }
    }

    private bool isDebug = false;

    private bool IsDebug
    {
        get { return isDebug; }

        set
        {
            if (isDebug != value)
            {
                ToggleDebugCallback?.Invoke(value);
                isDebug = value;
            }
        }
    }

    /// <summary>
    /// Initialize the backpack manager.
    /// </summary>
    /// <param name="inventoryName">the name of the backpack</param>
    /// <param name="gridSize">the size (in pixel) of each grid of backpack items in UI panels.</param>
    /// <param name="toggleBackpackKeyDownHandler">This handler should return a signal which toggles the backpack(e.g. return Input.GetKeyDown(KeyCode.B);)</param>
    /// <param name="rotateItemKeyDownHandler">This handler should return a signal which rotates the backpack item(e.g. return Input.GetKeyDown(KeyCode.R);)</param>
    /// <param name="instantiateBackpackGridHandler">This handler should instantiate a prefab with BackpackGrid component.</param>
    /// <param name="instantiateBackpackItemHandler">This handler should instantiate a prefab with BackpackItem component.</param>
    /// <param name="instantiateBackpackItemGridHitBoxHandler">This handler should instantiate a prefab with BackpackItemGridHitBox component.</param>
    public Backpack(
        string inventoryName, DragArea dragArea, int gridSize, int rows, int columns, bool unlockPartialGrids, int unlockedGridCount,
        KeyDownDelegate toggleBackpackKeyDownHandler,
        KeyDownDelegate rotateItemKeyDownHandler,
        InstantiatePrefabDelegate instantiateBackpackGridHandler,
        InstantiatePrefabDelegate instantiateBackpackItemHandler,
        InstantiatePrefabDelegate instantiateBackpackItemGridHitBoxHandler
    ) : base(inventoryName, dragArea, gridSize, rows, columns, unlockPartialGrids, unlockedGridCount, rotateItemKeyDownHandler,
        (gridPos) => gridPos,
        (gridPos) => gridPos,
        (gridPos) => gridPos,
        (gridPos) => gridPos)
    {
        ToggleBackpackKeyDownHandler = toggleBackpackKeyDownHandler;
        InstantiateBackpackGridHandler = instantiateBackpackGridHandler;
        InstantiateBackpackItemHandler = instantiateBackpackItemHandler;
        InstantiateBackpackItemGridHitBoxHandler = instantiateBackpackItemGridHitBoxHandler;
    }

    public void Update()
    {
        if (ToggleBackpackKeyDownHandler != null && ToggleBackpackKeyDownHandler.Invoke())
        {
            IsOpen = !IsOpen;
        }

        if (ToggleDebugKeyDownHandler != null && ToggleDebugKeyDownHandler.Invoke())
        {
            IsDebug = !IsDebug;
        }
    }

    public BackpackGrid CreateBackpackGrid(Transform transform)
    {
        if (InstantiateBackpackGridHandler != null)
        {
            MonoBehaviour mono = InstantiateBackpackGridHandler?.Invoke(transform);
            if (mono != null)
            {
                try
                {
                    BackpackGrid res = (BackpackGrid) mono;
                    return res;
                }
                catch (Exception e)
                {
                    LogError(e.ToString());
                }
            }
        }

        return null;
    }

    public BackpackItem CreateBackpackItem(Transform transform)
    {
        if (InstantiateBackpackItemHandler != null)
        {
            MonoBehaviour mono = InstantiateBackpackItemHandler?.Invoke(transform);
            if (mono != null)
            {
                try
                {
                    BackpackItem res = (BackpackItem) mono;
                    return res;
                }
                catch (Exception e)
                {
                    LogError(e.ToString());
                }
            }
        }

        return null;
    }

    public BackpackItemGridHitBox CreateBackpackItemGridHitBox(Transform transform)
    {
        if (InstantiateBackpackItemGridHitBoxHandler != null)
        {
            MonoBehaviour mono = InstantiateBackpackItemGridHitBoxHandler?.Invoke(transform);
            if (mono != null)
            {
                try
                {
                    BackpackItemGridHitBox res = (BackpackItemGridHitBox) mono;
                    return res;
                }
                catch (Exception e)
                {
                    LogError(e.ToString());
                }
            }
        }

        return null;
    }
}