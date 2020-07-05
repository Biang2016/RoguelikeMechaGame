using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ShapedInventory;
using UnityEngine;
using UnityEngine.UI;

namespace BiangStudio.GridBackpack
{
    public class BackpackPanel : MonoBehaviour
    {
        private Backpack Backpack;

        [SerializeField] private GridLayoutGroup ItemContainerGridLayout;
        [SerializeField] private Transform GridContainer;

        public Transform ItemContainer;

        private BackpackGrid[,] backpackGridMatrix; // column, row
        private SortedDictionary<int, BackpackItem> backpackItems = new SortedDictionary<int, BackpackItem>();

        void Awake()
        {
            //UIType.InitUIType(
            //    false,
            //    true,
            //    false,
            //    UIFormTypes.Normal,
            //    UIFormShowModes.Normal,
            //    UIFormLucencyTypes.Penetrable);
        }

        void Update()
        {
        }

        void Reset()
        {
            foreach (KeyValuePair<int, BackpackItem> kv in backpackItems)
            {
                kv.Value.PoolRecycle();
            }

            backpackItems.Clear();
        }

        public void Init(Backpack backPack)
        {
            Backpack = backPack;
            Backpack.BackpackPanel = this;
            backpackGridMatrix = new BackpackGrid[Backpack.Columns, Backpack.Rows];
            for (int row = 0; row < Backpack.Rows; row++)
            {
                for (int col = 0; col < Backpack.Columns; col++)
                {
                    BackpackGrid bg = Backpack.CreateBackpackGrid(GridContainer);
                    bg.Init(Backpack.InventoryGridMatrix[col, row], new GridPos(col, row));
                    backpackGridMatrix[col, row] = bg;
                }
            }

            Backpack.OnAddItemSucAction = OnAddItemSuc;
            Backpack.OnRemoveItemSucAction = OnRemoveItemSuc;
            backPack.RefreshInventoryGrids();
        }

        public BackpackItem GetBackpackItem(int guid)
        {
            backpackItems.TryGetValue(guid, out BackpackItem backpackItem);
            return backpackItem;
        }

        private void OnAddItemSuc(InventoryItem ii)
        {
            BackpackItem backpackItem = Backpack.CreateBackpackItem(ItemContainer);
            backpackItem.Initialize(Backpack, ii);
            backpackItems.Add(ii.GUID, backpackItem);
        }

        private void OnRemoveItemSuc(InventoryItem ii)
        {
            BackpackItem bi = backpackItems[ii.GUID];
            bi.PoolRecycle();
            backpackItems.Remove(ii.GUID);
        }
    }
}