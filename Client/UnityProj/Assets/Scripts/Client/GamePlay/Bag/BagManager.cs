using System;
using UnityEngine;
using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay.UI;
using BiangStudio.Singleton;
using GameCore;

namespace Client
{
    public class BagManager : TSingletonBaseManager<BagManager>
    {
        public BagInfo BagInfo;
        internal BagPanel BagPanel;
        internal int BagItemGridSize;

        public Dictionary<string, Sprite> BagItemSpriteDict = new Dictionary<string, Sprite>();

        public override void Awake()
        {
            LoadAllBlockItemPics();
            BagPanel = UIManager.Instance.ShowUIForms<BagPanel>();
            BagPanel.CloseUIForm();
            BagInfo = new BagInfo();
            BagPanel.Init(BagInfo);
            BagInfo.BagGridNumber = 95;
            BagInfo.RefreshBagGrid();
        }

        public override void Start()
        {
        }

        public override void Update()
        {
            if (ControlManager.Instance.Building_ToggleBag.Down)
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
                MechaComponentType mcType = (MechaComponentType) Enum.Parse(typeof(MechaComponentType), s);
                BagItemInfo bii = new BagItemInfo(new MechaComponentInfo(mcType, new GridPosR(0, 0, GridPosR.Orientation.Up), 100, 0));
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
                BagItemSpriteDict.Add(typeof(MechaComponentType).FullName + "." + mcType, sprite);
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
                CameraManager.Instance.MainCameraFollow.FOV_Level = 1;
                BattleManager.Instance.PlayerMecha.GridShown = true;
                GameStateManager.Instance.SetState(GameState.Building);
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

                CameraManager.Instance.MainCameraFollow.SetTarget(BattleManager.Instance.PlayerMecha.transform);
                CameraManager.Instance.MainCameraFollow.FOV_Level = 2;
                GameStateManager.Instance.SetState(GameState.Fighting);
            }
        }
    }
}