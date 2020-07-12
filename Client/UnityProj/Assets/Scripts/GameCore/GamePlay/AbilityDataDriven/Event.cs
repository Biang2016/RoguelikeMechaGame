using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameCore.AbilityDataDriven
{
    public class Event
    {
        [HideInInspector]
        public string EventName => EventType.ToString();

        [LabelText("时机")]
        public ENUM_Event EventType;

        [LabelText("行为列表")]
        [ListDrawerSettings(ListElementLabelName = "ActionName")]
        public List<Action> Actions = new List<Action>();
    }
}