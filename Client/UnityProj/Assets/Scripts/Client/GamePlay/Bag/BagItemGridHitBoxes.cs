using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

namespace Client
{
    public class BagItemGridHitBoxes : MonoBehaviour
    {
        [SerializeField] private Transform HitBoxContainer;
        public List<BagItemGridHitBox> bagItemGridHitBoxes = new List<BagItemGridHitBox>();

        public void Initialize(List<GridPos> gridPositions, GridPos centerGP)
        {
            foreach (BagItemGridHitBox b in bagItemGridHitBoxes)
            {
                b.PoolRecycle();
            }

            bagItemGridHitBoxes.Clear();

            foreach (GridPos gp in gridPositions)
            {
                GridPos localGP = gp - centerGP;
                BagItemGridHitBox hb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BagItemGridHitBox].AllocateGameObject<BagItemGridHitBox>(HitBoxContainer);
                hb.Initialize(localGP, new GridRect(localGP.x, -localGP.z, BagManager.Instance.BagItemGridSize, BagManager.Instance.BagItemGridSize));
                bagItemGridHitBoxes.Add(hb);
            }
        }
    }
}