using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;

namespace BiangStudio.GridBackpack
{
    public interface IBackpackItemContentInfo
    {
        List<GridPos> OriginalOccupiedGridPositions { get; }

        string BackpackItemSpriteKey { get; }
        string BackpackItemName { get; }
    }
}