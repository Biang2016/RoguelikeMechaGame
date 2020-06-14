using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameCore;
using UnityEngine.UI;

namespace Client
{
    public class BagPanel : BaseUIForm
    {
        private BagInfo Data;

        [SerializeField] private GridLayoutGroup ItemContainerGridLayout;
        [SerializeField] private Transform GridContainer;
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
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    BagGrid big = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BagGrid].AllocateGameObject<BagGrid>(GridContainer);
                    big.Init(bagInfo.BagGridMatrix[j, i]);
                    bagGridMatrix[j, i] = big;
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
            bagItem.Initialize(bii, false);
            bagItems.Add(bii.GUID, bagItem);
        }

        private void OnRemoveItemSuc(BagItemInfo bii)
        {
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