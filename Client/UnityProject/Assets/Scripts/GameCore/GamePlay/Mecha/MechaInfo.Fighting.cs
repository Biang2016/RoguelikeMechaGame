using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ShapedInventory;
using UnityEngine;
using UnityEngine.Events;

namespace GameCore
{
    public partial class MechaInfo
    {
        public bool AbilityForbidMovement = false;

        public void Update_Fighting()
        {
            AbilityForbidMovement = false;
            foreach (KeyValuePair<uint, MechaComponentInfo> kv in MechaComponentInfoDict)
            {
                if (!kv.Value.IsDead)
                {
                    kv.Value.PreUpdate_Fighting();
                }
            }

            foreach (KeyValuePair<uint, MechaComponentInfo> kv in MechaComponentInfoDict)
            {
                if (!kv.Value.IsDead)
                {
                    kv.Value.PowerUpdate_Fighting();
                }
            }

            foreach (KeyValuePair<uint, MechaComponentInfo> kv in MechaComponentInfoDict)
            {
                if (!kv.Value.IsDead)
                {
                    kv.Value.M_InputPower = kv.Value.AccumulatedPowerInsideThisFrame;
                    kv.Value.AccumulatedPowerInsideThisFrame = 0;
                }
            }

            foreach (KeyValuePair<uint, MechaComponentInfo> kv in MechaComponentInfoDict)
            {
                if (!kv.Value.IsDead)
                {
                    kv.Value.Update_Fighting();
                }
            }
        }

        public void LateUpdate_Fighting()
        {
            foreach (KeyValuePair<uint, MechaComponentInfo> kv in MechaComponentInfoDict)
            {
                if (!kv.Value.IsDead)
                {
                    kv.Value.LateUpdate_Fighting();
                }
            }
        }

        public void FixedUpdate_Fighting()
        {
            foreach (KeyValuePair<uint, MechaComponentInfo> kv in MechaComponentInfoDict)
            {
                if (!kv.Value.IsDead)
                {
                    kv.Value.FixedUpdate_Fighting();
                }
            }
        }

        #region PowerCalculation

        public void ProvidePower(GridPos destinationGP, int power)
        {
            if (destinationGP.x >= 0 && destinationGP.x < MechaEditorInventory.InventoryItemMatrix.GetLength(0) && destinationGP.z >= 0 && destinationGP.z < MechaEditorInventory.InventoryItemMatrix.GetLength(1))
            {
                InventoryItem item = MechaEditorInventory.InventoryItemMatrix[destinationGP.x, destinationGP.z];
                if (item != null)
                {
                    MechaComponentInfo mci = ((MechaComponentInfo) item.ItemContentInfo);
                    mci.AccumulatedPowerInsideThisFrame += power;
                }
            }
        }

        #endregion

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

        public UnityAction<int, int> OnBatteryPowerChange;

        private int _leftBatteryPower;

        public int M_LeftBatteryPower
        {
            get { return _leftBatteryPower; }
            private set
            {
                if (_leftBatteryPower != value)
                {
                    _leftBatteryPower = value;
                    OnBatteryPowerChange?.Invoke(_leftBatteryPower, M_TotalBatteryPower);
                }
            }
        }

        private int _totalBatteryPower;

        public int M_TotalBatteryPower
        {
            get { return _totalBatteryPower; }
            private set
            {
                if (_totalBatteryPower != value)
                {
                    _totalBatteryPower = value;
                    OnBatteryPowerChange?.Invoke(M_LeftBatteryPower, _totalBatteryPower);
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