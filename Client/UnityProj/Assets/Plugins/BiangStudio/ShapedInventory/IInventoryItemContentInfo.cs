using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;

namespace BiangStudio.ShapedInventory
{
    public interface IInventoryItemContentInfo
    {
        List<GridPos> OriginalOccupiedGridPositions { get; }
        string ItemName { get; }
        string ItemSpriteKey { get; }
    }
}