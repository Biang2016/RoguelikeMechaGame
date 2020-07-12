using System;
using System.Collections.Generic;
using BiangStudio.GameDataFormat;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore.AbilityDataDriven
{
    public class Modifier
    {
        [LabelText("Modifier名称")]
        public string ModifierName;

        [LabelText("属性")]
        public ENUM_ModifierAttribute Attributes;

        [LabelText("持续时间")]
        [SuffixLabel("ms", true)]
        public int Duration;

        public bool IsBuff;
        public bool IsDeBuff;

        [LabelText("隐藏")]
        public bool IsHidden;

        [LabelText("被动触发")]
        public bool Passive;

        [LabelText("触发间隔")]
        [SuffixLabel("ms", true)]
        public int ThinkInterval;

        public Dictionary<ENUM_ModifierProperty, Fix64> Properties = new Dictionary<ENUM_ModifierProperty, Fix64>();

        public HashSet<ENUM_ModifierStates> States = new HashSet<ENUM_ModifierStates>();

        [LabelText("触发事件列表")]
        [ListDrawerSettings(ListElementLabelName = "EventName")]
        public List<Event> Events = new List<Event>();
    }
}