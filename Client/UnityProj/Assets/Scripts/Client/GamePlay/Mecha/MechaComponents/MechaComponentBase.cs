﻿using System;
using System.Collections.Generic;
using System.IO;
using BiangStudio.CloneVariant;
using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay;
using BiangStudio.GridBackpack;
using BiangStudio.ObjectPool;
using BiangStudio.ShapedInventory;
using GameCore;
using GameCore.AbilityDataDriven;
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
    public abstract partial class MechaComponentBase : PoolObject, IDraggable
    {
        [ReadOnly]
        [PropertyOrder(-10)]
        [HideInEditorMode]
        public Mecha Mecha = null;

        [PropertyOrder(10)]
        [Title("Data")]
        public MechaComponentInfo MechaComponentInfo;

        [TitleGroup("GameObjectContainers")]
        [PropertyOrder(-9)]
        public MechaComponentGridRoot MechaComponentGridRoot;

        [TitleGroup("GameObjectContainers")]
        [PropertyOrder(-9)]
        public GameObject ModelRoot;

        internal Draggable Draggable;
        private bool isReturningToBackpack = false;
        internal UnityAction<MechaComponentBase> OnRemoveMechaComponentBaseSuc;

        internal MechaInfo MechaInfo => Mecha.MechaInfo;
        internal Inventory Inventory => MechaComponentInfo.InventoryItem.Inventory;
        internal InventoryItem InventoryItem => MechaComponentInfo.InventoryItem;
        internal MechaType MechaType => Mecha ? Mecha.MechaInfo.MechaType : MechaType.None;

        public override void PoolRecycle()
        {
            MechaComponentGridRoot.SetIsolatedIndicatorShown(true);
            MechaComponentGridRoot.SetInBattle(false);
            MechaComponentInfo = null;
            Mecha = null;
            isReturningToBackpack = false;
            OnRemoveMechaComponentBaseSuc = null;
            base.PoolRecycle();
        }

        void Awake()
        {
            Draggable = GetComponent<Draggable>();
        }

        protected virtual void Update()
        {
            if (!IsRecycled)
            {
                if (!Application.isPlaying)
                {
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                }
                else
                {
                    Update_Fighting();
                }
            }
        }

        public static MechaComponentBase BaseInitialize(MechaComponentInfo mechaComponentInfo, Mecha parentMecha)
        {
            MechaComponentBase mcb = GameObjectPoolManager.Instance.MechaComponentPoolDict[mechaComponentInfo.MechaComponentType]
                .AllocateGameObject<MechaComponentBase>(parentMecha ? parentMecha.transform : null);
            mcb.Initialize(mechaComponentInfo, parentMecha);
            return mcb;
        }

        private void Initialize(MechaComponentInfo mechaComponentInfo, Mecha parentMecha)
        {
            mechaComponentInfo.OnDied = PoolRecycle;
            mechaComponentInfo.OnRemoveMechaComponentInfoSuc += (mci) =>
            {
                OnRemoveMechaComponentBaseSuc?.Invoke(this);
                PoolRecycle();
            };

            {
                mechaComponentInfo.InventoryItem.OnSetGridPosHandler = (gridPos_World) =>
                {
                    GridPosR.ApplyGridPosToLocalTrans(gridPos_World, transform, ConfigManager.GridSize);
                    MechaInfo.MechaEditorInventory.RefreshConflictAndIsolation();
                };
                mechaComponentInfo.InventoryItem.OnIsolatedHandler = MechaComponentGridRoot.SetIsolatedIndicatorShown;
                mechaComponentInfo.InventoryItem.OnConflictedHandler = MechaComponentGridRoot.SetGridConflicted;
                mechaComponentInfo.InventoryItem.OnResetConflictHandler = MechaComponentGridRoot.ResetAllGridConflict;
            }

            MechaComponentInfo = mechaComponentInfo;
            GridPos.ApplyGridPosToLocalTransXZ(MechaComponentInfo.InventoryItem.GridPos_World, transform, ConfigManager.GridSize);
            Mecha = parentMecha;
            MechaComponentGridRoot.SetInBattle(true);
            Child_Initialize();
        }

        protected virtual void Child_Initialize()
        {
        }

#if UNITY_EDITOR

        [MenuItem("开发工具/序列化模组占位")]
        public static void SerializeMechaComponentOccupiedPositions()
        {
            PrefabManager.Instance.LoadPrefabs();
            SortedDictionary<MechaComponentType, List<GridPos>> MechaComponentOccupiedGridPosDict = new SortedDictionary<MechaComponentType, List<GridPos>>();
            List<MechaComponentBase> mcbs = new List<MechaComponentBase>();

            foreach (string s in Enum.GetNames(typeof(MechaComponentType)))
            {
                MechaComponentType mcType = (MechaComponentType) Enum.Parse(typeof(MechaComponentType), s);

                GameObject prefab = PrefabManager.Instance.GetPrefab("MechaComponent_" + mcType);
                MechaComponentBase mcb = Instantiate(prefab).GetComponent<MechaComponentBase>();
                mcb.Initialize_Editor(new MechaComponentInfo(mcType, new GamePlayAbilityGroup(), 10, 0));
                mcbs.Add(mcb);
                MechaComponentOccupiedGridPosDict.Add(mcType, mcb.MechaComponentGridRoot.GetOccupiedPositions().Clone());
            }

            string json = JsonConvert.SerializeObject(MechaComponentOccupiedGridPosDict, Formatting.Indented);
            StreamWriter sw = new StreamWriter(ConfigManager.BlockOccupiedGridPosJsonFilePath);
            sw.Write(json);
            sw.Close();

            foreach (MechaComponentBase mcb in mcbs)
            {
                DestroyImmediate(mcb.gameObject);
            }
        }

        private void Initialize_Editor(MechaComponentInfo mechaComponentInfo)
        {
            MechaComponentInfo = mechaComponentInfo;
            GridPos.ApplyGridPosToLocalTransXZ(GridPos.Zero, transform, ConfigManager.GridSize);
        }
#endif

        public void SetShown(bool shown)
        {
            ModelRoot.SetActive(shown);
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
                if (Mecha && Mecha.MechaInfo.MechaType == MechaType.Player)
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

                OnRemoveMechaComponentBaseSuc?.Invoke(this);
                PoolRecycle();
            }

            return suc;
        }

        public void Draggable_OnMouseUp(DragArea dragArea, Vector3 diffFromStart, Vector3 deltaFromLastFrame)
        {
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