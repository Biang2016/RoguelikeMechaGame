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

        internal Mecha Mecha = null;
        internal Inventory Inventory => MechaComponentInfo.InventoryItem.Inventory;
        internal InventoryItem InventoryItem => MechaComponentInfo.InventoryItem;

        public MechaComponentGridRoot MechaComponentGrids;
        public GameObject ModelRoot;
        public MechaComponentHitBoxRoot MechaHitBoxRoot;

        internal Draggable Draggable;
        private bool isReturningToBackpack = false;

        internal MechaType MechaType => Mecha ? Mecha.MechaInfo.MechaType : MechaType.None;

        public UnityAction<MechaComponentBase> OnRemoveMechaComponentBaseSuc;

        public override void PoolRecycle()
        {
            MechaComponentGrids.SetIsolatedIndicatorShown(true);
            MechaComponentInfo = null;
            Mecha = null;
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
            Mecha = parentMecha;
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

        private GridPos lastPickedUpHitBoxGridPos;
        private Vector3 dragBeginPosition;

        public void Draggable_OnMouseDown(DragArea dragArea, Collider collider)
        {
            MechaComponentHitBox hitBox = MechaHitBoxRoot.FindHitBox(collider);
            if (hitBox)
            {
                lastPickedUpHitBoxGridPos = hitBox.LocalGridPos + (GridPos) MechaComponentInfo.InventoryItem.GridPos_World;
            }

            dragBeginPosition = transform.position;
        }

        public void MoveBaseOnHitBox(GridPos hitBoxTargetPos)
        {
            GridPosR targetGP = hitBoxTargetPos - lastPickedUpHitBoxGridPos + (GridPos) InventoryItem.GridPos_World;
            GridPosR targetGP_matrix = MechaComponentInfo.InventoryItem.Inventory.CoordinateTransformationHandler_FromPosToMatrixIndex(targetGP);
            targetGP_matrix.orientation = InventoryItem.GridPos_Matrix.orientation;
            InventoryItem.SetGridPosition(targetGP_matrix);
        }

        public void Draggable_OnMousePressed(DragArea dragArea)
        {
            if (Inventory.RotateItemKeyDownHandler != null && Inventory.RotateItemKeyDownHandler.Invoke())
            {
                MechaComponentInfo.InventoryItem.Rotate();
            }

            if (dragArea.Equals(DragAreaDefines.BattleInventory))
            {
                ReturnToBackpack(true, true);
                return;
            }


            GridPosR gp_matrix = MechaComponentInfo.InventoryItem.Inventory.CoordinateTransformationHandler_FromPosToMatrixIndex(gridPos);
            gp_matrix.orientation = MechaComponentInfo.InventoryItem.GridPos_Matrix.orientation;
            MechaComponentInfo.InventoryItem.SetGridPosition(gp_matrix);

            if (Mecha && Mecha.MechaInfo.MechaType == MechaType.Player)
            {
                Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(Draggable.MyDragProcessor.GetDragMousePosition());
                Vector3 mousePosInWorld = GridUtils.GetPosByMousePos(Mecha.transform, ray, Vector3.up, Inventory.GridSize);

                float draggedDistance = (mousePosInWorld - dragBeginPosition).magnitude;
                if (draggedDistance < Draggable_DragMinDistance)
                {
                    //不动
                }
                else if (Draggable.MyDragProcessor.GetCurrentDragArea().Equals(Inventory.DragArea))
                {
                    Vector3 mcbPos = mousePosInWorld - dragBeginPosition + transform.position;
                    Vector3 local_GP = transform.InverseTransformPoint(mcbPos);
                    int x = Mathf.FloorToInt(local_GP.x / Inventory.GridSize) * Inventory.GridSize;
                    int z = Mathf.FloorToInt(local_GP.z / Inventory.GridSize) * Inventory.GridSize;

                    GridPosR gp_matrix = MechaComponentInfo.InventoryItem.Inventory.CoordinateTransformationHandler_FromPosToMatrixIndex(gridPos);


                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        panelRectTransform,
                        buildingMousePos,
                        Draggable.MyDragProcessor.GetCamera(),
                        out Vector2 anchoredPos))
                    {
                        anchoredPos.x += panelRectTransform.rect.width / 2f;
                        anchoredPos.y -= panelRectTransform.rect.height / 2f;
                        int grid_X = Mathf.FloorToInt((anchoredPos.x) / Backpack.GridSize);
                        int grid_Z = Mathf.FloorToInt((-anchoredPos.y) / Backpack.GridSize);
                        MoveBaseOnHitBox(new GridPos(grid_X, grid_Z));
                    }
                }
                else // drag out of the backpack
                {
                    Draggable_DragOutEffects();
                }
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

        public float Draggable_DragMinDistance => 0f;

        public float Draggable_DragMaxDistance => 9999f;

        public void Draggable_DragOutEffects()
        {
        }

        #endregion
    }
}