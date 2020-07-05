using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay.UI;
using UnityEngine;
using UnityEngine.UI;

namespace BiangStudio.GridBackpack
{
    public class BackpackPanel : BaseUIPanel
    {
        private BackpackInfo Data;

        [SerializeField] private GridLayoutGroup ItemContainerGridLayout;

        [SerializeField] private Transform GridContainer;

        public Transform ItemContainer;

        private BackpackGrid[,] backpackGridMatrix = new BackpackGrid[10, 10]; // column, row
        private SortedDictionary<int, BackpackItem> backpackItems = new SortedDictionary<int, BackpackItem>();

        void Awake()
        {
            UIType.InitUIType(
                false,
                true,
                false,
                UIFormTypes.Normal,
                UIFormShowModes.Normal,
                UIFormLucencyTypes.Penetrable);
        }

        void Update()
        {
            ItemContainerGridLayout.cellSize = Vector2.one * BackpackManager.Instance.BackpackItemGridSize;
        }

        void Reset()
        {
            foreach (KeyValuePair<int, BackpackItem> kv in backpackItems)
            {
                kv.Value.PoolRecycle();
            }

            backpackItems.Clear();
        }

        public void Init(BackpackInfo backpackInfo)
        {
            Data = backpackInfo;
            for (int z = 0; z < 10; z++)
            {
                for (int x = 0; x < 10; x++)
                {
                    BackpackGrid bg = (BackpackGrid) BackpackManager.Instance.InstantiateBackpackGridHandler(GridContainer);
                    if (!bg)
                    {
                        BackpackManager.LogError("Instantiate BackpackGrid prefab failed.");
                    }
                    else
                    {
                        bg.Init(backpackInfo.BackpackGridMatrix[x, z], new GridPos(x, z));
                        backpackGridMatrix[x, z] = bg;
                    }
                }
            }

            Data.OnAddItemSucAction = OnAddItemSuc;
            Data.OnRemoveItemSucAction = OnRemoveItemSuc;
        }

        public BackpackItem GetBackpackItem(int guid)
        {
            backpackItems.TryGetValue(guid, out BackpackItem backpackItem);
            return backpackItem;
        }

        private void OnAddItemSuc(BackpackItemInfo bii)
        {
            BackpackItem backpackItem = (BackpackItem) BackpackManager.Instance.InstantiateBackpackItemHandler(ItemContainer);
            if (!backpackItem)
            {
                BackpackManager.LogError("Instantiate BackpackItem prefab failed.");
            }
            else
            {
                backpackItem.Initialize(bii);
                backpackItems.Add(bii.GUID, backpackItem);
            }
        }

        private void OnRemoveItemSuc(BackpackItemInfo bii)
        {
            BackpackItem bi = backpackItems[bii.GUID];
            bi.PoolRecycle();
            backpackItems.Remove(bii.GUID);
        }
    }
}