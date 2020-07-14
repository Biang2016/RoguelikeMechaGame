﻿using System.Collections.Generic;
using BiangStudio.CloneVariant;
using Sirenix.OdinInspector;

namespace GameCore.AbilityDataDriven
{
    public class Ability : IClone<Ability>
    {
        [LabelText("技能名称")]
        public string AbilityName;

        [LabelText("技能特征")]
        public ENUM_AbilityBehavior AbilityBehaviors;

        [LabelText("被动")]
        public bool Passive;

        [LabelText("施法点")]
        public ENUM_AbilityCastDummyPosition CastDummyPosition;

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

        public Ability Clone()
        {
            return new Ability
            {
                AbilityName = AbilityName,
                AbilityBehaviors = AbilityBehaviors,
                Passive = Passive,
                CastDummyPosition = CastDummyPosition,
                AbilityCastRange = AbilityCastRange,
                AbilityCooldown = AbilityCooldown,
                AbilityPowerCost = AbilityPowerCost,
                Modifiers = Modifiers.Clone(),
                Events = Events.Clone(),
            };
        }
    }
}