using System;
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
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using DragAreaDefines = GameCore.DragAreaDefines;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Client
{
    [ExecuteInEditMode]
    public abstract class MechaComponentBase : PoolObject, IDraggable
    {
        public MechaComponentInfo MechaComponentInfo;

        internal Mecha ParentMecha = null;

        public MechaComponentGridRoot MechaComponentGrids;
        public GameObject ModelRoot;
        public MechaComponentHitBoxRoot MechaHitBoxRoot;

        internal Draggable Draggable;
        private bool isReturningToBackpack = false;

        internal MechaType MechaType => ParentMecha ? ParentMecha.MechaInfo.MechaType : MechaType.None;

        public UnityAction<MechaComponentBase> OnRemoveMechaComponentBaseSuc;

        public override void PoolRecycle()
        {
            MechaComponentGrids.SetIsolatedIndicatorShown(true);
            MechaComponentInfo = null;
            ParentMecha = null;
            MechaHitBoxRoot.SetInBattle(false);
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
            if (!Application.isPlaying)
            {
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
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
            mechaComponentInfo.OnDied = () => PoolRecycle(0.2f);
            mechaComponentInfo.OnRemoveMechaComponentInfoSuc += (mci) =>
            {
                OnRemoveMechaComponentBaseSuc?.Invoke(this);
                PoolRecycle(1f);
            };

            {
                mechaComponentInfo.InventoryItem.OnSetGridPosHandler = (gridPos_World) => { GridPosR.ApplyGridPosToLocalTrans(gridPos_World, transform, ConfigManager.GridSize); };
                mechaComponentInfo.InventoryItem.OnIsolatedHandler = MechaComponentGrids.SetIsolatedIndicatorShown;
                mechaComponentInfo.InventoryItem.OnConflictedHandler = MechaComponentGrids.SetGridConflicted;
                mechaComponentInfo.InventoryItem.OnResetConflictHandler = MechaComponentGrids.ResetAllGridConflict;
            }

            MechaComponentInfo = mechaComponentInfo;
            GridPos.ApplyGridPosToLocalTrans(MechaComponentInfo.InventoryItem.GridPos_World, transform, ConfigManager.GridSize);
            ParentMecha = parentMecha;
            MechaHitBoxRoot.SetInBattle(true);
            Child_Initialize();
        }

        protected virtual void Child_Initialize()
        {
        }

#if UNITY_EDITOR

        [MenuItem("Tools/序列化模组占位")]
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
                mcb.Initialize_Editor(new MechaComponentInfo(mcType, 10, 0));
                mcbs.Add(mcb);
                MechaComponentOccupiedGridPosDict.Add(mcType, mcb.MechaComponentGrids.GetOccupiedPositions().Clone());
            }

            string json = JsonConvert.SerializeObject(MechaComponentOccupiedGridPosDict, Formatting.Indented);
            StreamWriter sw = new StreamWriter(GameCore.ConfigManager.BlockOccupiedGridPosJsonFilePath);
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
            GridPos.ApplyGridPosToLocalTrans(GridPos.Zero, transform, ConfigManager.GridSize);
        }
#endif

        public void SetShown(bool shown)
        {
            ModelRoot.SetActive(shown);
        }

        #region IDraggable

        public void Draggable_OnMouseDown(DragArea dragArea, Collider collider)
        {
        }

        public void Draggable_OnMousePressed(DragArea dragArea)
        {
            if (ControlManager.Instance.Building_RotateItem.Down)
            {
                MechaComponentInfo.InventoryItem.Rotate();
            }

            if (dragArea.Equals(DragAreaDefines.BattleInventory))
            {
                ReturnToBackpack(true, true);
                return;
            }

            if (ParentMecha && ParentMecha.MechaInfo.MechaType == MechaType.Player)
            {
                Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(ControlManager.Instance.Building_MousePosition);
                GridPos gridPos = GridUtils.GetGridPosByMousePos(ParentMecha.transform, ray, Vector3.up, ConfigManager.GridSize);
                GridPosR gp_matrix = MechaComponentInfo.InventoryItem.Inventory.CoordinateTransformationHandler_FromPosToMatrixIndex(gridPos);
                gp_matrix.orientation = MechaComponentInfo.InventoryItem.GridPos_Matrix.orientation;
                MechaComponentInfo.InventoryItem.SetGridPosition(gp_matrix);
            }
        }

        public bool ReturnToBackpack(bool cancelDrag, bool dragTheItem)
        {
            Backpack bp = BackpackManager.Instance.GetBackPack(DragAreaDefines.BattleInventory.DragAreaName);
            InventoryItem ii = new InventoryItem(MechaComponentInfo, bp, GridPosR.Zero);
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
                    DragManager.Instance.CurrentDrag = bp.BackpackPanel.GetBackpackItem(ii.GUID).gameObject
                        .GetComponent<Draggable>();
                    DragManager.Instance.CurrentDrag.SetOnDrag(true, null, DragManager.Instance.GetDragProcessor<BackpackItem>());
                }

                OnRemoveMechaComponentBaseSuc?.Invoke(this);
                PoolRecycle();
            }

            return suc;
        }

        public void Draggable_OnMouseUp(DragArea dragArea)
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

        float IDraggable.Draggable_DragMinDistance => 0f;

        float IDraggable.Draggable_DragMaxDistance => 9999f;

        public void Draggable_DragOutEffects()
        {
        }

        #endregion
    }
}