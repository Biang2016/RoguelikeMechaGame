using System.Collections.Generic;

public class MechaComponentInfo
{
    public MechaComponentType MechaComponentType;
    public GridPos GridPos;
    internal List<GridPos> OccupiedGridPositions = new List<GridPos>();
    public int DropProbability;
    public int TotalLife;

    public MechaComponentInfo(MechaComponentType mechaComponentType, GridPos gridPos, int totalLife, int dropProbability)
    {
        MechaComponentType = mechaComponentType;
        GridPos = gridPos;
        TotalLife = totalLife;
        DropProbability = dropProbability;
    }

    public MechaComponentInfo Clone()
    {
        MechaComponentInfo mci = new MechaComponentInfo(MechaComponentType, GridPos, TotalLife, DropProbability);
        mci.OccupiedGridPositions = CloneVariantUtils.List(OccupiedGridPositions);
        return mci;
    }
}