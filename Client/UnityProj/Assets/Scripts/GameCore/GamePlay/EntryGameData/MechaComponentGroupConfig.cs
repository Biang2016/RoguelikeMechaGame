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
        [TableList]
        public List<Config> MechaComponentList = new List<Config>();

        public struct Config
        {
            [PropertyOrder(-1)]
            [VerticalGroup("简称")]
            [HideLabel]
            [ShowInInspector]
            private string EditorDisplayName => Quality + "@" + (MechaComponentKey != null ? MechaComponentKey.Replace("MechaComponent_", "") : "");

            [VerticalGroup("机甲组件Key")]
            [HideLabel]
            public string MechaComponentKey;

            [VerticalGroup("机甲组件品质")]
            [HideLabel]
            public Quality Quality;
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