using System.Collections.Generic;
using BiangStudio.GridBackpack;
using UnityEngine;

public class BackpackItemVirtualOccupationRoot : MonoBehaviour
{
    internal List<BackpackVirtualOccupationQuad> backpackVirtualOccupationQuads = new List<BackpackVirtualOccupationQuad>();

    internal void Clear()
    {
        foreach (BackpackVirtualOccupationQuad quad in backpackVirtualOccupationQuads)
        {
            quad.PoolRecycle();
        }
    }
 
}
