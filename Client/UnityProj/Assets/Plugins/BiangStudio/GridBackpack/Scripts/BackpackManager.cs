using System.Collections.Generic;
using BiangStudio.GamePlay.UI;
using BiangStudio.Singleton;
using UnityEngine;
using UnityEngine.Events;

namespace BiangStudio.GridBackpack
{
    public class BackpackManager : TSingletonBaseManager<BackpackManager>
    {
        public delegate bool KeyDownDelegate();

        public delegate void LogErrorDelegate(string log);

        public delegate MonoBehaviour InstantiatePrefabDelegate(Transform parent);

        private BackpackInfo backpackInfo;
        public BackpackInfo BackpackInfo => backpackInfo;
        public BackpackPanel BackpackPanel;

        private int backpackItemGridSize;
        public int BackpackItemGridSize => backpackItemGridSize;

        private KeyDownDelegate ToggleBackpackKeyDownHandler;
        internal KeyDownDelegate RotateItemKeyDownHandler;
        internal KeyDownDelegate ToggleDebugKeyDownHandler;

        private LogErrorDelegate LogErrorHandler;
        internal InstantiatePrefabDelegate InstantiateBackpackGridHandler;
        internal InstantiatePrefabDelegate InstantiateBackpackItemHandler;
        internal InstantiatePrefabDelegate InstantiateBackpackItemGridHitBoxHandler;

        private UnityAction<bool> ToggleBackpackCallback;
        internal UnityAction<BackpackItem> DragItemOutBackpackCallback;

        private Dictionary<string, Sprite> BackpackItemSpriteDict;

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
                        UIManager.Instance.ShowUIForms<BackpackPanel>();
                    }
                    else
                    {
                        BackpackPanel.CloseUIForm();
                    }

                    ToggleBackpackCallback?.Invoke(value);
                    isOpen = value;
                }
            }
        }

        /// <summary>
        /// Initialize the backpack manager.
        /// </summary>
        /// <param name="backpackItemGridSize">the size (in pixel) of each grid of backpack items in UI panels.</param>
        /// <param name="backpackItemSpriteDict">Key: backpack item name, Value: backpack item sprite</param>
        /// <param name="toggleBackpackKeyDownHandler">This handler should return a signal which toggles the backpack(e.g. return Input.GetKeyDown(KeyCode.B);)</param>
        /// <param name="rotateItemKeyDownHandler">This handler should return a signal which rotates the backpack item(e.g. return Input.GetKeyDown(KeyCode.R);)</param>
        /// <param name="toggleBackpackCallback">This callback will be execute when the backpack is opened or closed</param>
        /// <param name="toggleDebugKeyDownHandler">This callback will be execute when the backpack is opened or closed</param>
        /// <param name="dragItemOutBackpackCallback">This callback will be execute when the backpack item is dragged out of the backpack.</param>
        /// <param name="instantiateBackpackGridHandler">This handler should instantiate a prefab with BackpackGrid component.</param>
        /// <param name="instantiateBackpackItemHandler">This handler should instantiate a prefab with BackpackItem component.</param>
        /// <param name="instantiateBackpackItemGridHitBoxHandler">This handler should instantiate a prefab with BackpackItemGridHitBox component.</param>
        public void Init(
            int backpackItemGridSize,
            Dictionary<string, Sprite> backpackItemSpriteDict,
            KeyDownDelegate toggleBackpackKeyDownHandler,
            KeyDownDelegate rotateItemKeyDownHandler,
            KeyDownDelegate toggleDebugKeyDownHandler,
            UnityAction<bool> toggleBackpackCallback,
            UnityAction<BackpackItem> dragItemOutBackpackCallback,
            InstantiatePrefabDelegate instantiateBackpackGridHandler,
            InstantiatePrefabDelegate instantiateBackpackItemHandler,
            InstantiatePrefabDelegate instantiateBackpackItemGridHitBoxHandler
        )
        {
            this.backpackItemGridSize = backpackItemGridSize;
            BackpackItemSpriteDict = backpackItemSpriteDict;
            ToggleBackpackKeyDownHandler = toggleBackpackKeyDownHandler;
            RotateItemKeyDownHandler = rotateItemKeyDownHandler;
            ToggleDebugKeyDownHandler = toggleDebugKeyDownHandler;
            ToggleBackpackCallback = toggleBackpackCallback;
            DragItemOutBackpackCallback = dragItemOutBackpackCallback;
            InstantiateBackpackGridHandler = instantiateBackpackGridHandler;
            InstantiateBackpackItemHandler = instantiateBackpackItemHandler;
            InstantiateBackpackItemGridHitBoxHandler = instantiateBackpackItemGridHitBoxHandler;
        }

        public void LoadBackpackInfo(BackpackInfo backpackInfo)
        {
            this.backpackInfo = backpackInfo;
            BackpackPanel.Init(BackpackInfo);
            BackpackInfo.RefreshBackpackGrid();
        }

        public override void Awake()
        {
            BackpackPanel = UIManager.Instance.ShowUIForms<BackpackPanel>();
            BackpackPanel.CloseUIForm();
        }

        public override void Start()
        {
        }

        public override void Update()
        {
            if (ToggleBackpackKeyDownHandler != null && ToggleBackpackKeyDownHandler.Invoke())
            {
                IsOpen = !IsOpen;
            }
        }

        public Sprite GetBackpackItemSprite(string spriteName)
        {
            BackpackItemSpriteDict.TryGetValue(spriteName, out Sprite sprite);
            return sprite;
        }

        internal static void LogError(string log)
        {
            Instance.LogErrorHandler?.Invoke("[BiangStudio.GridBackpack] " + log);
        }
    }
}