using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore
{
    [CreateAssetMenu(menuName = "BattleConfig/MechaComponentQualityConfig")]
    public class MechaComponentQualityConfigSSO : SerializedScriptableObject
    {
        [NonSerialized]
        [OdinSerialize]
        [ShowInInspector]
        [LabelText("机甲组件品质配置")]
        private MechaComponentQualityConfig mechaComponentQualityConfig = new MechaComponentQualityConfig();

        public MechaComponentQualityConfig MechaComponentQualityConfig
        {
            get
            {
                mechaComponentQualityConfig.MechaComponentQualityConfigName = name;
                return mechaComponentQualityConfig;
            }
        }
    }
}