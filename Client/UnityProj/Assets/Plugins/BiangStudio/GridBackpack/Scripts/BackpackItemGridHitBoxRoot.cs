using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

namespace BiangStudio.GridBackpack
{
    public class BackpackItemGridHitBoxRoot : MonoBehaviour
    {
        public Backpack Backpack;

        [SerializeField] private Transform HitBoxContainer;
        private List<BackpackItemGridHitBox> backpackItemGridHitBoxes = new List<BackpackItemGridHitBox>();

        internal void Initialize(Backpack backpack, List<GridPos> gridPositions, GridPos centerGP)
        {
            Backpack = backpack;
            foreach (BackpackItemGridHitBox b in backpackItemGridHitBoxes)
            {
                b.PoolRecycle();
            }

            backpackItemGridHitBoxes.Clear();

            foreach (GridPos gp in gridPositions)
            {
                GridPos localGP = gp - centerGP;

                BackpackItemGridHitBox hb = Backpack.CreateBackpackItemGridHitBox(HitBoxContainer);
                hb.Initialize(localGP, new GridRect(localGP.x, -localGP.z, Backpack.GridSize, Backpack.GridSize));
                backpackItemGridHitBoxes.Add(hb);
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