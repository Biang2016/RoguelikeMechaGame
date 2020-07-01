﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
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

        [SerializeField]
        private GameObject Models;

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
            PrefabManager.Instance.LoadPrefabs_Editor();
            SortedDictionary<MechaComponentType, List<GridPos>> MechaComponentOccupiedGridPosDict = new SortedDictionary<MechaComponentType, List<GridPos>>();
            List<MechaComponentBase> mcbs = new List<MechaComponentBase>();

            foreach (string s in Enum.GetNames(typeof(MechaComponentType)))
            {
                MechaComponentType mcType = (MechaComponentType) Enum.Parse(typeof(MechaComponentType), s);

                GameObject prefab = PrefabManager.Instance.GetPrefab("MechaComponent_" + mcType);
                MechaComponentBase mcb = Instantiate(prefab).GetComponent<MechaComponentBase>();
                mcb.Initialize_Editor(new MechaComponentInfo(mcType, new GridPosR(0, 0, GridPosR.Orientation.Up), 10, 0));
                mcbs.Add(mcb);
                MechaComponentOccupiedGridPosDict.Add(mcType, CloneVariantUtils.List(mcb.MechaComponentGrids.GetOccupiedPositions()));
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
            GridPos.ApplyGridPosToLocalTrans(mechaComponentInfo.GridPos, transform, GameManager.GridSize);
        }
#endif

        private void Initialize(MechaComponentInfo mechaComponentInfo, Mecha parentMecha)
        {
            IsDead = false;
            MechaComponentInfo = mechaComponentInfo;
            GridPos.ApplyGridPosToLocalTrans(mechaComponentInfo.GridPos, transform, GameManager.GridSize);
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
                    if (newGP.x > ConfigManager.EDIT_AREA_SIZE)
                    {
                        SetGridPosition(new GridPosR(gridPos.x - 1, gridPos.z, gridPos.orientation));
                        return;
                    }

                    if (newGP.x < -ConfigManager.EDIT_AREA_SIZE)
                    {
                        SetGridPosition(new GridPosR(gridPos.x + 1, gridPos.z, gridPos.orientation));
                        return;
                    }

                    if (newGP.z > ConfigManager.EDIT_AREA_SIZE)
                    {
                        SetGridPosition(new GridPosR(gridPos.x, gridPos.z - 1, gridPos.orientation));
                        return;
                    }

                    if (newGP.z < -ConfigManager.EDIT_AREA_SIZE)
                    {
                        SetGridPosition(new GridPosR(gridPos.x, gridPos.z + 1, gridPos.orientation));
                        return;
                    }
                }

                MechaComponentInfo.GridPos = gridPos;
                GridPos.ApplyGridPosToLocalTrans(gridPos, transform, GameManager.GridSize);
                RefreshOccupiedGridPositions();
                ParentMecha?.RefreshMechaMatrix();
            }
        }

        private void RefreshOccupiedGridPositions()
        {
            if (GameCore.ConfigManager.MechaComponentOccupiedGridPosDict.TryGetValue(MechaComponentInfo.MechaComponentType, out List<GridPos> ops))
            {
                MechaComponentInfo.OccupiedGridPositions = GridPos.TransformOccupiedPositions(MechaComponentInfo.GridPos, ops);
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

        public void DragComponent_OnMouseDown(Collider collider)
        {
        }

        public void DragComponent_OnMousePressed(DragAreaTypes dragAreaTypes)
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                Rotate();
            }

            switch (dragAreaTypes)
            {
                case DragAreaTypes.Bag:
                {
                    ReturnToBag(true, true);
                    break;
                }
            }

            if (ParentMecha && ParentMecha.MechaInfo.MechaType == MechaType.Self)
            {
                GridPosR gridPos = ClientUtils.GetGridPosByMousePos(ParentMecha.transform, Vector3.up, GameManager.GridSize);
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
                    DragManager.Instance.CancelCurrentDrag();
                }

                if (dragTheItem)
                {
                    DragManager.Instance.CurrentDrag = BagManager.Instance.BagPanel.GetBagItem(bii.GUID).gameObject.GetComponent<Draggable>();
                    DragManager.Instance.CurrentDrag.SetOnDrag(true, null);
                }

                ParentMecha?.RemoveMechaComponent(this);
                PoolRecycle();
            }

            return suc;
        }

        public void DragComponent_OnMouseUp(DragAreaTypes dragAreaTypes)
        {
            switch (dragAreaTypes)
            {
                case DragAreaTypes.Bag:
                {
                    if (!isReturningToBag)
                    {
                        bool suc = ReturnToBag(false, false);
                        if (!suc)
                        {
                            DragManager.Instance.CurrentDrag.ReturnOriginalPositionRotation();
                        }
                    }

                    break;
                }
                case DragAreaTypes.MechaEditorArea:
                {
                    break;
                }
                case DragAreaTypes.None:
                {
                    bool suc = ReturnToBag(false, false);
                    if (!suc)
                    {
                        DragManager.Instance.CurrentDrag.ReturnOriginalPositionRotation();
                    }

                    break;
                }
            }
        }

        public void DragComponent_SetStates(ref bool canDrag, ref DragAreaTypes dragFrom)
        {
            canDrag = true;
            dragFrom = DragAreaTypes.MechaEditorArea;
        }

        float IDraggable.DragComponent_DragMinDistance => 0f;

        float IDraggable.DragComponent_DragMaxDistance => 9999f;

        public void DragComponent_DragOutEffects()
        {
        }

        #endregion
    }
}