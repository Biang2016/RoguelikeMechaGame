using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

namespace BiangStudio.GridBag
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

                BagItemGridHitBox hb = (BagItemGridHitBox) BagManager.Instance.InstantiateBagItemGridHitBoxHandler(HitBoxContainer);
                if (!hb)
                {
                    BagManager.LogError("Instantiate BagItemGridHitBox prefab failed.");
                }
                else
                {
                    hb.Initialize(localGP, new GridRect(localGP.x, -localGP.z, BagManager.Instance.BagItemGridSize, BagManager.Instance.BagItemGridSize));
                    bagItemGridHitBoxes.Add(hb);
                }
            }
        }
    }
}