using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

namespace BiangStudio.GridBackpack
{
    public class BackpackItemGridHitBoxRoot : MonoBehaviour
    {
        [SerializeField] private Transform HitBoxContainer;
        private List<BackpackItemGridHitBox> backpackItemGridHitBoxes = new List<BackpackItemGridHitBox>();

        internal void Initialize(List<GridPos> gridPositions, GridPos centerGP)
        {
            foreach (BackpackItemGridHitBox b in backpackItemGridHitBoxes)
            {
                b.PoolRecycle();
            }

            backpackItemGridHitBoxes.Clear();

            foreach (GridPos gp in gridPositions)
            {
                GridPos localGP = gp - centerGP;

                BackpackItemGridHitBox hb = (BackpackItemGridHitBox) BackpackManager.Instance.InstantiateBackpackItemGridHitBoxHandler(HitBoxContainer);
                if (!hb)
                {
                    BackpackManager.LogError("Instantiate BackpackItemGridHitBox prefab failed.");
                }
                else
                {
                    hb.Initialize(localGP, new GridRect(localGP.x, -localGP.z, BackpackManager.Instance.BackpackItemGridSize, BackpackManager.Instance.BackpackItemGridSize));
                    backpackItemGridHitBoxes.Add(hb);
                }
            }
        }

        internal BackpackItemGridHitBox FindHitBox(Collider collider)
        {
            foreach (BackpackItemGridHitBox backpackItemGridHitBox in backpackItemGridHitBoxes)
            {
                if (backpackItemGridHitBox.BoxCollider == collider)
                {
                    return backpackItemGridHitBox;
                }
            }

            return null;
        }
    }
}