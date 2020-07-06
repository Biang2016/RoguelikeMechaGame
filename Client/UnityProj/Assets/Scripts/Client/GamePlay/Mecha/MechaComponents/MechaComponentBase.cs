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
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Client
{
    [ExecuteInEditMode]
    public abstract class MechaComponentBase : PoolObject, IDraggable
    {
        public MechaComponentInfo MechaComponentInfo;
        public InventoryItem InventoryItem;

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
            mechaComponentInfo.OnRemoveMechaComponentInfoSuc += (mci) => OnRemoveMechaComponentBaseSuc?.Invoke(this);

            MechaComponentInfo = mechaComponentInfo;
            GridPos.ApplyGridPosToLocalTrans(InventoryItem.Inventory.CoordinateTransformationHandler_FromMatrixIndexToPos(InventoryItem.GridPos_Matrix), transform, ConfigManager.GridSize);
            RefreshOccupiedGridPositions();
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

        public void SetInventoryItem(InventoryItem inventoryItem)
        {
            InventoryItem = inventoryItem;
            InventoryItem.OnSetGridPosHandler = SetGridPos;
        }

        private void SetGridPos(GridPosR gridPos)
        {
            GridPosR.ApplyGridPosToLocalTrans(gridPos, transform, ConfigManager.GridSize);
            ParentMecha?.RefreshMechaMatrix();
        }

        private void Rotate()
        {
            GridPosR newGP = new GridPosR(MechaComponentInfo.GridPos.x, MechaComponentInfo.GridPos.z, GridPosR.RotateOrientationClockwise90(MechaComponentInfo.GridPos.orientation));
            SetGridPosition(newGP);
        }

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
                Rotate();
            }

            if (DragManager.Instance.Current_DragArea.Equals(DragAreaDefines.BattleInventory))
            {
                ReturnToBackpack(true, true);
                return;
            }

            if (ParentMecha && ParentMecha.MechaInfo.MechaType == MechaType.Player)
            {
                Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(ControlManager.Instance.Building_MousePosition);
                GridPosR gridPos = GridUtils.GetGridPosByMousePos(ParentMecha.transform, ray, Vector3.up, ConfigManager.GridSize);
                gridPos.orientation = MechaComponentInfo.GridPos.orientation;
                SetGridPosition(gridPos);
            }
        }

        public bool ReturnToBackpack(bool cancelDrag, bool dragTheItem)
        {
            Inventory iv = BackpackManager.Instance.GetBackPack(DragAreaDefines.BattleInventory.DragAreaName);
            InventoryItem ii = new InventoryItem(MechaComponentInfo, iv);
            ii.ItemContentInfo = MechaComponentInfo;
            bool suc = iv.TryAddItem(ii);
            if (suc)
            {
                if (cancelDrag)
                {
                    isReturningToBackpack = true;
                    DragManager.Instance.CurrentDrag = null;
                }

                if (dragTheItem)
                {
                    DragManager.Instance.CurrentDrag = BackpackManager.Instance.GetBackPack(DragAreaDefines.BattleInventory.DragAreaName).BackpackPanel.GetBackpackItem(ii.GUID).gameObject.GetComponent<DraggableBackpackItem>();
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
                        DragManager.Instance.CurrentDrag.ResetToOriginalPositionRotation();
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
                    DragManager.Instance.CurrentDrag.ResetToOriginalPositionRotation();
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