using System;
using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ShapedInventory;
using GameCore.AbilityDataDriven;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace GameCore
{
    [Serializable]
    public class MechaComponentInfo : IInventoryItemContentInfo
    {
        [ReadOnly]
        [HideInEditorMode]
        public uint GUID;

        private static uint guidGenerator = (uint) ConfigManager.GUID_Separator.MechaComponentInfo;

        private uint GetGUID()
        {
            return guidGenerator++;
        }

        public MechaComponentType MechaComponentType;

        public MechaInfo MechaInfo;

        [ReadOnly]
        [ShowInInspector]
        [DisableInEditorMode]
        [ListDrawerSettings(ListElementLabelName = "AbilityName")]
        public GamePlayAbilityGroup AbilityGroup;

        [HideInEditorMode]
        public InventoryItem InventoryItem;

        public UnityAction<MechaComponentInfo> OnRemoveMechaComponentInfoSuc;

        private List<GridPos> originalOccupiedGridPositions;

        public List<GridPos> OriginalOccupiedGridPositions => originalOccupiedGridPositions;
        public string ItemSpriteKey => typeof(MechaComponentType).FullName + "." + MechaComponentType;
        public string ItemName => "机甲组件." + MechaComponentType;

        public MechaComponentInfo(MechaComponentType mechaComponentType, GamePlayAbilityGroup abilityGroup, int totalLife, int dropProbability)
        {
            GUID = GetGUID();
            MechaComponentType = mechaComponentType;
            AbilityGroup = abilityGroup;
            TotalLife = totalLife;
            M_LeftLife = totalLife;
            DropProbability = dropProbability;
            if (ConfigManager.MechaComponentOccupiedGridPosDict.TryGetValue(mechaComponentType, out List<GridPos> ops))
            {
                originalOccupiedGridPositions = ops.Clone();
            }
        }

        public void Reset()
        {
            OnDied = null;
            OnDamaged = null;
            OnLifeChange = null;
            OnRemoveMechaComponentInfoSuc = null;
        }

        public MechaComponentInfo Clone()
        {
            MechaComponentInfo mci = new MechaComponentInfo(MechaComponentType, AbilityGroup.Clone(), TotalLife, DropProbability);
            return mci;
        }

        public void RemoveMechaComponentInfo()
        {
            OnRemoveMechaComponentInfoSuc?.Invoke(this);
        }

        public void SetInventoryItem(InventoryItem inventoryItem)
        {
            InventoryItem = inventoryItem;
        }

        #region Life

        public int DropProbability;
        public int TotalLife;

        internal bool IsDead = false;

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

        public bool CheckAlive()
        {
            return M_LeftLife > 0;
        }

        public UnityAction<MechaComponentInfo, int> OnDamaged;

        public void Damage(MechaComponentInfo attacker, int damage)
        {
            M_LeftLife -= damage;
            OnDamaged?.Invoke(attacker, damage);

            if (!IsDead && !CheckAlive())
            {
                IsDead = true;
                Died();
                OnDied?.Invoke();
            }
        }

        public UnityAction OnDied;

        private void Died()
        {
            OnRemoveMechaComponentInfoSuc?.Invoke(this);
        }

        #endregion
    }
}