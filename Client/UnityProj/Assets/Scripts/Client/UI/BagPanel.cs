using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay.UI;
using GameCore;
using UnityEngine.UI;

namespace Client
{
    public class BagPanel : BaseUIForm
    {
        private BagInfo Data;

        [SerializeField]
        private GridLayoutGroup ItemContainerGridLayout;

        [SerializeField]
        private Transform GridContainer;

        public Transform ItemContainer;

        private BagGrid[,] bagGridMatrix = new BagGrid[10, 10]; // column, row
        private SortedDictionary<int, BagItem> bagItems = new SortedDictionary<int, BagItem>();

        void Awake()
        {
            UIType.InitUIType(
                isClearStack: false,
                isESCClose: true,
                isClickElsewhereClose: false,
                uiForms_Type: UIFormTypes.Normal,
                uiForms_ShowMode: UIFormShowModes.Normal,
                uiForm_LucencyType: UIFormLucencyTypes.Penetrable);

            BagManager.Instance.BagItemGridSize = Mathf.RoundToInt(ItemContainerGridLayout.cellSize.x);
        }

        void Update()
        {
        }

        void Reset()
        {
            foreach (KeyValuePair<int, BagItem> kv in bagItems)
            {
                kv.Value.PoolRecycle();
            }

            bagItems.Clear();
        }

        public void Init(BagInfo bagInfo)
        {
            Data = bagInfo;
            for (int z = 0; z < 10; z++)
            {
                for (int x = 0; x < 10; x++)
                {
                    BagGrid big = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BagGrid].AllocateGameObject<BagGrid>(GridContainer);
                    big.Init(bagInfo.BagGridMatrix[x, z], new GridPos(x, z));
                    bagGridMatrix[x, z] = big;
                }
            }

            Data.OnAddItemSucAction = OnAddItemSuc;
            Data.OnRemoveItemSucAction = OnRemoveItemSuc;
        }

        public BagItem GetBagItem(int guid)
        {
            bagItems.TryGetValue(guid, out BagItem bagItem);
            return bagItem;
        }

        private void OnAddItemSuc(BagItemInfo bii)
        {
            BagItem bagItem = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BagItem].AllocateGameObject<BagItem>(ItemContainer);
            bagItem.Initialize(bii);
            bagItems.Add(bii.GUID, bagItem);
        }

        private void OnRemoveItemSuc(BagItemInfo bii)
        {
            BagItem bi = bagItems[bii.GUID];
            bi.PoolRecycle();
            bagItems.Remove(bii.GUID);
        }

        public void OnMouseEnterBag()
        {
            DragManager.Instance.IsMouseInsideBag = true;
        }

        public void OnMouseLeaveBag()
        {
            DragManager.Instance.IsMouseInsideBag = false;
        }
    }
}