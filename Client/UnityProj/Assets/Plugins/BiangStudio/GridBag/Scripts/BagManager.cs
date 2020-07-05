using System;
using UnityEngine;
using System.Collections.Generic;
using BiangStudio.GamePlay.UI;
using BiangStudio.Singleton;
using UnityEngine.Events;

namespace BiangStudio.GridBag
{
    public class BagManager : TSingletonBaseManager<BagManager>
    {
        public delegate bool KeyDownDelegate();

        public delegate void LogErrorDelegate(string log);

        public delegate MonoBehaviour InstantiatePrefabDelegate(Transform parent);

        private BagInfo bagInfo;
        public BagInfo BagInfo => bagInfo;
        public BagPanel BagPanel;

        private int bagItemGridSize;
        public int BagItemGridSize => bagItemGridSize;

        private KeyDownDelegate ToggleBagKeyDownHandler;
        internal KeyDownDelegate RotateItemKeyDownHandler;

        private LogErrorDelegate LogErrorHandler;
        internal InstantiatePrefabDelegate InstantiateBagGridHandler;
        internal InstantiatePrefabDelegate InstantiateBagItemHandler;
        internal InstantiatePrefabDelegate InstantiateBagItemGridHitBoxHandler;

        private UnityAction<bool> ToggleBagCallback;
        internal UnityAction<BagItem> DragItemOutBagCallback;

        private Dictionary<string, Sprite> BagItemSpriteDict;

        private bool isOpen = false;

        private bool IsOpen
        {
            get { return isOpen; }

            set
            {
                if (isOpen != value)
                {
                    if (value)
                    {
                        UIManager.Instance.ShowUIForms<BagPanel>();
                    }
                    else
                    {
                        BagPanel.CloseUIForm();
                    }

                    ToggleBagCallback?.Invoke(value);
                    isOpen = value;
                }
            }
        }

        /// <summary>
        /// Initialize the bag manager.
        /// </summary>
        /// <param name="bagItemGridSize">the size (in pixel) of each grid of bag items in UI panels.</param>
        /// <param name="bagItemSpriteDict">Key: bag item name, Value: bag item sprite</param>
        /// <param name="toggleBagKeyDownHandler">This handler should return a signal which toggles the bag(e.g. return Input.GetKeyDown(KeyCode.B);)</param>
        /// <param name="rotateItemKeyDownHandler">This handler should return a signal which rotates the bag item(e.g. return Input.GetKeyDown(KeyCode.R);)</param>
        /// <param name="toggleBagCallback">This callback will be execute when the bag is opened or closed</param>
        /// <param name="dragItemOutBagCallback">This callback will be execute when the bag item is dragged out of the bag.</param>
        /// <param name="instantiateBagGridHandler">This handler should instantiate a prefab with BagGrid component.</param>
        /// <param name="instantiateBagItemHandler">This handler should instantiate a prefab with BagItem component.</param>
        /// <param name="instantiateBagItemGridHitBoxHandler">This handler should instantiate a prefab with BagItemGridHitBox component.</param>
        public void Init(
            int bagItemGridSize,
            Dictionary<string, Sprite> bagItemSpriteDict,
            KeyDownDelegate toggleBagKeyDownHandler,
            KeyDownDelegate rotateItemKeyDownHandler,
            UnityAction<bool> toggleBagCallback,
            UnityAction<BagItem> dragItemOutBagCallback,
            InstantiatePrefabDelegate instantiateBagGridHandler,
            InstantiatePrefabDelegate instantiateBagItemHandler,
            InstantiatePrefabDelegate instantiateBagItemGridHitBoxHandler
        )
        {
            this.bagItemGridSize = bagItemGridSize;
            BagItemSpriteDict = bagItemSpriteDict;
            ToggleBagKeyDownHandler = toggleBagKeyDownHandler;
            RotateItemKeyDownHandler = rotateItemKeyDownHandler;
            ToggleBagCallback = toggleBagCallback;
            DragItemOutBagCallback = dragItemOutBagCallback;
            InstantiateBagGridHandler = instantiateBagGridHandler;
            InstantiateBagItemHandler = instantiateBagItemHandler;
            InstantiateBagItemGridHitBoxHandler = instantiateBagItemGridHitBoxHandler;
        }

        public void LoadBagInfo(BagInfo bagInfo)
        {
            this.bagInfo = bagInfo;
            BagPanel.Init(BagInfo);
            BagInfo.RefreshBagGrid();
        }

        public override void Awake()
        {
            BagPanel = UIManager.Instance.ShowUIForms<BagPanel>();
            BagPanel.CloseUIForm();
        }

        public override void Start()
        {
        }

        public override void Update()
        {
            if (ToggleBagKeyDownHandler != null && ToggleBagKeyDownHandler.Invoke())
            {
                IsOpen = !IsOpen;
            }
        }

        public Sprite GetBagItemSprite(string spriteName)
        {
            BagItemSpriteDict.TryGetValue(spriteName, out Sprite sprite);
            return sprite;
        }

        internal static void LogError(string log)
        {
            Instance.LogErrorHandler?.Invoke("[BiangStudio.GridBag] " + log);
        }
    }
}