﻿using System.Collections.Generic;
using System;
using System.IO;
using BiangStudio;
using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay;
using BiangStudio.GridBackpack;
using BiangStudio.ShapedInventory;
using GameCore;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using DragAreaDefines = GameCore.DragAreaDefines;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Client
{
    [ExecuteInEditMode]
    public partial class MechaComponent : MechaComponentBase, IDraggable
    {
        [ReadOnly]
        [PropertyOrder(-10)]
        [HideInEditorMode]
        [LabelText("父Mecha")]
        public Mecha Mecha = null;

        [PropertyOrder(-10)]
        [Title("Data")]
        public MechaComponentInfo MechaComponentInfo;

        [TitleGroup("GameObjectContainers")]
        [PropertyOrder(-9)]
        [LabelText("Grid根节点")]
        public MechaComponentGridRoot MechaComponentGridRoot;

        [TitleGroup("GameObjectContainers")]
        [PropertyOrder(-9)]
        [LabelText("模型根节点")]
        public MechaComponentModelRoot MechaComponentModelRoot;

        internal Draggable Draggable;
        private bool isReturningToBackpack = false;
        internal UnityAction<MechaComponent> OnRemoveMechaComponentSuc;

        internal MechaInfo MechaInfo => Mecha.MechaInfo;
        internal Inventory Inventory => MechaComponentInfo.InventoryItem.Inventory;
        internal InventoryItem InventoryItem => MechaComponentInfo.InventoryItem;
        internal MechaCamp MechaCamp => Mecha ? Mecha.MechaInfo.MechaCamp : MechaCamp.None;

        public override void PoolRecycle()
        {
            UnregisterAbilityEvents();
            MechaComponentGridRoot.SetIsolatedIndicatorShown(true);
            MechaComponentGridRoot.SetInBattle(false);
            MechaComponentModelRoot.ResetColor();
            MechaComponentInfo?.Reset();
            MechaComponentInfo = null;
            Mecha = null;
            isReturningToBackpack = false;
            OnRemoveMechaComponentSuc = null;
            base.PoolRecycle();
        }

        void Awake()
        {
            Draggable = GetComponent<Draggable>();
            Awake_Fighting();
        }

        protected virtual void Update()
        {
            if (!Application.isPlaying) // 编辑器中不允许调整位置
            {
                if (!GetComponentInParent<MechaDesignerHelper>())
                {
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                }
            }
        }

        public bool IsAlive()
        {
            return !IsRecycled && MechaComponentInfo != null && !MechaComponentInfo.IsDead;
        }

        public static MechaComponent BaseInitialize(MechaComponentInfo mechaComponentInfo, Mecha parentMecha)
        {
            MechaComponent mc = GameObjectPoolManager.Instance.MechaComponentPoolDict[mechaComponentInfo.MechaComponentConfig.MechaComponentKey]
                .AllocateGameObject<MechaComponent>(parentMecha ? parentMecha.transform : null);
            mc.Initialize(mechaComponentInfo, parentMecha);
            return mc;
        }

        public static MechaComponent BaseInitialize_Editor(MechaComponentInfo mechaComponentInfo, Mecha parentMecha)
        {
            MechaComponent mc = GameObjectPoolManager.Instance.MechaComponentPoolDict[mechaComponentInfo.MechaComponentConfig.MechaComponentKey]
                .AllocateGameObject<MechaComponent>(parentMecha ? parentMecha.transform : null);
            return mc;
        }

        private void Initialize(MechaComponentInfo mechaComponentInfo, Mecha parentMecha)
        {
            SetLayer(LayerManager.Instance.GetLayerByMechaCamp(mechaComponentInfo.MechaInfo.MechaCamp));
            mechaComponentInfo.OnRemoveMechaComponentInfoSuc += (mci) =>
            {
                OnRemoveMechaComponentSuc?.Invoke(this);
                PoolRecycle();
            };

            mechaComponentInfo.OnHighLightColorChange += HighLightColorChange;
            HighLightColorChange(QualityManager.GetQuality(mechaComponentInfo.Quality).Color, mechaComponentInfo.CurrentPowerUpgradeData.HighLightColorIntensity);

            {
                mechaComponentInfo.InventoryItem.OnSetGridPosHandler = (gridPos_World) =>
                {
                    GridPosR.ApplyGridPosToLocalTrans(gridPos_World, transform, ConfigManager.GridSize);
                    MechaInfo.MechaEditorInventory.RefreshConflictAndIsolation();
                };
                mechaComponentInfo.InventoryItem.OnIsolatedHandler = (shown) =>
                {
                    if (shown)
                    {
                        MechaComponentModelRoot.SetBuildingBasicEmissionColor(CommonUtils.HTMLColorToColor("#E42835"));
                    }
                    else
                    {
                        MechaComponentModelRoot.ResetBuildingBasicEmissionColor();
                    }

                    MechaComponentGridRoot.SetIsolatedIndicatorShown(shown);
                };
                mechaComponentInfo.InventoryItem.OnConflictedHandler = MechaComponentGridRoot.SetGridConflicted;
                mechaComponentInfo.InventoryItem.OnResetConflictHandler = MechaComponentGridRoot.ResetAllGridConflict;
            }

            MechaComponentInfo = mechaComponentInfo;
            GridPos.ApplyGridPosToLocalTransXZ(MechaComponentInfo.InventoryItem.GridPos_World, transform, ConfigManager.GridSize);
            Mecha = parentMecha;
            MechaComponentGridRoot.SetInBattle(true);

            Initialize_Fighting();
        }

        private void SetLayer(int layer)
        {
            gameObject.layer = layer;
            MechaComponentGridRoot.SetLayer(layer);
        }

        private void HighLightColorChange(Color highLightColor, float intensity)
        {
            MechaComponentModelRoot.SetDefaultHighLightEmissionColor(highLightColor * intensity);
        }

#if UNITY_EDITOR

        [HideInPlayMode]
        [TitleGroup("Editor Params")]
        [LabelText("拍照角度")]
        public float ScreenShotAngle;

        [MenuItem("开发工具/配置/序列化模组占位")]
        public static void SerializeMechaComponentOccupiedPositions()
        {
            PrefabManager.Instance.LoadPrefabs();
            Dictionary<string, MechaComponentOriginalOccupiedGridInfo> dict = new Dictionary<string, MechaComponentOriginalOccupiedGridInfo>();
            List<MechaComponent> mcs = new List<MechaComponent>();
            ConfigManager.LoadAllConfigs();
            try
            {
                foreach (KeyValuePair<string, MechaComponentConfig> kv in ConfigManager.MechaComponentConfigDict)
                {
                    GameObject prefab = PrefabManager.Instance.GetPrefab(kv.Key);
                    if (prefab != null)
                    {
                        Debug.Log($"模组占位序列化成功: <color=\"#00ADFF\">{kv.Key}</color>");
                        MechaComponent mc = Instantiate(prefab).GetComponent<MechaComponent>();
                        mcs.Add(mc);
                        MechaComponentOriginalOccupiedGridInfo info = new MechaComponentOriginalOccupiedGridInfo();
                        info.MechaComponentOccupiedGridPositionList = mc.MechaComponentGridRoot.GetOccupiedPositions();
                        info.MechaComponentAllSlotLocalPositionsList = mc.MechaComponentGridRoot.GetAllSlotPositions_Local();
                        dict.Add(kv.Key, info);
                    }
                }

                if (!Directory.Exists(ConfigManager.MechaComponentOriginalOccupiedGridInfoJsonFileFolder))
                {
                    Directory.CreateDirectory(ConfigManager.MechaComponentOriginalOccupiedGridInfoJsonFileFolder);
                }

                string json = JsonConvert.SerializeObject(dict, Formatting.Indented);
                StreamWriter sw = new StreamWriter(ConfigManager.MechaComponentOriginalOccupiedGridInfoJsonFilePath);
                sw.Write(json);
                sw.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
            finally
            {
                foreach (MechaComponent mc in mcs)
                {
                    DestroyImmediate(mc.gameObject);
                }
            }
        }

#endif

        public void SetShown(bool shown)
        {
            MechaComponentModelRoot.SetShown(shown);
        }

        #region IDraggable

        private Vector3 dragStartLocalPos;

        public void Draggable_OnMouseDown(DragArea dragArea, Collider collider)
        {
            dragStartLocalPos = transform.localPosition;
        }

        public void Draggable_OnMousePressed(DragArea dragArea, Vector3 diffFromStart, Vector3 deltaFromLastFrame)
        {
            if (dragArea.Equals(DragAreaDefines.BattleInventory))
            {
                ReturnToBackpack(true, true);
                return;
            }

            if (dragArea.Equals(Inventory.DragArea))
            {
                if (Mecha && Mecha.MechaInfo.MechaCamp == MechaCamp.Player)
                {
                    if (Inventory.RotateItemKeyDownHandler != null && Inventory.RotateItemKeyDownHandler.Invoke())
                    {
                        MechaComponentInfo.InventoryItem.Rotate();
                    }

                    if (diffFromStart.magnitude <= Draggable_DragMinDistance)
                    {
                        //不动
                    }
                    else
                    {
                        Vector3 currentLocalPos = dragStartLocalPos + transform.parent.InverseTransformVector(diffFromStart);
                        GridPosR gp_world = GridPos.GetGridPosByPointXZ(currentLocalPos, Inventory.GridSize);
                        gp_world.orientation = InventoryItem.GridPos_Matrix.orientation;
                        GridPosR gp_matrix = Inventory.CoordinateTransformationHandler_FromPosToMatrixIndex(gp_world);
                        MechaComponentInfo.InventoryItem.SetGridPosition(gp_matrix);
                    }
                }
            }
        }

        public bool ReturnToBackpack(bool cancelDrag, bool dragTheItem)
        {
            Backpack bp = BackpackManager.Instance.GetBackPack(DragAreaDefines.BattleInventory.DragAreaName);
            bp.BackpackPanel.BackpackDragArea.GetMousePosOnThisArea(out Vector3 _, out Vector3 pos_local, out Vector3 pos_matrix, out GridPos gp_matrix);
            InventoryItem ii = new InventoryItem(MechaComponentInfo, bp, gp_matrix);
            ii.ItemContentInfo = MechaComponentInfo;
            bool suc = bp.TryAddItem(ii);
            if (suc)
            {
                if (cancelDrag)
                {
                    isReturningToBackpack = true;
                    DragManager.Instance.CurrentDrag = null;
                }

                if (dragTheItem)
                {
                    bp.ResetGrids(ii.OccupiedGridPositions_Matrix);
                    BackpackItem backpackItem = bp.BackpackPanel.GetBackpackItem(ii.GUID);
                    DragManager.Instance.CurrentDrag = backpackItem.gameObject.GetComponent<Draggable>();
                    backpackItem.RectTransform.anchoredPosition = pos_local;
                    ii.SetGridPosition(gp_matrix);
                    DragManager.Instance.CurrentDrag.SetOnDrag(true, null, DragManager.Instance.GetDragProcessor<BackpackItem>());
                }

                MechaComponentInfo.RemoveMechaComponentInfo();
                PoolRecycle();
            }

            return suc;
        }

        public void Draggable_OnMouseUp(DragArea dragArea, Vector3 diffFromStart, Vector3 deltaFromLastFrame)
        {
            if (IsRecycled) return;
            if (dragArea.Equals(DragAreaDefines.BattleInventory))
            {
                if (!isReturningToBackpack)
                {
                    bool suc = ReturnToBackpack(false, false);
                    if (!suc)
                    {
                        //DragManager.Instance.CurrentDrag.ResetToOriginalPositionRotation();
                    }
                }

                return;
            }

            if (dragArea.Equals(DragAreaDefines.MechaEditorArea))
            {
                return;
            }

            if (dragArea.Equals(BiangStudio.DragHover.DragAreaDefines.None))
            {
                bool suc = ReturnToBackpack(false, false);
                if (!suc)
                {
                    //DragManager.Instance.CurrentDrag.ResetToOriginalPositionRotation();
                }

                return;
            }
        }

        public void Draggable_SetStates(ref bool canDrag, ref DragArea dragFrom)
        {
            canDrag = true;
            dragFrom = DragAreaDefines.MechaEditorArea;
        }

        public float Draggable_DragMinDistance => 0f;

        public float Draggable_DragMaxDistance => 9999f;

        #endregion
    }
}