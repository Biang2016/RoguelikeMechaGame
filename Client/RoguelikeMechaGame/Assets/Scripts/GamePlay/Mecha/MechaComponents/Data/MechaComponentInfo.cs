using System.Collections.Generic;

public class MechaComponentInfo
{
    public MechaComponentType MechaComponentType;
    public GridPos GridPos;
    internal List<GridPos> OccupiedGridPositions = new List<GridPos>();
    public int DropProbability = 10;

    public MechaComponentInfo(MechaComponentType mechaComponentType, GridPos gridPos, int dropProbability = 0)
    {
        MechaComponentType = mechaComponentType;
        GridPos = gridPos;
        DropProbability = dropProbability;
    }

    public MechaComponentInfo Clone()
    {
        MechaComponentInfo mci = new MechaComponentInfo(MechaComponentType, GridPos, DropProbability);
        mci.OccupiedGridPositions = CloneVariantUtils.List(OccupiedGridPositions);
        return mci;
    }
}