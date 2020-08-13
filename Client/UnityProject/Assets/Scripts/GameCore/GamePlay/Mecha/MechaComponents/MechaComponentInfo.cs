using System;
using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ShapedInventory;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace GameCore
{
    [Serializable]
    public partial class MechaComponentInfo : IInventoryItemContentInfo
    {
        [ReadOnly]
        [HideInEditorMode]
        public uint GUID;

        private static uint guidGenerator = (uint) ConfigManager.GUID_Separator.MechaComponentInfo;

        private uint GetGUID()
        {
            return guidGenerator++;
        }

        [HideInEditorMode]
        public InventoryItem InventoryItem;

        private MechaComponentOriginalOccupiedGridInfo MechaComponentOriginalOccupiedGridInfo;

        #region IInventoryItemContentInfo

        public List<GridPos> IInventoryItemContentInfo_OriginalOccupiedGridPositions => MechaComponentOriginalOccupiedGridInfo.MechaComponentOccupiedGridPositionList;

        public string ItemCategoryName => MechaComponentType.ToString();
        public string ItemName => MechaComponentConfig.EnglishName;
        public string ItemQuality => Quality.ToString();
        public string ItemBasicInfo => CurrentQualityUpgradeData.GetBasicDescription();
        public string ItemDetailedInfo => CurrentQualityUpgradeData.GetDetailedDescription();
        public string ItemSpriteKey => MechaComponentConfig.ItemSpriteKey;
        public Color ItemColor => QualityManager.GetQuality(Quality).Color;

        #endregion

        public List<GridPos> OriginalAllSlotLocalPositions => MechaComponentOriginalOccupiedGridInfo.MechaComponentAllSlotLocalPositionsList;

        public MechaComponentType MechaComponentType => MechaComponentConfig.MechaComponentType;

        private string logIdentityName;
        public string LogIdentityName => logIdentityName;

        public UnityAction<MechaComponentInfo> OnRemoveMechaComponentInfoSuc;
        public UnityAction<Color, float> OnHighLightColorChange;

        public MechaComponentInfo(MechaComponentConfig mechaComponentConfig, Quality quality)
        {
            GUID = GetGUID();
            MechaComponentConfig = mechaComponentConfig;
            Quality = quality;
            AbilityGroup = ConfigManager.Instance.GetAbilityGroup(MechaComponentConfig.AbilityGroupConfigKey);
            MechaComponentQualityConfig = ConfigManager.Instance.GetMechaComponentQualityConfig(MechaComponentConfig.MechaComponentQualityConfigKey);

            CurrentQualityUpgradeData = MechaComponentQualityConfig.GetQualityUpgradeData(quality);
            if (CurrentQualityUpgradeData == null)
            {
                Debug.LogError($"未配置品质为{quality}的{MechaComponentType}");
            }
            else
            {
                CurrentPowerUpgradeData = CurrentQualityUpgradeData.GetPowerUpgradeData(0);
            }

            M_TotalLife = CurrentQualityUpgradeData.Life;
            M_LeftLife = CurrentQualityUpgradeData.Life;
            if (ConfigManager.MechaComponentOriginalOccupiedGridInfoDict.TryGetValue(mechaComponentConfig.MechaComponentKey, out MechaComponentOriginalOccupiedGridInfo info))
            {
                MechaComponentOriginalOccupiedGridInfo = info.Clone();
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
    }
}