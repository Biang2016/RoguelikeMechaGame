using System;
using System.Collections.Generic;
using GameCore.AbilityDataDriven;
using Sirenix.OdinInspector;
using UnityEngine;
using static System.String;

namespace GameCore
{
    [CreateAssetMenu(menuName = "BattleConfig/AllMechaComponentConfig")]
    public class AllMechaComponentConfigSSO : SerializedScriptableObject
    {
        [LabelText("机甲组件配置列表（工具）")]
        [OnValueChanged("OnMechaComponentConfigRawListChanged")]
        [ListDrawerSettings(ListElementLabelName = "EditorDisplayName", ShowIndexLabels = false, AddCopiesLastElement = true)]
        [TableList(ShowPaging = false)]
        public List<MechaComponentConfigRaw> MechaComponentConfigRawList = new List<MechaComponentConfigRaw>();

        [LabelText("机甲组件配置列表")]
        [TableList(ShowPaging = false)]
        public List<MechaComponentConfig> MechaComponentConfigList = new List<MechaComponentConfig>();

        public class MechaComponentConfigRaw
        {
            [VerticalGroup("机甲组件")]
            [TableColumnWidth(300, true)]
            [HideLabel]
            [Required]
            [AssetSelector(Filter = "MC_", Paths = "Assets/Resources/Prefabs/MechaComponents", FlattenTreeView = false)]
            [AssetsOnly]
            public MechaComponentBase MechaComponentPrefab;

            [VerticalGroup("机甲组件类型")]
            [HideLabel]
            [TableColumnWidth(80, true)]
            public MechaComponentType MechaComponentType;

            [VerticalGroup("物品图片")]
            [HideLabel]
            [Required]
            [AssetsOnly]
            [TableColumnWidth(110, true)]
            public Sprite ItemSprite;

            [VerticalGroup("技能组配置")]
            [HideLabel]
            [Required]
            [AssetsOnly]
            [TableColumnWidth(120, true)]
            public AbilityGroupConfigSSO AbilityGroupConfigSSO;

            [VerticalGroup("机甲组件品质配置")]
            [HideLabel]
            [Required]
            [AssetsOnly]
            [TableColumnWidth(120, true)]
            public MechaComponentQualityConfigSSO MechaComponentQualityConfigSSO;

            private string EditorDisplayName
            {
                get
                {
                    string shortName = MechaComponentPrefab != null ? MechaComponentPrefab.name.Replace("MC_", "") : "";
                    return $"{shortName}";
                }
            }

            public bool Valid => MechaComponentPrefab && ItemSprite && AbilityGroupConfigSSO && MechaComponentQualityConfigSSO;
        }

        [Button("刷新、排序")]
        private void OnMechaComponentConfigRawListChanged()
        {
            MechaComponentConfigRawList.Sort((x, y) =>
            {
                if (x.Valid && !y.Valid) return 1;
                if (!x.Valid && y.Valid) return -1;
                if (!x.Valid && !y.Valid) return 0;
                int result = x.MechaComponentType.CompareTo(y.MechaComponentType);
                if (result == 0)
                {
                    result = Compare(x.MechaComponentPrefab.name, y.MechaComponentPrefab.name, StringComparison.Ordinal);
                }

                return result;
            });
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