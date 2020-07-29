using System.Collections.Generic;
using BiangStudio.CloneVariant;
using Sirenix.OdinInspector;

namespace GameCore
{
    public class MechaComponentGroupConfig : IClone<MechaComponentGroupConfig>
    {
        [ReadOnly]
        [LabelText("机甲组件组名称")]
        public string MechaComponentGroupConfigName;

        [ReadOnly]
        [LabelText("机甲组件列表")]
        [ListDrawerSettings(ListElementLabelName = "DisplayName")]
        public List<Config> MechaComponentList = new List<Config>();

        public struct Config
        {
            [LabelText("机甲组件Key")]
            public string MechaComponentKey;

            [LabelText("机甲组件品质")]
            public Quality Quality;

            public string DisplayName => Quality + "@" + MechaComponentKey;
        }

        public MechaComponentGroupConfig Clone()
        {
            return new MechaComponentGroupConfig
            {
                MechaComponentList = MechaComponentList.Clone(),
            };
        }
    }
}