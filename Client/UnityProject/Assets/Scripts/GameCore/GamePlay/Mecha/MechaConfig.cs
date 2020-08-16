using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;
using Sirenix.OdinInspector;

namespace GameCore
{
    public class MechaConfig : IClone<MechaConfig>
    {
        [ReadOnly]
        [LabelText("机甲名称")]
        public string MechaConfigName;

        [ReadOnly]
        [LabelText("机甲组件列表")]
        [TableList]
        public List<Config> MechaComponentList = new List<Config>();

        [ReadOnly]
        [LabelText("敌兵AI配置Key")]
        public string MechaAIConfigKey;

        public struct Config
        {
            public string MechaComponentKey;
            public string MechaComponentAlias;
            public Quality MechaComponentQuality;
            public GridPosR GridPosR;
        }

        public MechaConfig Clone()
        {
            return new MechaConfig
            {
                MechaConfigName = MechaConfigName,
                MechaComponentList = MechaComponentList.Clone(),
                MechaAIConfigKey = MechaAIConfigKey
            };
        }
    }
}