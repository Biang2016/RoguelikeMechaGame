using System;
using BiangStudio.CloneVariant;
using Sirenix.OdinInspector;

namespace GameCore
{
    public class MechaComponentConfig : IClone<MechaComponentConfig>
    {
        [ReadOnly]
        [LabelText("机甲组件名称")]
        public string MechaComponentKey;

        [ReadOnly]
        [LabelText("机甲组件类型")]
        public MechaComponentType MechaComponentType;

        [ReadOnly]
        [LabelText("物品图片Key")]
        public string ItemSpriteKey;

        [ReadOnly]
        [LabelText("机甲组件品质配置Key")]
        public string MechaComponentQualityConfigKey;

        [ReadOnly]
        [LabelText("机甲组件技能组配置Key")]
        public string AbilityGroupConfigKey;

        private string EditorDisplayName => MechaComponentKey != null ? MechaComponentKey.Replace("MechaComponent_", "") : "";

        public MechaComponentConfig Clone()
        {
            MechaComponentConfig newConfig = new MechaComponentConfig();
            newConfig.MechaComponentKey = MechaComponentKey;
            newConfig.MechaComponentType = MechaComponentType;
            newConfig.ItemSpriteKey = ItemSpriteKey;
            newConfig.MechaComponentQualityConfigKey = MechaComponentQualityConfigKey;
            newConfig.AbilityGroupConfigKey = AbilityGroupConfigKey;
            return newConfig;
        }
    }
}