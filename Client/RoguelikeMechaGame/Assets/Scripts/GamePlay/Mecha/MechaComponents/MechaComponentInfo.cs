using System;
using System.Collections.Generic;
using System.Xml;

public class MechaComponentInfo
{
    public MechaComponentType M_MechaComponentType;
    public GridPos M_GridPos;
    public List<GridPos> OccupiedGridPositions = new List<GridPos>();

    public MechaComponentInfo(MechaComponentType mechaComponentType, GridPos gridPos)
    {
        M_MechaComponentType = mechaComponentType;
        M_GridPos = gridPos;
    }

    public void ExportToXML(XmlElement allBlocks_ele)
    {
        XmlDocument doc = allBlocks_ele.OwnerDocument;
        XmlElement blockNode = doc.CreateElement("Block");
        allBlocks_ele.AppendChild(blockNode);
        blockNode.SetAttribute("BlockType", M_MechaComponentType.ToString());

        foreach (GridPos gridPos in OccupiedGridPositions)
        {
            XmlElement gridPosNode = doc.CreateElement("GridPos");
            blockNode.AppendChild(gridPosNode);
            gridPosNode.SetAttribute("x", gridPos.x.ToString());
            gridPosNode.SetAttribute("y", gridPos.y.ToString());
            gridPosNode.SetAttribute("z", gridPos.z.ToString());
            gridPosNode.SetAttribute("orientation", gridPos.orientation.ToString());
        }
    }
}