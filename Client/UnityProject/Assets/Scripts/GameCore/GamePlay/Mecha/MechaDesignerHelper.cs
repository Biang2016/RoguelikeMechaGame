using System;
using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine;
using GameCore;
using NodeCanvas.BehaviourTrees;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;

namespace GameCore
{
    [ExecuteInEditMode]
    public class MechaDesignerHelper : MonoBehaviour
    {
        [ShowInInspector]
        [LabelText("AI参数")]
        [OnInspectorGUI("DrawColoredHeader", append: false)]
        [ListDrawerSettings(ListElementLabelName = "ParamType", ShowIndexLabels = false)]
        public List<MechaAIParam> MechaAIParams = new List<MechaAIParam>();

        private string errorTip;

        private Dictionary<MechaAIConfigParamType, float> mechaAIParamDict = new Dictionary<MechaAIConfigParamType, float>();

        private Dictionary<MechaAIConfigParamType, float> MechaAIParamDict
        {
            get
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
                        errorTip = $"配置了重复的AI参数{aiParam.ParamType}";
                        Debug.LogError($"在{name}里配置了重复的AI参数{aiParam.ParamType}");
                        return mechaAIParamDict;
                    }
                }

                errorTip = "";
                return mechaAIParamDict;
            }
        }

#if UNITY_EDITOR
        private void DrawColoredHeader()
        {
            if (!errorTip.IsNullOrWhitespace())
            {
                SirenixEditorGUI.BeginBoxHeader();
                GUIHelper.PushColor(Color.magenta);
                GUILayout.Label(errorTip);
                GUIHelper.PopColor();
                SirenixEditorGUI.EndBoxHeader();
            }
        }
#endif

        void OnValidate()
        {
            mechaAIParamDict = MechaAIParamDict;
        }

        [Button("补全默认AI参数")]
        private void GenerateDefaultMechaAIParams()
        {
            foreach (MechaAIConfigParamType paramType in Enum.GetValues(typeof(MechaAIConfigParamType)))
            {
                if (!MechaAIParamDict.ContainsKey(paramType))
                {
                    MechaAIParams.Add(new MechaAIParam {ParamType = paramType, ParamValue = 0});
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