﻿using System;
using System.Collections.Generic;
using BiangStudio.CloneVariant;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GameCore.AbilityDataDriven
{
    public class GamePlayAbility : IClone<GamePlayAbility>
    {
        [LabelText("技能名称")]
        public string AbilityName;

        [LabelText("技能特征")]
        public ENUM_AbilityBehavior AbilityBehaviors;

        [LabelText("被动")]
        public bool Passive;

        [LabelText("施法点")]
        public ENUM_ProjectileDummyPosition CastDummyPosition;

        [LabelText("释放范围")]
        [SuffixLabel("unit", true)]
        public int AbilityCastRange;

        [LabelText("冷却时间")]
        [SuffixLabel("ms", true)]
        public int AbilityCooldown;

        [NonSerialized]
        public int cooldownTicker = 0;

        public bool canTriggered => cooldownTicker > AbilityCooldown;

        [LabelText("能量消耗")]
        public int AbilityPowerCost;

        [LabelText("Modifier定义列表")]
        [ListDrawerSettings(ListElementLabelName = "ModifierName")]
        public List<GamePlayModifier> Modifiers = new List<GamePlayModifier>();

        [LabelText("触发事件列表")]
        [ListDrawerSettings(ListElementLabelName = "EventName")]
        [ShowInInspector]
        public List<GamePlayEvent> Events = new List<GamePlayEvent>();

        public SortedDictionary<ENUM_Event, GamePlayEvent> EventDict
        {
            get
            {
                if (eventDict == null)
                {
                    eventDict = new SortedDictionary<ENUM_Event, GamePlayEvent>();
                    foreach (GamePlayEvent gamePlayEvent in Events)
                    {
                        eventDict.Add(gamePlayEvent.EventType, gamePlayEvent);
                    }
                }

                return eventDict;
            }
        }

        private SortedDictionary<ENUM_Event, GamePlayEvent> eventDict = null;

        public GamePlayAbility Clone()
        {
            return new GamePlayAbility
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