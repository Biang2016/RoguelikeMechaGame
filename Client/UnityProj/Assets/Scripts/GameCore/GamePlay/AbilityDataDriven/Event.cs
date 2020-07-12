using System.Collections.Generic;
using BiangStudio.CloneVariant;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore.AbilityDataDriven
{
    public class Event : IClone<Event>
    {
        [HideInInspector]
        public string EventName => EventType.ToString();

        [LabelText("时机")]
        public ENUM_Event EventType;

        [LabelText("行为列表")]
        [ListDrawerSettings(ListElementLabelName = "ActionName")]
        public List<Action> Actions = new List<Action>();

        public Event Clone()
        {
            return new Event
            {
                EventType = EventType,
                Actions = Actions.Clone(),
            };
        }
    }
}