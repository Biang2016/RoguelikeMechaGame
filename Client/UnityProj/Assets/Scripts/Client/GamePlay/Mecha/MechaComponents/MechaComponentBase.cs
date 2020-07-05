using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using BiangStudio.CloneVariant;
using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay;
using BiangStudio.GridBag;
using BiangStudio.ObjectPool;
using GameCore;
using Newtonsoft.Json;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Events;

namespace Client
{
    [ExecuteInEditMode]
    public abstract class MechaComponentBase : PoolObject, IDraggable
    {
        internal Mecha ParentMecha = null;

        [SerializeField] private GameObject Models;

        internal Draggable Draggable;

        internal MechaType MechaType => ParentMecha ? ParentMecha.MechaInfo.MechaType : MechaType.None;
        public MechaComponentInfo MechaComponentInfo;

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

        public override void PoolRecycle()
        {
            ParentMecha = null;
            MechaHitBoxRoot.SetInBattle(false);
            base.PoolRecycle();
            foreach (FX lighteningFX in lighteningFXs)
            {
                lighteningFX.PoolRecycle();
            }

            lighteningFXs.Clear();
            isReturningToBag = false;
        }

        public static MechaComponentBase BaseInitialize(MechaComponentInfo mechaComponentInfo, Mecha parentMecha)
        {
            MechaComponentBase mcb = GameObjectPoolManager.Instance.MechaComponentPoolDict[mechaComponentInfo.MechaComponentType]
                .AllocateGameObject<MechaComponentBase>(parentMecha ? parentMecha.transform : null);
            mcb.Initialize(mechaComponentInfo, parentMecha);
            return mcb;
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
                mcb.Initialize_Editor(new MechaComponentInfo(mcType, new GridPosR(0, 0, GridPosR.Orientation.Up), 10, 0));
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
            GridPos.ApplyGridPosToLocalTrans(mechaComponentInfo.GridPos, transform, ConfigManager.GridSize);
        }
#endif

        private void Initialize(MechaComponentInfo mechaComponentInfo, Mecha parentMecha)
        {
            IsDead = false;
            MechaComponentInfo = mechaComponentInfo;
            GridPos.ApplyGridPosToLocalTrans(mechaComponentInfo.GridPos, transform, ConfigManager.GridSize);
            RefreshOccupiedGridPositions();
            ParentMecha = parentMecha;
            M_TotalLife = mechaComponentInfo.TotalLife;
            M_LeftLife = mechaComponentInfo.TotalLife;
            MechaHitBoxRoot.SetInBattle(true);
            Child_Initialize();
        }

        protected virtual void Child_Initialize()
        {
        }

        public void SetGridPosition(GridPosR gridPos)
        {
            if (!gridPos.Equals(MechaComponentInfo.GridPos))
            {
                foreach (GridPos gp in MechaComponentInfo.OccupiedGridPositions)
                {
                    GridPos gp_rot = GridPos.RotateGridPos(gp - (GridPos) MechaComponentInfo.GridPos, (GridPosR.Orientation) ((gridPos.orientation - MechaComponentInfo.GridPos.orientation + 4) % 4));
                    GridPosR newGP = gridPos + (GridPosR) gp_rot;
                    if (newGP.x > ConfigManager.EDIT_AREA_HALF_SIZE)
                    {
                        SetGridPosition(new GridPosR(gridPos.x - 1, gridPos.z, gridPos.orientation));
                        return;
                    }

                    if (newGP.x < -ConfigManager.EDIT_AREA_HALF_SIZE)
                    {
                        SetGridPosition(new GridPosR(gridPos.x + 1, gridPos.z, gridPos.orientation));
                        return;
                    }

                    if (newGP.z > ConfigManager.EDIT_AREA_HALF_SIZE)
                    {
                        SetGridPosition(new GridPosR(gridPos.x, gridPos.z - 1, gridPos.orientation));
                        return;
                    }

                    if (newGP.z < -ConfigManager.EDIT_AREA_HALF_SIZE)
                    {
                        SetGridPosition(new GridPosR(gridPos.x, gridPos.z + 1, gridPos.orientation));
                        return;
                    }
                }

                MechaComponentInfo.GridPos = gridPos;
                GridPosR.ApplyGridPosToLocalTrans(gridPos, transform, ConfigManager.GridSize);
                RefreshOccupiedGridPositions();
                ParentMecha?.RefreshMechaMatrix();
            }
        }

        private void RefreshOccupiedGridPositions()
        {
            if (ConfigManager.MechaComponentOccupiedGridPosDict.TryGetValue(MechaComponentInfo.MechaComponentType, out List<GridPos> ops))
            {
                MechaComponentInfo.OccupiedGridPositions = GridPos.TransformOccupiedPositions(MechaComponentInfo.GridPos, ops.Clone());
            }
        }

        public MechaComponentGridRoot MechaComponentGrids;
        public MechaComponentHitBoxRoot MechaHitBoxRoot;

        private void Rotate()
        {
            GridPosR newGP = new GridPosR(MechaComponentInfo.GridPos.x, MechaComponentInfo.GridPos.z, GridPosR.RotateOrientationClockwise90(MechaComponentInfo.GridPos.orientation));
            SetGridPosition(newGP);
        }

        public void SetShown(bool shown)
        {
            Models.SetActive(shown);
        }

        #region Life

        internal bool IsDead = false;

        internal UnityAction<int, int> OnLifeChange;

        private int _leftLife;

        public int M_LeftLife
        {
            get { return _leftLife; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                if (_leftLife != value)
                {
                    _leftLife = value;
                    OnLifeChange?.Invoke(_leftLife, M_TotalLife);
                }
            }
        }

        private int _totalLife;

        public int M_TotalLife
        {
            get { return _totalLife; }
            set
            {
                if (_totalLife != value)
                {
                    _totalLife = value;
                    OnLifeChange?.Invoke(M_LeftLife, _totalLife);
                }
            }
        }

        public bool CheckAlive()
        {
            return M_LeftLife > 0;
        }

        private List<FX> lighteningFXs = new List<FX>();

        public void Damage(int damage)
        {
            if (_leftLife > M_TotalLife * 0.5f && _leftLife - damage <= M_TotalLife * 0.5f)
            {
                foreach (MechaComponentHitBox hb in MechaHitBoxRoot.HitBoxes)
                {
                    FX lighteningFX = FXManager.Instance.PlayFX(FX_Type.FX_BlockDamagedLightening, hb.transform.position);
                    lighteningFXs.Add(lighteningFX);
                }
            }

            M_LeftLife -= damage;
            FXManager.Instance.PlayFX(FX_Type.FX_BlockDamageHit, transform.position + Vector3.up * 0.5f);

            if (!IsDead && !CheckAlive())
            {
                IsDead = true;
                OnDied();
                PoolRecycle(0.2f);
            }
        }

        private void OnDied()
        {
            FXManager.Instance.PlayFX(FX_Type.FX_BlockExplode, transform.position);
            ParentMecha?.RemoveMechaComponent(this);
        }

        #endregion

        #region IDraggable

        public void Draggable_OnMouseDown(string dragAreaName, Collider collider)
        {
        }

        public void Draggable_OnMousePressed(string dragAreaName)
        {
            if (ControlManager.Instance.Building_RotateItem.Down)
            {
                Rotate();
            }

            switch (DragManager.Instance.Current_DragAreaName)
            {
                case BiangStudio.GridBag.DragAreaDefines.Bag:
                {
                    ReturnToBag(true, true);
                    return;
                }
            }

            if (ParentMecha && ParentMecha.MechaInfo.MechaType == MechaType.Self)
            {
                Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(ControlManager.Instance.Building_MousePosition);
                GridPosR gridPos = GridUtils.GetGridPosByMousePos(ParentMecha.transform, ray, Vector3.up, ConfigManager.GridSize);
                gridPos.orientation = MechaComponentInfo.GridPos.orientation;
                SetGridPosition(gridPos);
            }
        }

        private bool isReturningToBag = false;

        public bool ReturnToBag(bool cancelDrag, bool dragTheItem)
        {
            BagItemInfo bii = new BagItemInfo(MechaComponentInfo);
            bii.BagItemContentInfo = MechaComponentInfo;
            bool suc = BagManager.Instance.BagInfo.TryAddItem(bii);
            if (suc)
            {
                if (cancelDrag)
                {
                    isReturningToBag = true;
                    DragManager.Instance.CurrentDrag = null;
                }

                if (dragTheItem)
                {
                    DragManager.Instance.CurrentDrag = BagManager.Instance.BagPanel.GetBagItem(bii.GUID).gameObject.GetComponent<DraggableBagItem>();
                    DragManager.Instance.CurrentDrag.SetOnDrag(true, null, DragManager.Instance.GetDragProcessor<BagItem>());
                }

                ParentMecha?.RemoveMechaComponent(this);
                PoolRecycle();
            }

            return suc;
        }

        public void Draggable_OnMouseUp(string dragAreaTypes)
        {
            switch (dragAreaTypes)
            {
                case BiangStudio.GridBag.DragAreaDefines.Bag:
                {
                    if (!isReturningToBag)
                    {
                        bool suc = ReturnToBag(false, false);
                        if (!suc)
                        {
                            DragManager.Instance.CurrentDrag.ResetToOriginalPositionRotation();
                        }
                    }

                    break;
                }
                case DragAreaDefines.MechaEditorArea:
                {
                    break;
                }
                case BiangStudio.DragHover.DragAreaDefines.None:
                {
                    bool suc = ReturnToBag(false, false);
                    if (!suc)
                    {
                        DragManager.Instance.CurrentDrag.ResetToOriginalPositionRotation();
                    }

                    break;
                }
            }
        }

        public void Draggable_SetStates(ref bool canDrag, ref string dragFrom)
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