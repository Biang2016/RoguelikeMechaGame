using System;
using System.Collections.Generic;
using System.Xml;

public class MechaComponentInfo
{
    public MechaComponentType MechaComponentType;
    public GridPos GridPos;
    internal List<GridPos> OccupiedGridPositions = new List<GridPos>();

    public MechaComponentInfo(MechaComponentType mechaComponentType, GridPos gridPos)
    {
        MechaComponentType = mechaComponentType;
        GridPos = gridPos;
    }

    public MechaComponentInfo Clone()
    {
        MechaComponentInfo mci = new MechaComponentInfo(MechaComponentType, GridPos);
        mci.OccupiedGridPositions = CloneVariantUtils.List(OccupiedGridPositions);
        return mci;
    }
}