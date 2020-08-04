using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore
{
    [CreateAssetMenu(menuName = "BattleConfig/MechaComponentGroupConfig")]
    public class MechaComponentGroupConfigSSO : SerializedScriptableObject
    {
        [TableList(ShowPaging = false, ShowIndexLabels = true)]
        [LabelText("机甲组件列表（工具）")]
        [OnValueChanged("OnMechaComponentGroupRawListChanged")]
        public List<ConfigRaw> MechaComponentGroupRawList = new List<ConfigRaw>();

        [ShowInInspector]
        [LabelText("机甲组件组配置")]
        [TableList(ShowPaging = false)]
        [ReadOnly]
        private MechaComponentGroupConfig mechaComponentGroupConfig = new MechaComponentGroupConfig();

        public MechaComponentGroupConfig MechaComponentGroupConfig
        {
            get
            {
                mechaComponentGroupConfig.MechaComponentGroupConfigName = name;
                return mechaComponentGroupConfig;
            }
        }

        public class ConfigRaw
        {
            [PropertyOrder(-1)]
            [VerticalGroup("简称")]
            [HideLabel]
            [ShowInInspector]
            [TableColumnWidth(150, true)]
            private string EditorDisplayName
            {
                get
                {
                    string shortName = MechaComponentPrefab != null ? MechaComponentPrefab.name.Replace("MC_", "") : "";
                    shortName = shortName.PadRight(30, '-');
                    return $"{shortName}\t{Quality}";
                }
            }

            [VerticalGroup("机甲组件")]
            [TableColumnWidth(300, true)]
            [HideLabel]
            [Required]
            [AssetSelector(Filter = "MC_", Paths = "Assets/Resources/Prefabs/MechaComponents", FlattenTreeView = false)]
            [AssetsOnly]
            [OnValueChanged("OnMechaComponentPrefabChanged")]
            public MechaComponentBase MechaComponentPrefab;

            [VerticalGroup("机甲组件品质")]
            [HideLabel]
            [TableColumnWidth(120, true)]
            [ValidateInput("ValidateQuality", "$qualityMessage")]
            public Quality Quality;

            [VerticalGroup("数量")]
            [HideLabel]
            [TableColumnWidth(50, true)]
            [ValidateInput("ValidateCount", "$countMessage")]
            public int Count;

            public bool Valid => MechaComponentPrefab != null;

            private void OnMechaComponentPrefabChanged()
            {
                Quality = Quality.None;
            }

            public bool ValidateQuality(Quality quality)
            {
                if (MechaComponentPrefab)
                {
                    MechaComponentConfig mcc = ConfigManager.Instance.GetMechaComponentConfig(MechaComponentPrefab.name);
                    MechaComponentQualityConfig qc = ConfigManager.Instance.GetMechaComponentQualityConfig(mcc.MechaComponentQualityConfigKey);
                    if (qc.GetQualityUpgradeData(quality) != null)
                    {
                        qualityMessage = "";
                        return true;
                    }
                    else
                    {
                        qualityMessage = $"{MechaComponentPrefab.name}组件未配置{quality}品质";
                        return false;
                    }
                }
                else
                {
                    qualityMessage = "机甲组件为空，请先设置机甲组件";
                    return false;
                }
            }

            private bool ValidateCount(int count)
            {
                if (count < 0)
                {
                    countMessage = "数量不能为负";
                    return false;
                }
                else
                {
                    countMessage = "";
                    return true;
                }
            }

            [NonSerialized]
            [HideInInspector]
            public string qualityMessage = "";

            private string countMessage = "";
        }

        [Button("刷新、排序")]
        private void OnMechaComponentGroupRawListChanged()
        {
            MechaComponentGroupRawList.Sort((x, y) =>
            {
                if (x.Valid && !y.Valid) return 1;
                if (!x.Valid && y.Valid) return -1;
                if (!x.Valid && !y.Valid) return 0;
                int result = String.Compare(x.MechaComponentPrefab.name, y.MechaComponentPrefab.name, StringComparison.Ordinal);
                if (result == 0)
                {
                    result = x.Quality.CompareTo(y.Quality);
                }

                return result;
            });

            mechaComponentGroupConfig.MechaComponentList.Clear();
            foreach (ConfigRaw raw in MechaComponentGroupRawList)
            {
                if (raw.MechaComponentPrefab != null)
                {
                    for (int i = 0; i < raw.Count; i++)
                    {
                        mechaComponentGroupConfig.MechaComponentList.Add(new MechaComponentGroupConfig.Config
                        {
                            MechaComponentKey = raw.MechaComponentPrefab.name,
                            Quality = raw.Quality
                        });
                    }
                }
            }
        }

        public void RefreshConfigListBeforeExport()
        {
            OnMechaComponentGroupRawListChanged();
            foreach (ConfigRaw configRow in MechaComponentGroupRawList)
            {
                if (!configRow.ValidateQuality(configRow.Quality))
                {
                    Debug.LogError(configRow.qualityMessage);
                }
            }
        }
    }
}