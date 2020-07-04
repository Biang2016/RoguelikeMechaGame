using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;

namespace BiangStudio.GridBag
{
    public interface IBagItemContentInfo
    {
        List<GridPos> OriginalOccupiedGridPositions { get; }

        string BagItemSpriteKey { get; }
        string BagItemName { get; }
    }
}