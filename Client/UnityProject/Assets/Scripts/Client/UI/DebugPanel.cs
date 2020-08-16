﻿using System;
using System.Collections.Generic;
using System.Reflection;
using BiangStudio;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay.UI;
using BiangStudio.GridBackpack;
using GameCore;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Client
{
    public class DebugPanel : BaseUIPanel
    {
        public Text fpsText;
        private float deltaTime;

        private Dictionary<string, DebugPanelButton> DebugButtonDictTree = new Dictionary<string, DebugPanelButton>();
        private List<DebugPanelColumn> DebugButtonColumns = new List<DebugPanelColumn>();
        public Transform ColumnContainer;

        void Awake()
        {
            UIType.InitUIType(
                false,
                false,
                false,
                UIFormTypes.Fixed,
                UIFormShowModes.Normal,
                UIFormLucencyTypes.Penetrable);
        }

        void Start()
        {
            IsButtonsShow = false;
            Type type = typeof(DebugPanel);
            foreach (MethodInfo m in type.GetMethods())
            {
                foreach (Attribute a in m.GetCustomAttributes(true))
                {
                    if (a is DebugButtonAttribute dba)
                    {
                        AddButton(dba.ButtonName, 0, DebugButtonDictTree, () => { m.Invoke(this, new object[] { }); });
                    }
                }
            }
        }

        void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            fpsText.text = Mathf.Ceil(fps).ToString();
        }

        private bool isButtonsShow = false;

        private bool IsButtonsShow
        {
            get { return isButtonsShow; }
            set
            {
                isButtonsShow = value;
                ColumnContainer.gameObject.SetActive(value);
            }
        }

        public void ToggleDebugPanel()
        {
            IsButtonsShow = !IsButtonsShow;
        }

        private void AddButton(string buttonName, int layerDepth, Dictionary<string, DebugPanelButton> currentTree, UnityAction action)
        {
            if (layerDepth >= DebugButtonColumns.Count)
            {
                DebugPanelColumn column = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.DebugPanelColumn].AllocateGameObject<DebugPanelColumn>(ColumnContainer);
                DebugButtonColumns.Add(column);
            }

            string[] paths = buttonName.Split('/');

            DebugPanelColumn currentColumn = DebugButtonColumns[layerDepth];
            if (!currentTree.ContainsKey(paths[0]))
            {
                DebugPanelButton btn = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.DebugPanelButton].AllocateGameObject<DebugPanelButton>(currentColumn.transform);
                btn.Initialize(paths[0], paths.Length == 1
                    ? action
                    : () =>
                    {
                        btn.IsOpen = !btn.IsOpen;
                        foreach (KeyValuePair<string, DebugPanelButton> kv in currentTree)
                        {
                            if (kv.Value != btn && btn.IsOpen)
                            {
                                kv.Value.IsOpen = false;
                            }
                        }
                    });
                btn.gameObject.SetActive(layerDepth == 0);
                currentTree.Add(paths[0], btn);
            }

            if (paths.Length > 1)
            {
                string remainingPath = buttonName.Replace(paths[0] + "/", "");
                AddButton(remainingPath, layerDepth + 1, currentTree[paths[0]].DebugButtonDictTree, action);
            }
        }

        [DebugButton("战斗/添加随机敌人")]
        public void AddEnemy()
        {
            MechaConfig mechaConfig = ConfigManager.Instance.GetRandomMechaConfig();
            MechaInfo mechaInfo = mechaConfig.GetMechaInfo(MechaType.Enemy);
            BattleManager.Instance.AddEnemyMecha(mechaInfo);
            ClientBattleManager.Instance.MechaDict[mechaInfo.GUID].transform.position = new Vector3(Random.Range(0,20), 0, Random.Range(0, 20));
        }

        [DebugButton("背包/增加5格")]
        public void AddBackpackGrid5()
        {
            BackpackManager.Instance.GetBackPack(DragAreaDefines.BattleInventory.DragAreaName).UnlockGrids(5);
        }

        [DebugButton("背包/增加10格")]
        public void AddBackpackGrid10()
        {
            BackpackManager.Instance.GetBackPack(DragAreaDefines.BattleInventory.DragAreaName).UnlockGrids(10);
        }
    }
}