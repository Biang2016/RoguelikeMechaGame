using System.Collections.Generic;

namespace GameCore
{
    public interface IBagItemContentInfo
    {
        List<GridPos> OriginalOccupiedGridPositions { get; }

        string BagItemSpriteKey { get; }
        string BagItemName { get; }
    }
}