using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore
{
    [CreateAssetMenu(menuName = "BattleConfig/MechaComponentQualityConfig")]
    public class MechaComponentQualityConfigSSO : SerializedScriptableObject
    {
        [OnValueChanged("OnMechaComponentPrefabChanged")]
        public GameObject MechaComponentPrefab;

        private void OnMechaComponentPrefabChanged()
        {
            if (MechaComponentPrefab)
            {
                MechaComponentQualityConfig.MechaComponentQualityConfigName = MechaComponentPrefab.name;
            }
            else
            {
                MechaComponentQualityConfig.MechaComponentQualityConfigName = null;
            }
        }

        [NonSerialized]
        [OdinSerialize]
        public MechaComponentQualityConfig MechaComponentQualityConfig = new MechaComponentQualityConfig();
    }
}