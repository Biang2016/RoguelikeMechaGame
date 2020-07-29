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

        private List<GridPos> originalOccupiedGridPositions;
        public List<GridPos> OriginalOccupiedGridPositions => originalOccupiedGridPositions;

        public string ItemName => "机甲组件." + ItemSpriteKey;
        public MechaComponentType MechaComponentType => MechaComponentConfig.MechaComponentType;
        public string ItemSpriteKey => MechaComponentConfig.ItemSpriteKey;

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
            CurrentPowerUpgradeData = CurrentQualityUpgradeData.GetPowerUpgradeData(0);

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
    }
}