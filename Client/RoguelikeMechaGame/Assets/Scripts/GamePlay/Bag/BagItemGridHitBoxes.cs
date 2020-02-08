using System.Collections.Generic;
using UnityEngine;

public class BagItemGridHitBoxes : MonoBehaviour
{
    [SerializeField] private Transform HitBoxContainer;
    private List<BagItemGridHitBox> bagItemGridHitBoxes = new List<BagItemGridHitBox>();

    public void Initialize(List<GridPos> gridPositions, GridPos centerGP)
    {
        foreach (BagItemGridHitBox b in bagItemGridHitBoxes)
        {
            b.PoolRecycle();
        }

        bagItemGridHitBoxes.Clear();

        foreach (GridPos gp in gridPositions)
        {
            GridPos localGP = new GridPos(gp.x - centerGP.x, gp.z - centerGP.z);
            BagItemGridHitBox hb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BagItemGridHitBox].AllocateGameObject<BagItemGridHitBox>(HitBoxContainer);
            hb.Initialize(new IntRect(localGP.x, -localGP.z, BagManager.Instance.BagItemGridSize, BagManager.Instance.BagItemGridSize));
            bagItemGridHitBoxes.Add(hb);
        }
    }
}