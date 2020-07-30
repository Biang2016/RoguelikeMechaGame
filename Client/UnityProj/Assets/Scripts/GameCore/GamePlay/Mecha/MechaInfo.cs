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
        public uint GUID;
        private static uint guidGenerator = (uint) ConfigManager.GUID_Separator.MechaInfo;

        private uint GetGUID()
        {
            return guidGenerator++;
        }

        public string MechaName;
        public MechaType MechaType;
        public SortedDictionary<uint, MechaComponentInfo> MechaComponentInfoDict = new SortedDictionary<uint, MechaComponentInfo>();

        public UnityAction<MechaComponentInfo, GridPosR> OnAddMechaComponentInfoSuc;
        public UnityAction<MechaInfo> OnRemoveMechaInfoSuc;
        public UnityAction<MechaComponentInfo> OnDropMechaComponent;

        public MechaEditorInventory MechaEditorInventory;

        public bool IsPlayer => MechaType == MechaType.Player;
        public bool IsBuilding => GameStateManager.Instance.GetState() == GameState.Building;
        public bool IsFighting => GameStateManager.Instance.GetState() == GameState.Fighting;

        public string LogIdentityName => $"<color=\"#61B2FF\">{MechaName}</color>-{GUID}";

        public MechaInfo(string mechaName, MechaType mechaType)
        {
            GUID = GetGUID();
            MechaName = mechaName;
            MechaType = mechaType;
        }

        public MechaInfo Clone()
        {
            MechaInfo mechaInfo = new MechaInfo(MechaName, MechaType);
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
            InventoryItem item = new InventoryItem(mci, MechaEditorInventory, gp_matrix);
            item.AmIRootItemInIsolationCalculationHandler = () => ((MechaComponentInfo) item.ItemContentInfo).MechaComponentType == MechaComponentType.Core;
            mci.SetInventoryItem(item);
            OnAddMechaComponentInfoSuc?.Invoke(mci, gp_matrix);
            MechaEditorInventory.TryAddItem(item);
            MechaEditorInventory.RefreshConflictAndIsolation();
        }

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

        #region Relationships

        public bool IsFriend(MechaInfo mechaInfo)
        {
            return IsFriend(mechaInfo.MechaType);
        }

        public bool IsFriend(MechaType targetMechaType)
        {
            if (MechaType == targetMechaType) return true;
            if (targetMechaType == MechaType.Friend && MechaType == MechaType.Player) return true;
            if (MechaType == MechaType.Friend && targetMechaType == MechaType.Player) return true;
            return false;
        }

        public bool IsMainPlayerFriend()
        {
            return MechaType == MechaType.Friend;
        }

        public bool IsOpponent(MechaInfo mechaInfo)
        {
            return IsOpponent(mechaInfo.MechaType);
        }

        public bool IsOpponent(MechaType targetMechaType)
        {
            if (targetMechaType == MechaType.Player && MechaType == MechaType.Enemy) return true;
            if (MechaType == MechaType.Enemy && targetMechaType == MechaType.Player) return true;
            if (targetMechaType == MechaType.Friend && MechaType == MechaType.Enemy) return true;
            if (MechaType == MechaType.Enemy && targetMechaType == MechaType.Friend) return true;
            return false;
        }

        #endregion


        #region Life & Power

        public bool IsDead;

        public void UpdateLifeChange()
        {
            int totalLife = 0;
            int leftLife = 0;
            foreach (KeyValuePair<uint, MechaComponentInfo> kv in MechaComponentInfoDict)
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
            foreach (KeyValuePair<uint, MechaComponentInfo> kv in MechaComponentInfoDict)
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
            private set
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
            private set
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
            private set
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
            private set
            {
                if (_totalPower != value)
                {
                    _totalPower = value;
                    OnPowerChange?.Invoke(M_LeftPower, _totalPower);
                }
            }
        }

        private void Die()
        {
            IsDead = true;
            if (IsPlayer)
            {
                // TODO Endgame
            }
            else
            {
                OnRemoveMechaInfoSuc(this);
            }
        }

        #endregion
    }

    public enum MechaType
    {
        None = 0,
        Player = 1,
        Enemy = 2,
        Friend = 3,
    }
}