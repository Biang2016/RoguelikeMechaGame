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

        public struct Config
        {
            public string MechaComponentKey;
            public string MechaComponentAlias;
            public Quality MechaComponentQuality;
            public GridPosR GridPosR;
        }

        [ReadOnly]
        [LabelText("AI配置Key")]
        public string MechaAIConfigKey;

        [ReadOnly]
        [LabelText("AI参数")]
        public Dictionary<MechaAIConfigParamType, float> MechaAIParams = new Dictionary<MechaAIConfigParamType, float>();

        public MechaConfig Clone()
        {
            return new MechaConfig
            {
                MechaConfigName = MechaConfigName,
                MechaComponentList = MechaComponentList.Clone(),
                MechaAIConfigKey = MechaAIConfigKey,
                MechaAIParams = MechaAIParams.Clone(),
            };
        }
    }

    public enum MechaAIConfigParamType
    {
        MoveSpeed = 1,
        RotateSpeed = 2,

        Weapon0_AttackInterval = 10,

        AttackDistance = 20,
    }
}