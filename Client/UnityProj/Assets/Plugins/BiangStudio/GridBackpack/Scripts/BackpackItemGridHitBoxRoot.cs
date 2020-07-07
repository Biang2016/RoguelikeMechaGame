using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ShapedInventory;
using UnityEngine;

namespace BiangStudio.GridBackpack
{
    public class BackpackItemGridHitBoxRoot : MonoBehaviour
    {
        public Backpack Backpack;

        [SerializeField] private Transform HitBoxContainer;
        private List<BackpackItemGridHitBox> backpackItemGridHitBoxes = new List<BackpackItemGridHitBox>();

        internal void Initialize(Backpack backpack, InventoryItem item)
        {
            Backpack = backpack;
            foreach (BackpackItemGridHitBox b in backpackItemGridHitBoxes)
            {
                b.PoolRecycle();
            }

            backpackItemGridHitBoxes.Clear();

            foreach (GridPos gp in item.OccupiedGridPositions_Matrix)
            {
                GridPos local = GridPos.GetLocalGridPosByCenter(item.GridPos_Matrix, gp);
                GridPos localGP = gp - item.BoundingRect.position;
                BackpackItemGridHitBox hb = Backpack.CreateBackpackItemGridHitBox(HitBoxContainer);
                hb.Initialize(local, new GridRect(localGP.x, -localGP.z, Backpack.GridSize, Backpack.GridSize));
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