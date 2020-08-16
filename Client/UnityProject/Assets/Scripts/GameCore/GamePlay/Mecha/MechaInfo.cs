using System;
using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ShapedInventory;
using UnityEngine;
using UnityEngine.Events;

namespace GameCore
{
    [Serializable]
    public partial class MechaInfo : IClone<MechaInfo>
    {
        public uint GUID;
        private static uint guidGenerator = (uint) ConfigManager.GUID_Separator.MechaInfo;

        private uint GetGUID()
        {
            return guidGenerator++;
        }

        public MechaConfig MechaConfig;
        public string MechaName;
        public MechaType MechaType;
        public SortedDictionary<uint, MechaComponentInfo> MechaComponentInfoDict = new SortedDictionary<uint, MechaComponentInfo>();
        public Dictionary<string, MechaComponentInfo> MechaComponentInfoDict_Alias = new Dictionary<string, MechaComponentInfo>();

        public UnityAction<MechaComponentInfo, GridPosR> OnAddMechaComponentInfoSuc;
        public UnityAction<MechaInfo> OnRemoveMechaInfoSuc;
        public UnityAction<MechaComponentInfo> OnDropMechaComponent;

        public MechaEditorInventory MechaEditorInventory;

        public Vector3 Position;
        public Quaternion Rotation;

        public bool IsPlayer => MechaType == MechaType.Player;
        public bool IsBuilding => GameStateManager.Instance.GetState() == GameState.Building;
        public bool IsFighting => GameStateManager.Instance.GetState() == GameState.Fighting;

        public string LogIdentityName => $"<color=\"#61B2FF\">{MechaName}</color>-{GUID}";

        public MechaInfo(string mechaName, MechaType mechaType, MechaConfig mechaConfig)
        {
            GUID = GetGUID();
            MechaName = mechaName;
            MechaType = mechaType;
            MechaConfig = mechaConfig;
        }

        public MechaInfo Clone()
        {
            MechaInfo mechaInfo = new MechaInfo(MechaName, MechaType, MechaConfig.Clone());
            foreach (KeyValuePair<uint, MechaComponentInfo> kv in MechaComponentInfoDict)
            {
                AddMechaComponentInfo(kv.Value.Clone(), kv.Value.InventoryItem.GridPos_Matrix);
            }

            return mechaInfo;
        }

        public void AddMechaComponentInfo(MechaComponentInfo mci, GridPosR gp_matrix)
        {
            if (IsBuilding) _totalLife = 0;
            mci.MechaInfo = this;
            mci.OnRemoveMechaComponentInfoSuc += RemoveMechaComponentInfo;
            MechaComponentInfoDict.Add(mci.GUID, mci);
            if (!string.IsNullOrEmpty(mci.Alias))
            {
                if (MechaComponentInfoDict_Alias.ContainsKey(mci.Alias))
                {
                    MechaComponentInfo tempMCI = MechaComponentInfoDict_Alias[mci.Alias];
                    Debug.LogError($"机甲组件花名重复. " +
                                   $"机甲: {LogIdentityName}, 组件: {mci.LogIdentityName}, 花名: {mci.Alias}" +
                                   $"重复对象组件: {tempMCI.LogIdentityName}");
                }
                else
                {
                    MechaComponentInfoDict_Alias.Add(mci.Alias, mci);
                }
            }

            InventoryItem item = new InventoryItem(mci, MechaEditorInventory, gp_matrix);
            item.AmIRootItemInIsolationCalculationHandler = () => ((MechaComponentInfo) item.ItemContentInfo).MechaComponentType == MechaComponentType.Core;
            mci.SetInventoryItem(item);

            void instantiateMechaComponent()
            {
                item.Inventory = MechaEditorInventory;
                OnAddMechaComponentInfoSuc.Invoke(mci, gp_matrix);
                MechaEditorInventory.TryAddItem(item);
                MechaEditorInventory.RefreshConflictAndIsolation();
            }

            if (MechaEditorInventory != null)
            {
                instantiateMechaComponent();
            }
            else
            {
                OnInstantiated += instantiateMechaComponent;
            }
        }

        public UnityAction OnInstantiated;

        private void RemoveMechaComponentInfo(MechaComponentInfo mci)
        {
            if (IsBuilding) _totalLife = 0;
            MechaEditorInventory.RemoveItem(mci.InventoryItem, false);
            MechaEditorInventory.RefreshConflictAndIsolation(out List<InventoryItem> _, out List<InventoryItem> isolatedItems);
            if (MechaType == MechaType.Enemy)
            {
                foreach (InventoryItem item in isolatedItems)
                {
                    MechaComponentInfo _mci = (MechaComponentInfo) item.ItemContentInfo;

                    //int ran = LevelManager.SRandom.Range(0, 100);
                    //bool drop = ran < _mci.DropProbability;
                    //if (drop)
                    //{
                    //    OnDropMechaComponent?.Invoke(_mci);
                    //}
                }
            }

            MechaComponentInfoDict.Remove(mci.GUID);
            if (string.IsNullOrEmpty(mci.Alias))
            {
                MechaComponentInfoDict_Alias.Remove(mci.Alias);
            }

            if (MechaComponentInfoDict.Count == 0)
            {
                Die();
            }

            RefreshHUDPanelCoreLifeSliderCount?.Invoke();
            mci.OnRemoveMechaComponentInfoSuc = null;
            mci.MechaInfo = null;
        }

        public void RemoveMechaInfo()
        {
            OnRemoveMechaInfoSuc?.Invoke(this);
        }
    }
}