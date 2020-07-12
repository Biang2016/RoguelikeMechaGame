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
    public class MechaInfo : IClone<MechaInfo>
    {
        public int GUID;
        private static int guidGenerator = (int) ConfigManager.GUID_Separator.MechaInfo;

        private int GetGUID()
        {
            return guidGenerator++;
        }

        public string MechaName;
        public MechaType MechaType;
        public SortedDictionary<int, MechaComponentInfo> MechaComponentInfos = new SortedDictionary<int, MechaComponentInfo>();

        public UnityAction<MechaComponentInfo, GridPosR> OnAddMechaComponentInfoSuc;
        public UnityAction<MechaInfo> OnRemoveMechaInfoSuc;
        public UnityAction<MechaComponentInfo> OnDropMechaComponent;

        public MechaEditorInventory MechaEditorInventory;

        public bool IsPlayer => MechaType == MechaType.Player;

        public MechaInfo(string mechaName, MechaType mechaType)
        {
            GUID = GetGUID();
            MechaName = mechaName;
            MechaType = mechaType;
        }

        public MechaInfo Clone()
        {
            MechaInfo mechaInfo = new MechaInfo(MechaName, MechaType);
            foreach (KeyValuePair<int, MechaComponentInfo> kv in MechaComponentInfos)
            {
                AddMechaComponentInfo(kv.Value.Clone(), kv.Value.InventoryItem.GridPos_Matrix);
            }

            return mechaInfo;
        }

        public void AddMechaComponentInfo(MechaComponentInfo mci, GridPosR gp_matrix)
        {
            mci.OnRemoveMechaComponentInfoSuc += RemoveMechaComponentInfo;
            MechaComponentInfos.Add(mci.GUID, mci);
            InventoryItem item = new InventoryItem(mci, MechaEditorInventory, gp_matrix);
            item.AmIRootItemInIsolationCalculationHandler = () => ((MechaComponentInfo) item.ItemContentInfo).MechaComponentType == MechaComponentType.Core;
            mci.SetInventoryItem(item);
            OnAddMechaComponentInfoSuc?.Invoke(mci, gp_matrix);
            MechaEditorInventory.TryAddItem(item);
            MechaEditorInventory.RefreshConflictAndIsolation();
        }

        private void RemoveMechaComponentInfo(MechaComponentInfo mci)
        {
            MechaEditorInventory.RemoveItem(mci.InventoryItem);
            MechaEditorInventory.RefreshConflictAndIsolation(out List<InventoryItem> _, out List<InventoryItem> isolatedItems);
            if (MechaType == MechaType.Enemy)
            {
                foreach (InventoryItem item in isolatedItems)
                {
                    MechaComponentInfo _mci = (MechaComponentInfo) item.ItemContentInfo;

                    int ran = LevelManager.SRandom.Range(0, 100);
                    bool drop = ran < _mci.DropProbability;
                    if (drop)
                    {
                        OnDropMechaComponent?.Invoke(_mci);
                    }
                }
            }

            mci.OnRemoveMechaComponentInfoSuc = null;
            MechaComponentInfos.Remove(mci.GUID);
        }

        public void RemoveMechaInfo()
        {
            OnRemoveMechaInfoSuc?.Invoke(this);
        }

        #region Life & Power

        public void UpdateLifeChange()
        {
            int totalLife = 0;
            int leftLife = 0;
            foreach (KeyValuePair<int, MechaComponentInfo> kv in MechaComponentInfos)
            {
                totalLife += kv.Value.M_TotalLife;
                leftLife += kv.Value.M_LeftLife;
            }

            M_TotalLife = Mathf.Max(M_TotalLife, totalLife);
            M_LeftLife = leftLife;
        }

        public UnityAction RefreshHUDPanelCoreLifeSliderCount;

        public List<MechaComponentInfo> GetCoreLifeChangeDelegates()
        {
            List<MechaComponentInfo> res = new List<MechaComponentInfo>();
            foreach (KeyValuePair<int, MechaComponentInfo> kv in MechaComponentInfos)
            {
                if (kv.Value.MechaComponentType == MechaComponentType.Core)
                {
                    res.Add(kv.Value);
                }
            }

            return res;
        }

        public UnityAction<int, int> OnLifeChange;

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

        public UnityAction<int, int> OnPowerChange;

        private int _leftPower;

        public int M_LeftPower
        {
            get { return _leftPower; }
            set
            {
                if (_leftPower != value)
                {
                    _leftPower = value;
                    OnPowerChange?.Invoke(_leftPower, M_TotalPower);
                }
            }
        }

        private int _totalPower;

        public int M_TotalPower
        {
            get { return _totalPower; }
            set
            {
                if (_totalPower != value)
                {
                    _totalPower = value;
                    OnPowerChange?.Invoke(M_LeftPower, _totalPower);
                }
            }
        }

        #endregion
    }

    public enum MechaType
    {
        None = 0,
        Player = 1,
        Enemy = 2,
    }
}