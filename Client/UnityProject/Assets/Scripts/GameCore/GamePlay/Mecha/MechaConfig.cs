using System;
using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;
using Sirenix.OdinInspector;
using UnityEngine;

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
        public List<MechaAIParam> MechaAIParams = new List<MechaAIParam>();

        private Dictionary<MechaAIConfigParamType, float> mechaAIParamDict = new Dictionary<MechaAIConfigParamType, float>();

        public Dictionary<MechaAIConfigParamType, float> MechaAIParamDict
        {
            get
            {
                if (mechaAIParamDict.Count != MechaAIParams.Count)
                {
                    mechaAIParamDict.Clear();
                    foreach (MechaAIParam aiParam in MechaAIParams)
                    {
                        if (!mechaAIParamDict.ContainsKey(aiParam.ParamType))
                        {
                            mechaAIParamDict.Add(aiParam.ParamType, aiParam.ParamValue);
                        }
                        else
                        {
                            Debug.LogError($"在{MechaConfigName}里配置了重复的AI参数{aiParam.ParamType}");
                        }
                    }
                }

                return mechaAIParamDict;
            }
        }
       
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

    [Serializable]
    public class MechaAIParam : IClone<MechaAIParam>
    {
        public MechaAIConfigParamType ParamType;

        public float ParamValue;

        public MechaAIParam Clone()
        {
            return new MechaAIParam {ParamType = ParamType, ParamValue = ParamValue};
        }
    }
}