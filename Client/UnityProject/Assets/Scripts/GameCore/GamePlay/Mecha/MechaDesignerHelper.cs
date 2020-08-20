using System;
using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine;
using GameCore;
using NodeCanvas.BehaviourTrees;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GameCore
{
    public class MechaDesignerHelper : MonoBehaviour
    {
        [ShowInInspector]
        [LabelText("AI参数")]
        public list<MechaAIConfigParamType, float> MechaAIParams = new Dictionary<MechaAIConfigParamType, float>();

        [Button("补全默认AI参数")]
        private void GenerateDefaultMechaAIParams()
        {
            foreach (MechaAIConfigParamType paramType in Enum.GetValues(typeof(MechaAIConfigParamType)))
            {
                if (!MechaAIParams.ContainsKey(paramType))
                {
                    MechaAIParams.Add(paramType, 0);
                }
            }
        }

        public MechaConfig ExportMechaConfig()
        {
            MechaConfig mechaConfig = new MechaConfig();
            mechaConfig.MechaConfigName = name;

            MechaComponentBase[] mcs = transform.GetComponentsInChildren<MechaComponentBase>();
            foreach (MechaComponentBase mcb in mcs)
            {
                MechaConfig.Config config = new MechaConfig.Config();
                config.MechaComponentKey = mcb.name;
                config.MechaComponentAlias = mcb.EditorAlias;
                config.MechaComponentQuality = mcb.EditorQuality;
                config.GridPosR = GridPosR.GetGridPosByLocalTrans(mcb.transform, 1);
                mechaConfig.MechaComponentList.Add(config);
            }

            BehaviourTreeOwner bto = GetComponent<BehaviourTreeOwner>();
            if (bto != null)
            {
                mechaConfig.MechaAIConfigKey = bto.behaviour.name;
            }

            mechaConfig.MechaAIParams = MechaAIParams.Clone();
            return mechaConfig;
        }
    }
}