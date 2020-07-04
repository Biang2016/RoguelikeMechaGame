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

        private BagInfo bagInfo;
        public BagInfo BagInfo => bagInfo;
        public BagPanel BagPanel;
        internal int BagItemGridSize;

        public delegate void LogErrorDelegate(string log);

        private UnityAction<bool> OnToggleBag;
        private KeyDownDelegate ToggleBagKeyDownHandler;
        internal KeyDownDelegate RotateItemKeyDownHandler;
        internal UnityAction<BagItem> DragItemOutBagAction;

        public delegate MonoBehaviour InstantiatePrefabDelegate(Transform parent);

        private LogErrorDelegate LogErrorHandler;
        internal InstantiatePrefabDelegate InstantiateBagGridHandler;
        internal InstantiatePrefabDelegate InstantiateBagItemHandler;
        internal InstantiatePrefabDelegate InstantiateBagItemGridHitBoxHandler;

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

                    OnToggleBag?.Invoke(value);
                    isOpen = value;
                }
            }
        }

        /// <summary>
        /// Initialize the bag manager.
        /// </summary>
        /// <param name="bagInfo"></param>
        /// <param name="onToggleBag">This callback will be execute when the bag is opened or closed</param>
        /// <param name="toggleBagKeyDownHandler">This handler should return a signal which toggles the bag(e.g. return Input.GetKeyDown(KeyCode.B);)</param>
        /// <param name="rotateItemKeyDownHandler">This handler should return a signal which rotates the bag item(e.g. return Input.GetKeyDown(KeyCode.R);)</param>
        /// <param name="dragItemOutBagAction">This callback will be execute when the bag item is dragged out of the bag.</param>
        public void Init(Dictionary<string, Sprite> bagItemSpriteDict, UnityAction<bool> onToggleBag,
            KeyDownDelegate toggleBagKeyDownHandler,
            KeyDownDelegate rotateItemKeyDownHandler,
            UnityAction<BagItem> dragItemOutBagAction,
            InstantiatePrefabDelegate instantiateBagGridHandler,
            InstantiatePrefabDelegate instantiateBagItemHandler,
            InstantiatePrefabDelegate instantiateBagItemGridHitBoxHandler
        )
        {
            BagItemSpriteDict = bagItemSpriteDict;
            OnToggleBag = onToggleBag;
            ToggleBagKeyDownHandler = toggleBagKeyDownHandler;
            RotateItemKeyDownHandler = rotateItemKeyDownHandler;
            DragItemOutBagAction = dragItemOutBagAction;
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