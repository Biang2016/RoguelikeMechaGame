using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ShapedInventory;

namespace GameCore
{
    public class MechaEditorContainer : Inventory
    {
        public MechaEditorContainer(
            string inventoryName,
            DragArea dragArea,
            int gridSize,
            int rows,
            int columns,
            bool unlockedPartialGrids,
            int unlockedGridCount,
            KeyDownDelegate rotateItemKeyDownHandler
        ) : base(inventoryName, dragArea, gridSize, rows, columns, unlockedPartialGrids, unlockedGridCount, rotateItemKeyDownHandler,
            (gridPos) => new GridPos(gridPos.z + ConfigManager.EDIT_AREA_HALF_SIZE, gridPos.x + ConfigManager.EDIT_AREA_HALF_SIZE),
            (gp_matrix) => new GridPos(gp_matrix.z - ConfigManager.EDIT_AREA_HALF_SIZE, gp_matrix.x - ConfigManager.EDIT_AREA_HALF_SIZE)
        )
        {
        }
    }
}