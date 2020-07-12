using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore.AbilityDataDriven
{
    public class Ability
    {
        [LabelText("技能名称")]
        public string AbilityName;

        [LabelText("技能特征")]
        public ENUM_AbilityBehavior AbilityBehaviors;

        [LabelText("被动")]
        public bool Passive;

        [LabelText("释放点")]
        public Vector3 AbilityCastPoint;

        [LabelText("释放范围")]
        [SuffixLabel("unit", true)]
        public int AbilityCastRange;

        [LabelText("冷却时间")]
        [SuffixLabel("ms", true)]
        public int AbilityCooldown;

        [LabelText("能量消耗")]
        public int AbilityPowerCost;

        [LabelText("Modifier定义列表")]
        [ListDrawerSettings(ListElementLabelName = "ModifierName")]
        public List<Modifier> Modifiers = new List<Modifier>();

        [LabelText("触发事件列表")]
        [ListDrawerSettings(ListElementLabelName = "EventName")]
        public List<Event> Events = new List<Event>();
    }
}