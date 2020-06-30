using System;
using UnityEngine;
using System.Collections.Generic;
using GameCore;

namespace Client
{
    public class BagManager : MonoSingleton<BagManager>
    {
        public BagInfo BagInfo;

        internal BagPanel BagPanel;

        internal int BagItemGridSize;

        public bool InfiniteComponents
        {
            get { return BagInfo.InfiniteComponents; }
            set { BagInfo.InfiniteComponents = value; }
        }

        public Dictionary<MechaComponentType, Sprite> MechaComponentSpriteDict = new Dictionary<MechaComponentType, Sprite>();

        void Awake()
        {
        }

        void Start()
        {
            LoadAllBlockItemPics();
            BagPanel = UIManager.Instance.ShowUIForms<BagPanel>();
            BagPanel.CloseUIForm();
            BagInfo = new BagInfo(95);
            BagPanel.Init(BagInfo);
            BagInfo.RefreshBagGrid();
        }

        void Update()
        {
            if (Input.GetButtonDown("Bag"))
            {
                if (!BagPanel.gameObject.activeInHierarchy)
                {
                    OpenBag();
                }
                else
                {
                    CloseBag();
                }
            }
        }

        public void Initialize()
        {
            foreach (string s in Enum.GetNames(typeof(MechaComponentType)))
            {
                MechaComponentType mcType = (MechaComponentType)Enum.Parse(typeof(MechaComponentType), s);
                BagItemInfo bii = new BagItemInfo(new MechaComponentInfo(mcType, new GridPos(0, 0, GridPos.Orientation.Up), 100, 0));
                BagInfo.TryAddItem(bii);
            }
        }

        /// <summary>
        /// Load all 2D sprites of mecha components
        /// </summary>
        private void LoadAllBlockItemPics()
        {
            foreach (string s in Enum.GetNames(typeof(MechaComponentType)))
            {
                MechaComponentType mcType = (MechaComponentType) Enum.Parse(typeof(MechaComponentType), s);
                Sprite sprite = Resources.Load<Sprite>("BlockItemPics/" + s);
                MechaComponentSpriteDict.Add(mcType, sprite);
            }
        }

        public void OpenBag()
        {
            if (!BagPanel.gameObject.activeInHierarchy)
            {
                BattleManager.Instance.SetAllEnemyShown(false);
                UIManager.Instance.ShowUIForms<BagPanel>();
                BattleManager.Instance.PlayerMecha.MechaEditArea.Show();
                BattleManager.Instance.PlayerMecha.SlotLightsShown = true;
                GameManager.Instance.MainCameraFollow.FOW_Level = 1;
                BattleManager.Instance.PlayerMecha.GridShown = true;
                GameManager.Instance.SetState(GameState.Building);
            }
        }

        public void CloseBag()
        {
            if (BagPanel.gameObject.activeInHierarchy)
            {
                BattleManager.Instance.SetAllEnemyShown(true);
                BagPanel.CloseUIForm();
                BattleManager.Instance.PlayerMecha.MechaEditArea.Hide();
                BattleManager.Instance.PlayerMecha.SlotLightsShown = false;
                BattleManager.Instance.PlayerMecha.GridShown = false;
                BattleManager.Instance.PlayerMecha.RefreshMechaMatrix(out List<MechaComponentBase> conflictComponents, out List<MechaComponentBase> isolatedComponents);

                foreach (MechaComponentBase mcb in conflictComponents)
                {
                    BattleManager.Instance.PlayerMecha.RemoveMechaComponent(mcb);
                    mcb.PoolRecycle();
                }

                foreach (MechaComponentBase mcb in isolatedComponents)
                {
                    BattleManager.Instance.PlayerMecha.RemoveMechaComponent(mcb);
                    mcb.PoolRecycle();
                }

                GameManager.Instance.MainCameraFollow.SetTarget(BattleManager.Instance.PlayerMecha.transform);
                GameManager.Instance.MainCameraFollow.FOW_Level = 2;
                GameManager.Instance.SetState(GameState.Fighting);
            }
        }
    }
}