using System;
using System.Collections.Generic;
using GameCore.AbilityDataDriven;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore
{
    [CreateAssetMenu(menuName = "BattleConfig/AllMechaComponentConfig")]
    public class AllMechaComponentConfigSSO : SerializedScriptableObject
    {
        [LabelText("机甲组件配置列表（工具）")]
        [OnValueChanged("OnMechaComponentConfigRawListChanged")]
        [ListDrawerSettings(ListElementLabelName = "EditorDisplayName", ShowIndexLabels = false)]
        public List<MechaComponentConfigRaw> MechaComponentConfigRawList = new List<MechaComponentConfigRaw>();

        [LabelText("机甲组件配置列表")]
        public List<MechaComponentConfig> MechaComponentConfigList = new List<MechaComponentConfig>();

        public class MechaComponentConfigRaw
        {
            [LabelText("机甲组件Prefab")]
            public GameObject MechaComponentPrefab;

            [LabelText("机甲组件类型")]
            public MechaComponentType MechaComponentType;

            [LabelText("物品图片")]
            public Sprite ItemSprite;

            [LabelText("技能组配置")]
            public AbilityGroupConfigSSO AbilityGroupConfigSSO;

            [LabelText("机甲组件品质配置")]
            public MechaComponentQualityConfigSSO MechaComponentQualityConfigSSO;

            private string EditorDisplayName => MechaComponentPrefab != null ? MechaComponentPrefab.name.Replace("MechaComponent_", "") : "";
        }

        [Button("刷新")]
        private void OnMechaComponentConfigRawListChanged()
        {
            MechaComponentConfigList.Clear();
            foreach (MechaComponentConfigRaw raw in MechaComponentConfigRawList)
            {
                if (raw.MechaComponentPrefab != null &&
                    raw.ItemSprite != null &&
                    raw.AbilityGroupConfigSSO != null &&
                    raw.MechaComponentQualityConfigSSO != null
                )
                {
                    MechaComponentConfigList.Add(new MechaComponentConfig
                    {
                        MechaComponentKey = raw.MechaComponentPrefab.name,
                        MechaComponentType = raw.MechaComponentType,
                        ItemSpriteKey = raw.ItemSprite.name,
                        AbilityGroupConfigKey = raw.AbilityGroupConfigSSO.AbilityGroupName,
                        MechaComponentQualityConfigKey = raw.MechaComponentQualityConfigSSO.MechaComponentQualityConfig.MechaComponentQualityConfigName,
                    });
                }
            }
        }

        public void RefreshConfigList()
        {
            OnMechaComponentConfigRawListChanged();
        }
    }
}