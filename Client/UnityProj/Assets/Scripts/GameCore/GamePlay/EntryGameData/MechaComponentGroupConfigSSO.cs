using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore
{
    [CreateAssetMenu(menuName = "BattleConfig/MechaComponentGroupConfig")]
    public class MechaComponentGroupConfigSSO : SerializedScriptableObject
    {
        [OnValueChanged("OnMechaComponentGroupRawListChanged")]
        public MechaComponentGroupConfig MechaComponentGroupConfig = new MechaComponentGroupConfig();

        [LabelText("机甲组件列表（工具）")]
        public List<ConfigRaw> MechaComponentGroupRawList = new List<ConfigRaw>();

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
            MechaComponentGroupConfig.MechaComponentList.Clear();
            foreach (ConfigRaw raw in MechaComponentGroupRawList)
            {
                if (raw.MechaComponentPrefab != null)
                {
                    MechaComponentGroupConfig.MechaComponentList.Add(new MechaComponentGroupConfig.Config
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