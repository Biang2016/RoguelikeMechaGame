using System;
using System.Collections.Generic;
using BiangStudio.CloneVariant;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore
{
    public class MechaComponentConfig : IClone<MechaComponentConfig>
    {
        [ReadOnly]
        [LabelText("机甲组件名称")]
        public string MechaComponentKey;

        public MechaComponentType MechaComponentType;
        public string ItemSpriteKey;
        public string MechaComponentQualityConfigKey;
        public string AbilityGroupConfigKey;

        public MechaComponentConfig Clone()
        {
            MechaComponentConfig newConfig = new MechaComponentConfig();
            newConfig.MechaComponentKey = MechaComponentKey;
            newConfig.MechaComponentQualityConfigKey = MechaComponentQualityConfigKey;
            newConfig.AbilityGroupConfigKey = AbilityGroupConfigKey;
            return newConfig;
        }
    }
}