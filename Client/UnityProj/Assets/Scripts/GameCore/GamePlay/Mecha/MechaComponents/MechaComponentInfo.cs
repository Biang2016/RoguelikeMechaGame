using System;
using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay;
using BiangStudio.ShapedInventory;
using GameCore.AbilityDataDriven;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
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

        #region GamePlay Info

        public MechaComponentConfig MechaComponentConfig;

        [LabelText("品质")]
        [HideInEditorMode]
        public Quality Quality;

        private MechaInfo mechaInfo;

        public MechaInfo MechaInfo
        {
            get { return mechaInfo; }
            set
            {
                mechaInfo = value;
                if (mechaInfo != null)
                {
                    logIdentityName = $"{MechaInfo.LogIdentityName}-<color=\"#7D67FF\">{ItemName}</color>-{GUID}";
                }
            }
        }

        [ReadOnly]
        [ShowInInspector]
        [HideInEditorMode]
        [LabelText("技能组")]
        public AbilityGroup AbilityGroup;

        [ReadOnly]
        [ShowInInspector]
        [HideInEditorMode]
        [LabelText("品质配置")]
        public MechaComponentQualityConfig MechaComponentQualityConfig;

        public QualityUpgradeDataBase CurrentQualityUpgradeData;
        public PowerUpgradeDataBase CurrentPowerUpgradeData;

        #endregion

        [HideInEditorMode]
        public InventoryItem InventoryItem;

        private List<GridPos> originalOccupiedGridPositions;
        public List<GridPos> OriginalOccupiedGridPositions => originalOccupiedGridPositions;

        public string ItemName => "机甲组件." + ItemSpriteKey;
        public MechaComponentType MechaComponentType => MechaComponentConfig.MechaComponentType;
        public string ItemSpriteKey => MechaComponentConfig.ItemSpriteKey;

        private string logIdentityName;
        public string LogIdentityName => logIdentityName;

        public UnityAction<MechaComponentInfo> OnRemoveMechaComponentInfoSuc;

        public MechaComponentInfo(MechaComponentConfig mechaComponentConfig, Quality quality)
        {
            GUID = GetGUID();
            MechaComponentConfig = mechaComponentConfig;
            Quality = quality;
            AbilityGroup = ConfigManager.Instance.GetAbilityGroup(MechaComponentConfig.AbilityGroupConfigKey);
            MechaComponentQualityConfig = ConfigManager.Instance.GetMechaComponentQualityConfig(MechaComponentConfig.MechaComponentQualityConfigKey);

            CurrentQualityUpgradeData = MechaComponentQualityConfig.GetQualityUpgradeData(quality);

            M_TotalLife = CurrentQualityUpgradeData.Life;
            M_LeftLife = CurrentQualityUpgradeData.Life;
            if (ConfigManager.MechaComponentOccupiedGridPosDict.TryGetValue(mechaComponentConfig.MechaComponentKey, out List<GridPos> ops))
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
            MechaComponentInfo mci = new MechaComponentInfo(MechaComponentConfig, Quality);
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

        [HideInInspector]
        public bool IsDead = false;

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