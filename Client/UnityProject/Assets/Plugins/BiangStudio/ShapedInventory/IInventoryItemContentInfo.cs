using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

namespace BiangStudio.ShapedInventory
{
    public interface IInventoryItemContentInfo
    {
        List<GridPos> OriginalOccupiedGridPositions { get; }
        string ItemName { get; }
        string ItemSpriteKey { get; }
        Color ItemColor { get; }
    }
}