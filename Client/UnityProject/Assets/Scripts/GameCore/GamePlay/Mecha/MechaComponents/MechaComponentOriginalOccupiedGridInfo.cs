using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;

namespace GameCore
{
    /// <summary>
    /// 机甲组件占位原始信息（均为局部坐标）
    /// </summary>
    public class MechaComponentOriginalOccupiedGridInfo : IClone<MechaComponentOriginalOccupiedGridInfo>
    {
        public List<GridPos> MechaComponentOccupiedGridPositionList = new List<GridPos>();
        public List<GridPos> MechaComponentOccupiedGridPositionList_Backpack = new List<GridPos>();
        public List<GridPos> MechaComponentAllSlotLocalPositionsList = new List<GridPos>();

        public MechaComponentOriginalOccupiedGridInfo Clone()
        {
            MechaComponentOriginalOccupiedGridInfo info = new MechaComponentOriginalOccupiedGridInfo();
            info.MechaComponentOccupiedGridPositionList = MechaComponentOccupiedGridPositionList.Clone();
            info.MechaComponentOccupiedGridPositionList_Backpack = MechaComponentOccupiedGridPositionList_Backpack.Clone();
            info.MechaComponentAllSlotLocalPositionsList = MechaComponentAllSlotLocalPositionsList.Clone();
            return info;
        }
    }
}