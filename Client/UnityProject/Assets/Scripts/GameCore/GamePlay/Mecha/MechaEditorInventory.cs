using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ShapedInventory;

namespace GameCore
{
    public class MechaEditorInventory : Inventory
    {
        public MechaEditorInventory(
            string inventoryName,
            DragArea dragArea,
            int gridSize,
            int rows,
            int columns,
            bool x_Mirror,
            bool z_Mirror,
            bool unlockedPartialGrids,
            int unlockedGridCount,
            KeyDownDelegate rotateItemKeyDownHandler
        ) : base(inventoryName, dragArea, gridSize, rows, columns, x_Mirror, z_Mirror, unlockedPartialGrids, unlockedGridCount, rotateItemKeyDownHandler,
            (gridPos) => new GridPosR(gridPos.x + ConfigManager.EDIT_AREA_HALF_SIZE, gridPos.z + ConfigManager.EDIT_AREA_HALF_SIZE, gridPos.orientation),
            (gp_matrix) => new GridPosR(gp_matrix.x - ConfigManager.EDIT_AREA_HALF_SIZE, gp_matrix.z - ConfigManager.EDIT_AREA_HALF_SIZE, gp_matrix.orientation),
            (gridPos) => new GridPos(gridPos.x, gridPos.z),
            (gp_matrix) => new GridPos(gp_matrix.x, gp_matrix.z)
        )
        {
        }
    }
}