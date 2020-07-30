﻿using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore
{
    [CreateAssetMenu(menuName = "BattleConfig/MechaComponentGroupConfig")]
    public class MechaComponentGroupConfigSSO : SerializedScriptableObject
    {
        [LabelText("机甲组件列表（工具）")]
        [OnValueChanged("OnMechaComponentGroupRawListChanged")]
        public List<ConfigRaw> MechaComponentGroupRawList = new List<ConfigRaw>();

        [NonSerialized]
        [OdinSerialize]
        [ShowInInspector]
        [LabelText("机甲组件组配置")]
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
            [LabelText("机甲组件Prefab")]
            public GameObject MechaComponentPrefab;

            [LabelText("机甲组件品质")]
            public Quality Quality;
        }

        [Button("刷新")]
        private void OnMechaComponentGroupRawListChanged()
        {
            mechaComponentGroupConfig.MechaComponentList.Clear();
            foreach (ConfigRaw raw in MechaComponentGroupRawList)
            {
                if (raw.MechaComponentPrefab != null)
                {
                    mechaComponentGroupConfig.MechaComponentList.Add(new MechaComponentGroupConfig.Config
                    {
                        MechaComponentKey = raw.MechaComponentPrefab.name,
                        Quality = raw.Quality
                    });
                }
            }
        }

        public void RefreshConfigList()
        {
            OnMechaComponentGroupRawListChanged();
        }
    }
}