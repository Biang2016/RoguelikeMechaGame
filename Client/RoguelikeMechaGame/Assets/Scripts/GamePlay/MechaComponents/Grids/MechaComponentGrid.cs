using UnityEngine;
using System.Collections;

public class MechaComponentGrid : MonoBehaviour, IGridPos
{
    public GridPos GetGridPos()
    {
        return GridPos.GetGridPosByLocalTrans(transform);
    }
}